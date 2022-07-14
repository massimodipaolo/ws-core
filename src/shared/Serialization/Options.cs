using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Ws.Core.Shared.Serialization
{
    public class Options
    {
        [DefaultValue(NullValueHandlingOptions.Ignore)]
        public NullValueHandlingOptions NullValueHandling { get; set; } = NullValueHandlingOptions.Ignore;
        [DefaultValue(FormattingOptions.None)]
        public FormattingOptions Formatting { get; set; } = FormattingOptions.None;
        [DefaultValue(ReferenceLoopHandlingOptions.Serialize)]
        public ReferenceLoopHandlingOptions ReferenceLoopHandling { get; set; } = ReferenceLoopHandlingOptions.Serialize;

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
        [Description("List of assembly/JsonConvert type to apply")]
        public JsonConverterDiscover[] Converters { get; set; } = Array.Empty<JsonConverterDiscover>();

        public class JsonConverterDiscover
        {
            /// <summary>
            /// Assembly full name
            /// </summary>
            [Description("Assembly full name")]
            public string? Assembly { get; set; }
            // JsonConverter class full name
            [Description("JsonConverter class full name")]
            public string? Type { get; set; }
        }

        public System.Text.Json.JsonSerializerOptions ToJsonSerializerSettings()
        {
            var settings = new System.Text.Json.JsonSerializerOptions() { };
            FromJsonSerializerSettings(ref settings);
            return settings;
        }
        public void FromJsonSerializerSettings(ref System.Text.Json.JsonSerializerOptions settings)
        {
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
            _addDefaultConverters(ref settings);
            if (Converters?.Any() == true)
                foreach (var converter in Converters.Where(_ => _ != null))
                    if (_loadConverter(converter) is System.Text.Json.Serialization.JsonConverter jsonConverter && !settings.Converters.Any(_ => _.ToString() == jsonConverter.ToString()))
                        settings.Converters.Add(jsonConverter);
        }

        private static void _addDefaultConverters(ref System.Text.Json.JsonSerializerOptions settings)
        {
            settings.Converters.Add(new Serialization.ExceptionConverter());
        }

        private static object? _loadConverter(JsonConverterDiscover converter)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().AsEnumerable()?.FirstOrDefault(_ => _.FullName?.Split(',')[0] == converter.Assembly);
            if (assembly != null && converter != null)
            {
                Type? converterType = System.Reflection.Assembly.Load(System.IO.File.ReadAllBytes(assembly.Location))?.GetType(converter.Type ?? "");
                if (converterType != null && typeof(System.Text.Json.Serialization.JsonConverter).IsAssignableFrom(converterType))
                    return _createConverter(converterType);
            }
            return null;
        }
        private static object? _createConverter(Type converterType)
        {
            object? obj;
            try
            {
                // try parms object[] parameters
                obj = Activator.CreateInstance(
                        converterType,
                        new object[] { new Microsoft.AspNetCore.Http.HttpContextAccessor() }
                        );
            }
            catch
            {
                // Try parameterless ctor
                obj = Activator.CreateInstance(converterType);
            }
            return obj;
        }
    }
}
