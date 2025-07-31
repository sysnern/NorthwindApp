using Microsoft.Extensions.Caching.Memory;
using NorthwindApp.Business.Services.Abstract;
using System.Collections.Concurrent;

namespace NorthwindApp.Business.Services.Concrete
{
    /// <summary>
    /// Enhanced memory cache service with improved cache management
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<string, byte> _keys = new();
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

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
            
            var expiration = absoluteExpiration ?? DefaultExpiration;
            options.SetAbsoluteExpiration(expiration);
            
            // Add callback to remove key from tracking when expired
            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                if (key is string keyString)
                {
                    _keys.TryRemove(keyString, out _);
                }
            });

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
        
        public void Clear()
        {
            foreach (var key in _keys.Keys.ToList())
            {
                _cache.Remove(key);
                _keys.TryRemove(key, out _);
            }
        }
        
        public bool Exists(string key)
        {
            return _cache.TryGetValue(key, out _);
        }
        
        public IEnumerable<string> GetAllKeys()
        {
            return _keys.Keys.ToList();
        }
    }
}