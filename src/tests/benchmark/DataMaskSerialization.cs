using BenchmarkDotNet.Attributes;
using System.Collections;
using Ws.Core.Shared.Serialization;
using x.core.Models;

namespace benchmark;

[SimpleJob(warmupCount: 1, launchCount: 1, invocationCount: 2, targetCount: 10)]
//[EventPipeProfiler(BenchmarkDotNet.Diagnosers.EventPipeProfile.CpuSampling)] // => https://https://www.speedscope.app/
[MemoryDiagnoser]
//[RPlotExporter]
public class DataMaskSerialization
{
    private static readonly MaskedUser user = new()
    {
        Id = 1,
        Name = "Joe Doe",
        Email = "secret@email.com",
        Phone = "123456789",
        Address = new() { Street = "No name street", City = "No name city", Geo = new() { Lat = "10", Lng = "10" } },
        Company = new() { Name = "Nasa", CatchPhrase = "#htfu" },
        Enemies = new[] { new Company() { Name = "U.R.S.S.", CatchPhrase = "!!!" } },
        Posts = new[] { new Post() { Id = 1, Title = "Public title", Body = "Not so secret text", UserId = 1, Comments = new[] { new Comment() { Id = 1, PostId = 1, Name = "Comment name", Body = "secret comment", Email = "author@email.com" } } } },
        CreatedAt = DateTime.Now
    };
    private static readonly MaskedUser? user2 = System.Text.Json.JsonSerializer.Deserialize<MaskedUser>(System.Text.Json.JsonSerializer.Serialize(user));

    [Benchmark]
    public static void Base()
    {
        _ = Ws.Core.Shared.Serialization.Obfuscator.MaskSensitiveData(user);
    }

    [Benchmark]
    public static void RuntimeReflection()
    {
        _ = RuntimeReflectionObfuscator.MaskSensitiveData(user2);
    }

}

public static class RuntimeReflectionObfuscator
{
    private const string Masked = "***";

    public static object? MaskSensitiveData(object? value) => _mask(value) ?? value;

    private static object? _mask(object? obj, Type? type = null)
    {
        try
        {
            if (obj != null)
            {
                if ((type ?? obj.GetType()).IsArray)
                    _maskArrayElements(ref obj);
                else
                    _maskProperties(ref obj);

                return obj;
            }
        }
        catch
        {
            //Die quietly :'(
        }
        return null;
    }

    private static void _maskArrayElements(ref object obj)
    {
        if (obj is IEnumerable elements)
        {
            var elementType = obj.GetType()?.GetElementType();
            foreach (var element in elements)
                _mask(element, elementType);
        }
    }

    private static void _maskProperties(ref object obj)
    {
        if (obj != null)
        {
            var type = obj.GetType();

            if (type == null)
                return;

            if (type.IsArray)
                _maskArrayElements(ref obj);
            else
                foreach (var property in type.GetProperties().Where(x => x.PropertyType.IsPublic))
                    if (Attribute.IsDefined(property, typeof(SensitiveDataAttribute)))
                        //mask string, or default value
                        property.SetValue(obj, property.PropertyType == typeof(string) || type == typeof(string) ? Masked : default);
                    else if (property.PropertyType.IsArray || !property.PropertyType.IsValueType)
                        _mask(property.GetValue(obj), property.PropertyType);
        }
    }
}