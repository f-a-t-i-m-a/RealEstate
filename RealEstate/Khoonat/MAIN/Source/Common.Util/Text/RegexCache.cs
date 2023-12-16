using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Linq;
using JahanJooy.Common.Util.Collections;

namespace JahanJooy.Common.Util.Text
{
    public class RegexCache
    {
        private LRUCache<string, Regex> cache;

        public RegexCache(int size)
        {
            cache = new LRUCache<string, Regex>(size);
        }

        public Regex getPatternForRegex(string regex)
        {
            Regex pattern = cache.get(regex);
            if (pattern == null)
            {
                pattern = new Regex(regex);
                cache.put(regex, pattern);
            }
            return pattern;
        }

        // This method is used for testing.
        public bool containsRegex(string regex)
        {
            return cache.containsKey(regex);
        }

        private class LRUCache<K, V>
        {
            // LinkedHashMap offers a straightforward implementation of LRU cache.
            private readonly LRUDictionary<K, V> _map;
            private readonly int _size;

            public LRUCache(int size)
            {
                _size = size;
                _map = new LRUDictionary<K, V>();
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public V get(K key)
            {
                V result;
                if (!_map.TryGetValue(key, out result))
                    return default(V);

                return result;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public void put(K key, V value)
            {
                _map[key] = value;
                if (_map.Count > _size)
                {
                    var first = _map.Keys.First();
                    _map.Remove(first);
                }
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public bool containsKey(K key)
            {
                return _map.ContainsKey(key);
            }
        }
    }
}