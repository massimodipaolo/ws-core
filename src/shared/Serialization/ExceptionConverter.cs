using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ws.Core.Shared.Serialization
{
    public class ExceptionConverter : JsonConverter<Exception>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(Exception).IsAssignableFrom(typeToConvert);
        }

        public override Exception Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException("Deserializing exceptions is not allowed");
        }

        public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
        {
            var serializableProperties = value?.GetType()?
                .GetProperties()?
                .Select(uu => new { uu.Name, Value = uu.GetValue(value) })?
                .Where(uu => uu.Name != nameof(Exception.TargetSite));
            if (serializableProperties != null)
            {
                if (options?.DefaultIgnoreCondition == JsonIgnoreCondition.Always)
                    serializableProperties = serializableProperties?.Where(uu => uu.Value != null);

                var propList = serializableProperties?.ToList();
                if (propList?.Any() == true)
                {
                    writer.WriteStartObject();
                    foreach (var prop in propList)
                    {
                        writer.WritePropertyName(prop.Name);
                        JsonSerializer.Serialize(writer, prop.Value, options);
                    }
                    writer.WriteEndObject();
                }
            }
        }
    }
}
