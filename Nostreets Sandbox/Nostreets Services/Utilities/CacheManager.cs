using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Utilities
{

    public static class CacheManager
    {
        private static MemoryCache _instance = MemoryCache.Default;

        public static bool Contains(string key)
        {
            return (_instance.Contains(key)) ? true : false;
        }

        public static T GetItem<T>(string key)
        {
            T result = default(T);

            if (Contains(key))
            {
                result = (T)_instance.Get(key);
            }

            return result;

        }

        public static void InsertItem(string key, object data, DateTimeOffset? timespan = null)
        {
            DateTimeOffset offset = (timespan == null) ? DateTimeOffset.Now : timespan.Value;

            if (!Contains(key))
            {
                _instance.Add(key, data, new CacheItemPolicy { AbsoluteExpiration = offset });
            }
            else
            {
                _instance.Set(key, data, new CacheItemPolicy { AbsoluteExpiration = offset });
            }

        }

        public static void DeleteItem(string key)
        {
            if (Contains(key))
            {
                _instance.Remove(key); 
            }

        }

    }
}
