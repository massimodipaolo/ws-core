using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace web.Code
{
    public interface IJsonConvertible { }
    public class LocaleText
    {
        public string LanguageId { get; set; }
        public string Text { get; set; }
    }

    public class LocaleTexts : List<LocaleText>, IJsonConvertible
    {
    }

    public class LocaleJsonConverter : JsonConverter
    {
        private static IHttpContextAccessor _context;
        private string _language => _context?.HttpContext?.Request?.Query["locale"];
        public LocaleJsonConverter(params object[] args)
        {
            _context = GetFromArgs<IHttpContextAccessor>(args);
        }

        private T GetFromArgs<T>(object[] args)
        {
            return (T)args.AsEnumerable().FirstOrDefault(_ => typeof(T).IsAssignableFrom(_.GetType()));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((IEnumerable<LocaleText>)value).FirstOrDefault(_ => _.LanguageId == _language)?.Text);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value != null)
                return new List<LocaleText>() { new LocaleText() { LanguageId = _language, Text = reader.Value.ToString() } };
            else
                return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return !string.IsNullOrEmpty(_language) && typeof(IEnumerable<LocaleText>).IsAssignableFrom(objectType);
        }
    }
}
