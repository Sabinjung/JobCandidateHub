using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace JobCandidateHub.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T? GetFromCache<T>(string key)
        {
            return _memoryCache.TryGetValue<T>(key, out var cachedItem) ? cachedItem : default;
        }

    
        public void SetInCache<T>(string key, T item, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            var cacheOptions = new MemoryCacheEntryOptions();

            if (absoluteExpirationRelativeToNow.HasValue)
            {
                cacheOptions.SetAbsoluteExpiration(absoluteExpirationRelativeToNow.Value);
            }

            _memoryCache.Set(key, item, cacheOptions);
        }

        public void RemoveFromCache(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
