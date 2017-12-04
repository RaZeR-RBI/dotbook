using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Linq;

namespace DotBook.Utils
{
    public class OrderedDictionary<K, V> : OrderedDictionary, IDictionary<K, V>,
        IReadOnlyDictionary<K, V>
    {
        public V this[K key]
        {
            get => (V)base[(K)key];
            set => base[(K)key] = value;
        }

        ICollection<K> IDictionary<K, V>.Keys
        {
            get
            {
                var result = new List<K>();
                foreach (var key in base.Keys)
                    result.Add((K)key);
                return result;
            }
        }

        IEnumerable<K> IReadOnlyDictionary<K, V>.Keys => Keys.Cast<K>();

        ICollection<V> IDictionary<K, V>.Values
        {
            get
            {
                var result = new List<V>();
                foreach (var value in base.Values)
                    result.Add((V)value);
                return result;
            }
        }

        IEnumerable<V> IReadOnlyDictionary<K, V>.Values => Values.Cast<V>();

        public OrderedDictionary() { }

        public OrderedDictionary(KeyValuePair<K,V>[] source)
        {
            foreach (var pair in source)
                Add(pair);
        }

        public void Add(K key, V value) => base.Add(key, value);

        public void Add(KeyValuePair<K, V> item) => base.Add(item.Key, item.Value);

        public bool Contains(KeyValuePair<K, V> item)
        {
            if (!base.Contains(item.Key)) return false;
            var value = this[item.Key];
            return item.Value.Equals(value);
        }

        public bool ContainsKey(K key) => base.Contains(key);

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) =>
            Enumerable.Zip(
                Keys.Cast<K>(),
                Values.Cast<V>(),
                (k, v) => KeyValuePair.Create(k, v))
                .ToArray();

        public bool Remove(K key)
        {
            var result = Contains(key);
            if (!result) return false;
            base.Remove(key);
            return true;
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            var result = Contains(item);
            if (!result) return false;
            base.Remove(item.Key);
            return true;
        }

        public bool TryGetValue(K key, out V value)
        {
            value = default(V);
            if (!ContainsKey(key)) return false;
            value = this[key];
            return true;
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
        {
            return Enumerable.Zip(
                Keys.Cast<K>(),
                Values.Cast<V>(),
                (k, v) => KeyValuePair.Create(k, v))
                .GetEnumerator();
        }
    }
}
