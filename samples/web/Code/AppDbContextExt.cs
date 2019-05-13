using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ws.Core.Extensions.Data;

namespace web.Code
{
    public class AppDbContextExt : AppDbContext
    {
        static AppDbContextExt()
        {

        }
        public AppDbContextExt(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /*
            var converter = new ValueConverter<IEnumerable<LocaleText>, string>(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<IEnumerable<LocaleText>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                );

            
            var entityBuilder = modelBuilder.Entity(typeof(Page));
            entityBuilder.Property("Title")
                .HasConversion(converter);
            */
            
            var entityBuilder = modelBuilder.Entity<Page>();
            entityBuilder.Property(e => e.Title).HasJsonConversion();
            entityBuilder.Property(e => e.Abstract).HasJsonConversion();


        }
    }

    // https://stackoverflow.com/a/53051419/11074305
    public static class ValueConversionExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder) where T: class
        {
            ValueConverter<T,string> converter = new ValueConverter<T,string>(
                t => JsonConvert.SerializeObject(t ?? default, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                s => JsonConvert.DeserializeObject<T>(s, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            ValueComparer<T> comparer = new ValueComparer<T>(
                (l, r) => JsonConvert.SerializeObject(l) == JsonConvert.SerializeObject(r),
                v => v == null ? 0 : JsonConvert.SerializeObject(v ?? default).GetHashCode(),
                v => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(v)));

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);

            return propertyBuilder;
        }
    }

}
