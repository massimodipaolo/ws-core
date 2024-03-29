﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Ws.Core.Extensions.Data.EF;

public static class ValueConversionExtensions
{
    private static readonly JsonSerializerSettings _jsonSettings = new() { NullValueHandling = NullValueHandling.Ignore };

    public static PropertyBuilder HasJsonConversion(this PropertyBuilder propertyBuilder, Type type)
    {

        JsonValueConverter<object, string> converter = new (
            t => JsonConvert.SerializeObject(t ?? default, type, _jsonSettings),
            s => JsonConvert.DeserializeObject(s, type, _jsonSettings) ?? new object(),
            type,
            null
        );

        JsonValueComparer<object> comparer = new (
            (l, r) => JsonConvert.SerializeObject(l, type, _jsonSettings) == JsonConvert.SerializeObject(r, type, _jsonSettings),
            v => v == null ? 0 : JsonConvert.SerializeObject(v, type, _jsonSettings).GetHashCode(),
            type,
            _jsonSettings
        );

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);

        return propertyBuilder;
    }
}

public class JsonValueConverter<TModel, TProvider> : ValueConverter<TModel, TProvider>
{
    private Type _tModel { get; set; }
    public JsonValueConverter(
        Expression<Func<TModel, TProvider>> convertToProviderExpression,
        Expression<Func<TProvider, TModel>> convertFromProviderExpression,
        Type tModel,
        ConverterMappingHints? mappingHints = null) 
        : base(convertToProviderExpression, convertFromProviderExpression, mappingHints)
    {
        _tModel = tModel;
    }
    public override Type ModelClrType => _tModel;
}

public class JsonValueComparer<T> : ValueComparer<T>
{
    private Type _type { get; set; }
    private JsonSerializerSettings _jsonSettings { get; set; }
    public JsonValueComparer(
        Expression<Func<T?, T?, bool>> equalsExpression,
        Expression<Func<T, int>> hashCodeExpression,
        Type type,
        JsonSerializerSettings jsonSettings
        ) : base(equalsExpression, hashCodeExpression/*, snapshotExpression*/)
    {
        _type = type;
        _jsonSettings = jsonSettings;
    }
    public override Type Type => _type;
    public override object? Snapshot(object? instance)
    {
        return instance == null ? default : JsonConvert.DeserializeObject(JsonConvert.SerializeObject(instance, _type, _jsonSettings), _type, _jsonSettings);
    }
}
