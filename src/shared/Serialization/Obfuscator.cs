using System.Collections;
using System.Reflection;

namespace Ws.Core.Shared.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SensitiveDataAttribute : Attribute { }

    public static class Obfuscator
    {

        private const string Masked = "***";
        internal record PropertyInfos
        {
            internal HashSet<PropertyInfo> Sensitive { get; set; } = new();
            internal HashSet<PropertyInfo> SensitiveCandidate { get; set; } = new();
        }

        public static object MaskSensitiveData(object value) => _mask(value) ?? value;
        private static readonly Dictionary<Type, PropertyInfos?> _sensitiveDataTypes = _sensitiveDataTypesInit();
        private static Dictionary<Type, PropertyInfos?> _sensitiveDataTypesInit()
        {
            Dictionary<Type, PropertyInfos?> d = new();
            IEnumerable<Type>? types = Ws.Core.Extensions.Base.Util.GetAllTypes()?.Where(_ =>
                _.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Public)?
                .Any(__ => __.GetCustomAttribute<Ws.Core.Shared.Serialization.SensitiveDataAttribute>() != null) == true
            );
            if (types is IEnumerable)
                foreach (Type type in types)
                {
                    PropertyInfos? p = _getSensitivePropsByType(type);
                    if (p != null)
                        d.Add(type, p);
                }
            return d;
        }

        private static PropertyInfos? _getSensitivePropsByType(Type type)
        {
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (props?.Any(_ => _.GetCustomAttribute<Ws.Core.Shared.Serialization.SensitiveDataAttribute>() != null) == true)
            {
                HashSet<PropertyInfo> _sensitive = new();
                HashSet<PropertyInfo> _sensitiveCandidate = new();
                foreach (PropertyInfo p in props)
                {
                    if (Attribute.IsDefined(p, typeof(SensitiveDataAttribute)))
                        _sensitive.Add(p);
                    else if (!(p.PropertyType == typeof(string) || p.PropertyType.IsValueType))
                        _sensitiveCandidate.Add(p);
                }
                return new PropertyInfos()
                {
                    Sensitive = _sensitive,
                    SensitiveCandidate = _sensitiveCandidate
                };
            }
            return null;
        }

        private static object? _mask(object? obj)
        {
            if (obj != null)
            {
                if (obj is not string && obj is IEnumerable enumerable)
                    _maskArrayElements(ref enumerable);
                else
                    _maskProperties(ref obj);

                return obj;
            }
            return null;
        }

        private static void _maskArrayElements(ref IEnumerable obj)
        {
            foreach (var item in obj)
                _mask(item);
        }
        private static void _maskProperties(ref object obj)
        {
            var type = obj.GetType();
            if (type == null || type == typeof(string) || type.IsValueType) return;
            if (_sensitiveDataTypes.TryGetValue(type, out PropertyInfos? props) && props != null)
            {
                foreach (PropertyInfo p in props.Sensitive)
                    p.SetValue(obj, p.PropertyType == typeof(string) ? Masked : default);
                foreach (PropertyInfo p in props.SensitiveCandidate)
                    _mask(p.GetValue(obj));
            }
        }
    }
}