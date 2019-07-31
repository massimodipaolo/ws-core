using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace web.Code
{
    public interface IJsonConvertible { }

    public class LocaleText : ICollection<LocaleText>, IJsonConvertible
    {
        static LocaleText[] _init => new LocaleText[] { };
        LocaleText[] _items = _init;
        static string [] _cultures { get; set; }
        static LocaleText()
        {
            _cultures = new string[] { "it", "en" };            
        }

        [Required]
        public string Code { get; set; }
        public string Text { get; set; }

        public int Count => _items.Length;

        public bool IsReadOnly => false;

        public void Add(LocaleText item)
        {
            if (item != null)
                if (Contains(item))                    
                    _items.Single(_ => _.Code == item.Code).Text = item.Text;
                else
                {
                    if(_cultures.Contains(item.Code))
                        _items.Append(item);
                }
        }

        public void Clear()
        {
            _items = _init;
        }

        public bool Contains(LocaleText item) => _items.Any(_ => _.Code == item.Code);

        public void CopyTo(LocaleText[] array, int arrayIndex)
        {
            foreach (var item in array.GroupBy(_ => _.Code).Select(g => g.Last()))
                Add(item);
        }

        public IEnumerator<LocaleText> GetEnumerator()
        {
            foreach (LocaleText item in _items)
                if (item == null)
                    break;
                else
                    yield return item;
        }

        public bool Remove(LocaleText item)
        {
            if (item != null && _items.Contains(item))
            {                
                _items = _items.Where(_ => _.Code != item.Code).ToArray();
                return true;
            }                  
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class LocaleJsonConverter : JsonConverter
    {
        private static IHttpContextAccessor _context;
        private string _locale => _context?.HttpContext?.Request?.Query["locale"];
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
            writer.WriteValue(((IEnumerable<LocaleText>)value).FirstOrDefault(_ => _.Code == _locale)?.Text);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value != null)
            {
                var locale = new LocaleText();
                locale.Add(new LocaleText() { Code = _locale, Text = reader.Value.ToString() });
                return locale.ToArray();
                //return new List<LocaleText>() { new LocaleText() { Code = _language, Text = reader.Value.ToString() } };
            }                
            else
                return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return !string.IsNullOrEmpty(_locale) && typeof(IEnumerable<LocaleText>).IsAssignableFrom(objectType);
        }
    }
}
