using System.Linq;

namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Adds or updates (if exists) a key value pair in the Dictionary.
        /// </summary>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <param name="dictionary">Current dictionary</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> pair)
        {
            if (dictionary.ContainsKey(pair.Key))
                dictionary[pair.Key] = pair.Value;
            else
                dictionary.Add(pair.Key, pair.Value);
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            if (pairs == null || !pairs.Any())
                return;

            foreach(var pair in pairs)
            {
                dictionary.AddOrUpdate(pair);
            }
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary.TryGetValue(key, out TValue value))
                return value;
            return defaultValue;
        }

        public static void Copy<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> otherDictionary)
        {
            if (otherDictionary == null || !otherDictionary.Any())
            {
                return;
            }
            foreach(var pair in otherDictionary)
            {
                dictionary.AddOrUpdate(pair);
            }
        }

        public static void Remove<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, List<TKey> keys)
        {
            foreach(var key in keys)
            {
                dictionary.Remove(key);
            }
        }
    }
}
