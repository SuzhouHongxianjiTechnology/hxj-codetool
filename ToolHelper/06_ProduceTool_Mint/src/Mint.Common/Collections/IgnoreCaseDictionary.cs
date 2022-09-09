namespace Mint.Common.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Mint.Common.Extensions;

    public class IgnoreCaseDictionary<TValue> : Dictionary<string, TValue>
    {
        public IgnoreCaseDictionary() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public IgnoreCaseDictionary(IDictionary<string, TValue> dictionary) : base(dictionary, StringComparer.OrdinalIgnoreCase)
        {
        }

        public IgnoreCaseDictionary(IEnumerable<KeyValuePair<string, TValue>> collection) : base(collection, StringComparer.OrdinalIgnoreCase)
        {
        }

        public bool TryGetKey(string key, [MaybeNullWhen(false)] out string actualKey)
        {
            actualKey = this.Keys.Where(k => k.EqualsIgnoreCase(key)).FirstOrDefault();
            return actualKey != null;
        }

        public bool TryRemove(string key, [MaybeNullWhen(false)] out TValue value)
        {
            value = default;
            if (this.ContainsKey(key))
            {
                this.TryGetValue(key, out value);
                this.Remove(key);
            }
            return value != null;
        }

        public IgnoreCaseDictionary<TValue> FilterKey(Func<string, bool> condition)
        {
            var newDict = new IgnoreCaseDictionary<TValue>();
            var pairs = this.Where(n => condition(n.Key));
            foreach (var pair in pairs)
            {
                newDict[pair.Key] = pair.Value;
            }
            return newDict;
        }
    }
}
