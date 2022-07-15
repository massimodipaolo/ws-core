using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;

namespace Ws.Core.Extensions.Data.EF;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    private static ILogger? _logger { get; set; }
    private static List<Type> _ignoreTypes { get; set; } = new();
    private static IEnumerable<Type> _jsonConvertTypes { get; set; } = Array.Empty<Type>();
    public DbContext(DbContextOptions options, ILogger logger) : base(options)
    {
        if (_logger == null) _logger = logger;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        EF.Options options = new EF.Extension().Options;

        _ignorePrimitiveTypes(modelBuilder);
        _ignoreCustomTypes(modelBuilder, options);

        // JsonConvert: map field with text columun, serializing/deserializing value
        if (options?.JsonConvert?.Any() == true)
            _jsonConvertTypes = options.JsonConvert.Select(_ => Type.GetType(_) ?? typeof(object));

        _map(modelBuilder, options);

    }

    /// <summary>
    /// Ignore common unsupported array of primitive types
    /// </summary>
    /// <param name="modelBuilder"></param>
    private static void _ignorePrimitiveTypes(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Ignore<int[]>()
            .Ignore<long[]>()
            .Ignore<Guid[]>()
            .Ignore<string[]>();
    }

    /// <summary>
    /// Ignore custom type    
    /// </summary>
    /// <param name="modelBuilder"></param>
    /// <param name="options"></param>
    private static void _ignoreCustomTypes(ModelBuilder modelBuilder, Options options)
    {
        if (options?.Ignore?.Any() == true)
            foreach (var type in options.Ignore)
                try
                {
                    Type? t = Type.GetType(type);
                    if (t != null)
                    {
                        _ignoreTypes.Add(t);
                        modelBuilder.Ignore(t);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Error on ignore custom type '{type}'", type);
                }
    }

    private static void _map(ModelBuilder modelBuilder, Options? options)
    {
        var tKeys = new KeyValuePair<Type, int>[] {
            new KeyValuePair<Type,int>(typeof(IEntity<int>),11),
            new KeyValuePair<Type,int>(typeof(IEntity<long>),20),
            new KeyValuePair<Type, int>(typeof(IEntity<Guid>),36),
            new KeyValuePair<Type, int>(typeof(IEntity<string>),255)
        };

        foreach (KeyValuePair<Type, int> tKey in tKeys)
        {
            var types = Base.Util.GetAllTypesOf(tKey.Key)?.Where(t => t != null && !_ignoreTypes?.Distinct().Any(i => i == t) == true);
            if (types?.Any() == true)
                foreach (Type type in types)
                {
                    try
                    {
                        EntityTypeBuilder entityBuilder = modelBuilder.Entity(type);

                        // get options
                        EF.Options.MappingConfig? opt = (options?.Mappings ?? new List<EF.Options.MappingConfig>())
                        .FirstOrDefault(_ =>
                            _.Name == type.Name
                            && (string.IsNullOrEmpty(_.NameSpace) || _.NameSpace == type.Namespace)
                        );

                        _mapTable(entityBuilder, type, opt);
                        _mapIdColumn(entityBuilder, tKey, opt);
                        _mapJsonConversion(entityBuilder, type, opt);
                        _mapProperties(entityBuilder, type, opt);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error on map type '{type}'", nameof(type));
                    }
                }
        }
    }

    /// <summary>
    /// Map to db schema,table
    /// </summary>
    /// <param name="entityBuilder"></param>
    /// <param name="type"></param>
    /// <param name="opt"></param>
    private static void _mapTable(EntityTypeBuilder entityBuilder, Type type, EF.Options.MappingConfig? opt)
    {
        // https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-3.0/breaking-changes#totable-on-a-derived-type-throws-an-exception
        // https://github.com/aspnet/EntityFrameworkCore/issues/11811
        if (
            type.BaseType == typeof(object) // i.e. partial class
            ||
            type.BaseType == type // IEntity<T>
            ||
            type.BaseType?.BaseType == typeof(object) // i.e. Entity<T>
            )
            entityBuilder.ToTable(opt?.Table ?? type.Name, opt?.Schema);
    }
    /// <summary>
    /// Map Id column
    /// </summary>
    private static void _mapIdColumn(EntityTypeBuilder entityBuilder, KeyValuePair<Type, int> tKey, EF.Options.MappingConfig? opt)
    {
        var idBuilder = entityBuilder.Property("Id").HasColumnName(opt?.IdColumnName ?? "Id")
            .IsUnicode(false)
            .HasMaxLength(tKey.Value)                                  
            ;

        // https://github.com/aspnet/EntityFrameworkCore/issues/16814
        if (opt?.IdHasDefaultValue == true)
            idBuilder.HasDefaultValue();
    }

    /// <summary>
    /// Map complex type (or interface) on a text column, serializing/deserializing value
    /// </summary>
    /// <param name="entityBuilder"></param>
    /// <param name="type"></param>
    /// <param name="opt"></param>
    private static void _mapJsonConversion(EntityTypeBuilder entityBuilder, Type type, EF.Options.MappingConfig? opt)
    {
        if (_jsonConvertTypes != null && _jsonConvertTypes.Any())
            foreach (var property in type.GetProperties()
                .Where(p => _jsonConvertTypes
                    .Any(jT => jT.IsInterface ? jT.IsAssignableFrom(p.PropertyType) : jT == p.PropertyType)
                    )
                )
                if (null == opt?.Properties?.FirstOrDefault(_ => _.Name == property.Name && _.JsonConvert.HasValue && !_.JsonConvert.Value))
                    entityBuilder.Property(property.Name).HasJsonConversion(property.PropertyType);
    }

    /// <summary>
    /// Property based settings
    /// </summary>
    /// <param name="entityBuilder"></param>
    /// <param name="type"></param>
    /// <param name="opt"></param>
    private static void _mapProperties(EntityTypeBuilder entityBuilder, Type type, EF.Options.MappingConfig? opt)
    {
        if (opt?.Properties != null)
            foreach (var p in opt.Properties.Where(_ => !string.IsNullOrEmpty(_.Name)))
            {
                _mapPropertiesBasicConversion(entityBuilder, p);
                _mapPropertiesJsonConversion(entityBuilder, type, p);
                _mapPropertiesClrConversion(entityBuilder, type, p);
            }
    }

    /// <summary>
    /// Map specific property to custom column, or ignore
    /// </summary>
    private static void _mapPropertiesBasicConversion(EntityTypeBuilder entityBuilder, Options.MappingConfig.PropertyConfig p)
    {
        if (!string.IsNullOrEmpty(p.Name))
        {
            // Ignore field
            if (p.Ignore)
                entityBuilder.Ignore(p.Name);
            else
                try
                {
                    // Custom map
                    if (!string.IsNullOrEmpty(p.Column))
                        entityBuilder.Property(p.Name).HasColumnName(p.Column);                        
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Error on map property '{p}'", $"{p.Name} > {p.Column}");
                }
        }
    }

    /// <summary>
    /// Map specific property on a text column, serializing/deserializing value
    /// </summary>
    private static void _mapPropertiesJsonConversion(EntityTypeBuilder entityBuilder, Type type, Options.MappingConfig.PropertyConfig p)
    {
        if ((p?.JsonConvert ?? false) && !string.IsNullOrEmpty(p.Name))
            entityBuilder.Property(p.Name)?.HasJsonConversion(type.GetProperty(p.Name)?.PropertyType ?? typeof(object));
    }

    /// <summary>
    /// Map specific property to CLR type
    /// </summary>
    private static void _mapPropertiesClrConversion(EntityTypeBuilder entityBuilder, Type type, Options.MappingConfig.PropertyConfig p)
    {
        if (!string.IsNullOrEmpty(p.HasConversion) && !string.IsNullOrEmpty(p.Name))
        {
            var _type = p.HasConversion.ToLower();
            if (_type == "json")
                entityBuilder.Property(p.Name).HasJsonConversion(type.GetProperty(p.Name)?.PropertyType ?? typeof(object));
            else
            {
                Type? clrType = EF.Options.MappingConfig.PropertyConfig.ColumnClrTypeConversions?.FirstOrDefault(_ => _.Name.ToLower() == _type);
                if (clrType != null)
                    entityBuilder.Property(p.Name).HasConversion(clrType);
            }
        }
    }
}

public class DbContext<TContext> : EF.DbContext where TContext : EF.DbContext
{
    public DbContext(DbContextOptions<TContext> options, ILogger<DbContext> logger) : base(options, logger) { }
}