using NLog;
using System.Reflection;
using System.Text;

namespace Ws.Core
{
    /// <summary>
    /// https://stackoverflow.com/questions/64884119/c-sharp-how-to-write-a-custom-json-serializer-that-changes-properties-based-on
    /// https://stackoverflow.com/questions/33148957/replace-sensitive-data-value-on-json-serialization
    /// https://stackoverflow.com/questions/37821298/how-to-mask-sensitive-values-in-json-for-logging-purposes
    /// </summary>
    public class LogJsonConverter : NLog.IJsonConverter
    {
        private static IJsonConverter _jsonConverter { get; } = (IJsonConverter)(new LogFactory().ServiceRepository.GetService(typeof(IJsonConverter)));

        public bool SerializeObject(object value, StringBuilder builder)
        => _jsonConverter.SerializeObject(Ws.Core.Shared.Serialization.Obfuscator.MaskSensitiveData(value), builder);
    }
}
