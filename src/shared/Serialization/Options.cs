using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ws.Core.Shared.Serialization
{
    public class Options
    {
        public Newtonsoft.Json.NullValueHandling NullValueHandling { get; set; } = Newtonsoft.Json.NullValueHandling.Ignore;
        public Newtonsoft.Json.Formatting Formatting { get; set; } = Newtonsoft.Json.Formatting.None;
        public Newtonsoft.Json.ReferenceLoopHandling ReferenceLoopHandling { get; set; } = Newtonsoft.Json.ReferenceLoopHandling.Error;
        public Newtonsoft.Json.DateParseHandling DateParseHandling { get; set; } = Newtonsoft.Json.DateParseHandling.DateTime;
        public Newtonsoft.Json.DateTimeZoneHandling DateTimeZoneHandling { get; set; } = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
        /// <summary>
        /// List of assembly/JsonConvert type to apply
        /// </summary>
        public JsonConverterDiscover[] Converters { get; set; } = new JsonConverterDiscover[] { };

        public class JsonConverterDiscover
        {
            /// <summary>
            /// Assembly full name
            /// </summary>
            public string Assembly { get; set; }
            // JsonConverter class full name
            public string Type { get; set; }
        }

        public Newtonsoft.Json.JsonSerializerSettings ToJsonSerializerSettings()
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                NullValueHandling = this.NullValueHandling,
                Formatting = this.Formatting,
                ReferenceLoopHandling = this.ReferenceLoopHandling,
                DateParseHandling = this.DateParseHandling,
                DateTimeZoneHandling = this.DateTimeZoneHandling
            };
            return settings;
        }

        public void FromJsonSerializerSettings(ref Newtonsoft.Json.JsonSerializerSettings settings)
        {
            settings.NullValueHandling = this.NullValueHandling;
            settings.Formatting = this.Formatting;
            settings.ReferenceLoopHandling = this.ReferenceLoopHandling;
            settings.DateParseHandling = this.DateParseHandling;
            settings.DateTimeZoneHandling = this.DateTimeZoneHandling;
            AddConverters(ref settings);
        }

        private void AddConverters(ref Newtonsoft.Json.JsonSerializerSettings settings)
        {
            if (Converters != null && Converters.Any())
                foreach (var converter in Converters.Where(_ => null != _))
                {
                    var assembly = AppDomain.CurrentDomain.GetAssemblies().AsEnumerable().Where(_ => _.FullName?.Split(',')[0] == converter.Assembly).FirstOrDefault();
                    if (null != assembly)
                    {
                        try
                        {
                            Type converterType = System.Reflection.Assembly.LoadFrom(assembly.Location).GetType(converter.Type);
                            if (typeof(Newtonsoft.Json.JsonConverter).IsAssignableFrom(converterType))
                            {
                                var obj = (Newtonsoft.Json.JsonConverter)Activator.CreateInstance(
                                    converterType,
                                    new object[] {
                                    new Microsoft.AspNetCore.Http.HttpContextAccessor()
                                    });
                                settings.Converters.Add(obj);
                            }
                        }
                        catch { }
                    }
                }
        }

    }
}
