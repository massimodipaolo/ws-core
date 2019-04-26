using System;
using System.Collections.Generic;
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

        public Newtonsoft.Json.JsonSerializerSettings ToJsonSerializerSettings()
        {
            return new Newtonsoft.Json.JsonSerializerSettings()
            {                
                NullValueHandling = this.NullValueHandling,
                Formatting = this.Formatting,
                ReferenceLoopHandling = this.ReferenceLoopHandling,
                DateParseHandling = this.DateParseHandling,
                DateTimeZoneHandling = this.DateTimeZoneHandling
            };
        }

        public void FromJsonSerializerSettings(ref Newtonsoft.Json.JsonSerializerSettings settings)
        {
            settings.NullValueHandling = this.NullValueHandling;
            settings.Formatting = this.Formatting;
            settings.ReferenceLoopHandling = this.ReferenceLoopHandling;
            settings.DateParseHandling = this.DateParseHandling;
            settings.DateTimeZoneHandling = this.DateTimeZoneHandling;         
        }

    }
}
