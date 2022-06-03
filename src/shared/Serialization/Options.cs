using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ws.Core.Shared.Serialization
{
    public class Options
    {
        
        public NullValueHandlingOptions NullValueHandling { get; set; } = NullValueHandlingOptions.Ignore;
        public FormattingOptions Formatting { get; set; } = FormattingOptions.None;
        public ReferenceLoopHandlingOptions ReferenceLoopHandling { get; set; } = ReferenceLoopHandlingOptions.Serialize;

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?pivots=dotnet-6-0#support-polymorphic-deserialization
        /// </summary>
        /*
        public Newtonsoft.Json.DateParseHandling DateParseHandling { get; set; } = Newtonsoft.Json.DateParseHandling.DateTime;
        public Newtonsoft.Json.DateTimeZoneHandling DateTimeZoneHandling { get; set; } = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
        public Newtonsoft.Json.TypeNameHandling TypeNameHandling { get; set; } = Newtonsoft.Json.TypeNameHandling.None;
        public Newtonsoft.Json.TypeNameAssemblyFormatHandling TypeNameAssemblyFormatHandling { get; set; } = Newtonsoft.Json.TypeNameAssemblyFormatHandling.Simple;
        */

        #region enum options
        //
        // Summary:
        //     Specifies null value handling options for the Newtonsoft.Json.JsonSerializer.
        public enum NullValueHandlingOptions
        {
            //
            // Summary:
            //     Include null values when serializing and deserializing objects.
            Include,
            //
            // Summary:
            //     Ignore null values when serializing and deserializing objects.
            Ignore
        }

        //
        // Summary:
        //     Specifies formatting options for the Newtonsoft.Json.JsonTextWriter.
        public enum FormattingOptions
        {
            //
            // Summary:
            //     No special formatting is applied. This is the default.
            None,
            //
            // Summary:
            //     Causes child objects to be indented according to the Newtonsoft.Json.JsonTextWriter.Indentation
            //     and Newtonsoft.Json.JsonTextWriter.IndentChar settings.
            Indented
        }

        //
        // Summary:
        //     Specifies reference loop handling options for the Newtonsoft.Json.JsonSerializer.
        public enum ReferenceLoopHandlingOptions
        {
            // Summary:
            //     Ignore loop references and do not serialize.
            Ignore,
            //
            // Summary:
            //     Serialize loop references.
            Serialize
        }
        #endregion 

        /// <summary>
        /// List of assembly/JsonConvert type to apply
        /// </summary>
        public JsonConverterDiscover[] Converters { get; set; } = Array.Empty<JsonConverterDiscover>();

        public class JsonConverterDiscover
        {
            /// <summary>
            /// Assembly full name
            /// </summary>
            public string Assembly { get; set; }
            // JsonConverter class full name
            public string Type { get; set; }
        }

        /*
        public Newtonsoft.Json.JsonSerializerSettings ToJsonSerializerSettings()
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                NullValueHandling = this.NullValueHandling,
                Formatting = this.Formatting,
                ReferenceLoopHandling = this.ReferenceLoopHandling,
                DateParseHandling = this.DateParseHandling,
                DateTimeZoneHandling = this.DateTimeZoneHandling,
                TypeNameHandling = this.TypeNameHandling,
                TypeNameAssemblyFormatHandling = this.TypeNameAssemblyFormatHandling
            };
            return settings;
        }
        */

        public System.Text.Json.JsonSerializerOptions ToJsonSerializerSettings()
        {
            var settings = new System.Text.Json.JsonSerializerOptions() {};
            FromJsonSerializerSettings(ref settings);
            return settings;
        }

        /*
        public void FromJsonSerializerSettings(ref Newtonsoft.Json.JsonSerializerSettings settings)
        {
            settings.NullValueHandling = this.NullValueHandling;
            settings.Formatting = this.Formatting;
            settings.ReferenceLoopHandling = this.ReferenceLoopHandling;
            settings.DateParseHandling = this.DateParseHandling;
            settings.DateTimeZoneHandling = this.DateTimeZoneHandling;
            settings.TypeNameHandling = this.TypeNameHandling;
            settings.TypeNameAssemblyFormatHandling = this.TypeNameAssemblyFormatHandling;
            AddConverters(ref settings);
        }
        */
        public void FromJsonSerializerSettings(ref System.Text.Json.JsonSerializerOptions settings)
        {
            /*
            options.NullValueHandling = this.NullValueHandling;
            options.Formatting = this.Formatting;
            options.ReferenceLoopHandling = this.ReferenceLoopHandling;
            options.DateParseHandling = this.DateParseHandling;
            options.DateTimeZoneHandling = this.DateTimeZoneHandling;
            options.TypeNameHandling = this.TypeNameHandling;
            options.TypeNameAssemblyFormatHandling = this.TypeNameAssemblyFormatHandling;
            */
            if (NullValueHandling == NullValueHandlingOptions.Ignore)
                settings.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
            settings.WriteIndented = (Formatting == FormattingOptions.Indented);
            settings.ReferenceHandler = (ReferenceLoopHandling == ReferenceLoopHandlingOptions.Ignore) ?
                   System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles :
                   System.Text.Json.Serialization.ReferenceHandler.Preserve
                   ;
            settings.AllowTrailingCommas = true;
            settings.PropertyNameCaseInsensitive = true;
           
            AddConverters(ref settings);
        }

        private void AddConverters(ref System.Text.Json.JsonSerializerOptions settings)
        {
            if (Converters != null && Converters.Any())
                foreach (var converter in Converters.Where(_ => _ != null))
                {
                    var assembly = AppDomain.CurrentDomain.GetAssemblies().AsEnumerable().Where(_ => _.FullName?.Split(',')[0] == converter.Assembly).FirstOrDefault();
                    if (null != assembly)
                    {
                        try
                        {
                            Type converterType = System.Reflection.Assembly.LoadFrom(assembly.Location).GetType(converter.Type);
                            if (typeof(System.Text.Json.Serialization.JsonConverter).IsAssignableFrom(converterType))
                            {
                                System.Text.Json.Serialization.JsonConverter obj = null;
                                try
                                {
                                    // try parms object[] parameters
                                    obj = (System.Text.Json.Serialization.JsonConverter)Activator.CreateInstance(
                                            converterType,
                                            new object[] {
                                                new Microsoft.AspNetCore.Http.HttpContextAccessor()
                                            });
                                }
                                catch
                                {
                                    // Try parameterless ctor
                                    obj = (System.Text.Json.Serialization.JsonConverter)Activator.CreateInstance(converterType);
                                }
                                if (obj != null)
                                    settings.Converters.Add(obj);
                            }
                        }
                        catch { }
                    }
                }
        }

        /*
        private void AddConverters(ref Newtonsoft.Json.JsonSerializerSettings settings)
        {
            if (Converters != null && Converters.Any())
                foreach (var converter in Converters.Where(_ => _ != null))
                {
                    var assembly = AppDomain.CurrentDomain.GetAssemblies().AsEnumerable().Where(_ => _.FullName?.Split(',')[0] == converter.Assembly).FirstOrDefault();
                    if (null != assembly)
                    {
                        try
                        {
                            Type converterType = System.Reflection.Assembly.LoadFrom(assembly.Location).GetType(converter.Type);
                            if (typeof(Newtonsoft.Json.JsonConverter).IsAssignableFrom(converterType))
                            {
                                Newtonsoft.Json.JsonConverter obj = null;
                                try
                                {
                                    // try parms object[] parameters
                                    obj = (Newtonsoft.Json.JsonConverter)Activator.CreateInstance(
                                            converterType,
                                            new object[] {
                                                new Microsoft.AspNetCore.Http.HttpContextAccessor()
                                            });
                                }
                                catch
                                {
                                    // Try parameterless ctor
                                    obj = (Newtonsoft.Json.JsonConverter)Activator.CreateInstance(converterType);
                                }
                                if (obj != null)
                                    settings.Converters.Add(obj);
                            }
                        }
                        catch { }
                    }
                }
        }
        */
    }
}
