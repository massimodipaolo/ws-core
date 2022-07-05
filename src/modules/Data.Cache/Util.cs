using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Core.Extensions.Data.Cache
{
    public static class Util
    {
        private static readonly System.Text.Json.JsonSerializerOptions _defaultSerializeOption = new()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
            WriteIndented = false,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            PropertyNameCaseInsensitive = true
        };
        public static byte[] ObjToByte(object value, System.Text.Json.JsonSerializerOptions? serializeOptions = null)
        => Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(value, serializeOptions ?? _defaultSerializeOption)) ?? Array.Empty<byte>();
    }
}