using Microsoft.Extensions.Caching.Memory;
using NorthwindApp.Business.Services.Abstract;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace NorthwindApp.Business.Services.Concrete
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<string, byte> _keys = new();

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T? Get<T>(string key)
        {
            _cache.TryGetValue(key, out T? value);
            return value;
        }

        public void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (absoluteExpiration.HasValue)
                options.SetAbsoluteExpiration(absoluteExpiration.Value);

            _cache.Set(key, value, options);
            _keys.TryAdd(key, 0);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _keys.TryRemove(key, out _);
        }

        public void RemoveByPrefix(string prefix)
        {
            var keysToRemove = _keys.Keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                _keys.TryRemove(key, out _);
            }
        }
    }
}