using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ws.Core.Extensions.Data;

namespace web.Code
{
    public interface IJsonConvertible { }

    public class LocaleText : HashSet<LocaleTextItem>, IJsonConvertible
    {
        private static LocaleTextItem[] _init => new LocaleTextItem[] { };
        private LocaleTextItem[] _items = _init;
        private static string[] _cultures { get; set; }
        static LocaleText()
        {
            _cultures = new string[] { "it", "en" };
        }

        public LocaleText() : base(new LocaleTextItemComparer()) { }

        public LocaleText(IEnumerable<LocaleTextItem> items) : base(items, new LocaleTextItemComparer()) { }
        public new int Count => _items.Length;

        public bool IsReadOnly => false;

        public new void Add(LocaleTextItem item)
        {
            if (item != null)
                if (Contains(item))
                    _items.Single(_ => _.Equals(item)).Text = item.Text;
                else
                {
                    if (_cultures.Contains(item.Id))
                        _items.Append(item);
                }
        }

        public new void Clear()
        {
            _items = _init;
        }

        public new bool Contains(LocaleTextItem item) => _items.Any(_ => _.Equals(item));

        public new void CopyTo(LocaleTextItem[] array, int arrayIndex)
        {
            foreach (var item in array.GroupBy(_ => _.Id).Select(g => g.Last()))
                Add(item);
        }

        public new IEnumerator<LocaleTextItem> GetEnumerator()
        {
            foreach (LocaleTextItem item in _items)
                if (item == null)
                    break;
                else
                    yield return item;
        }

        public new bool Remove(LocaleTextItem item)
        {
            if (item != null && _items.Contains(item))
            {
                _items = _items.Where(_ => _.Equals(item)).ToArray();
                return true;
            }
            return false;
        }
        //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }

    public class LocaleTextItem : IComparable<LocaleTextItem>
    {
        public LocaleTextItem() { }

        /// <summary>
        /// Don't inherits/implement IEntity: mongodb mapping conflict between Entity and HashSet of Entity 
        /// </summary>
        [Required]
        public string Id { get; set; }
        public string Text { get; set; }

        public int CompareTo([AllowNull] LocaleTextItem other)
        {
            return this.Id.CompareTo(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return ((LocaleTextItem)obj).Id.Equals(this.Id);
        }
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }

    public class LocaleTextItemComparer : IEqualityComparer<LocaleTextItem>
    {
        public bool Equals([AllowNull] LocaleTextItem x, [AllowNull] LocaleTextItem y)
        => x?.Id.Equals(y?.Id) == true;

        public int GetHashCode([DisallowNull] LocaleTextItem obj)
        => obj.Id.GetHashCode();
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
            writer.WriteValue(((IEnumerable<LocaleTextItem>)value).FirstOrDefault(_ => _.Id == _locale)?.Text);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value != null)
            {
                var locale = new LocaleText();
                locale.Add(new LocaleTextItem() { Id = _locale, Text = reader.Value.ToString() });
                return locale.ToArray();
                //return new List<LocaleText>() { new LocaleText() { Code = _language, Text = reader.Value.ToString() } };
            }
            else
                return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return !string.IsNullOrEmpty(_locale) && typeof(IEnumerable<LocaleTextItem>).IsAssignableFrom(objectType);
        }
    }
}
