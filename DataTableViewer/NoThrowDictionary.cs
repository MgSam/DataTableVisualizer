using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTableViewer
{
    /// <summary>
    /// A thin wrapper over a dictionary which looks up keys without throwing if the key is missing.
    /// This is necessary because the compiled OData expressions always look for indexers.
    /// </summary>
    /// <typeparam name="K">Type of keys.</typeparam>
    /// <typeparam name="V">Type of values.</typeparam>
    internal class NoThrowDictionary<K, V>
    {
        private readonly Dictionary<K, V> _dict;

        public NoThrowDictionary(Dictionary<K, V> dict)
        {
            _dict = dict;
        }

        public V this[K key]
        {
            get
            {
                var success = _dict.TryGetValue(key, out var value);
                return success ? value : default(V);
            }
            set => _dict[key] = value;
        }
    }
}
