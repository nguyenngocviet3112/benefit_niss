using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TimorINSSBackEnd.Cache
{
    public class CacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;
        private List<string> _keyList;

        public CacheProvider()
        {
            //max limit set up for units of size (defined on set of key)
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 100000
            });
            _keyList = new List<string>();
        }

        public T GetFromCache<T>(string key) where T : class
        {
            _cache.TryGetValue(key, out T cachedResponse);
            return cachedResponse as T;
        }

        public void SetCache<T>(string key, T value, long size) where T : class
        {
            DateTime now = DateTime.Now;
            DateTime tomorrow = now.AddDays(1).Date;
            TimeSpan timer = TimeSpan.FromTicks((tomorrow - now).Ticks);

            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(timer)
                .SetAbsoluteExpiration(timer)
                .SetSize(size);
            _cache.Set(key, value, options);

            if (!_keyList.Any(s => s == key))
                _keyList.Add(key);
        }

        public void ClearCache(string key)
        {
            _keyList.Remove(key);
            _cache.Remove(key);
        }

        public void Reset()
        {
            while (_keyList.Count > 0)
            {
                this.ClearCache(_keyList.First());
            }
        }
    }
}