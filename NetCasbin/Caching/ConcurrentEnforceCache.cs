using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetCasbin.Abstractions;

namespace NetCasbin.Caching
{
    public class ConcurrentEnforceCache : IEnforceCache
    {
        private readonly ConcurrentDictionary<string, bool> _memoryCache = new();

        public bool TryGetResult(IReadOnlyList<object> requestValues, string key, out bool result)
        {
            return _memoryCache.TryGetValue(key, out result);
        }

        public Task<bool?> TryGetResultAsync(IReadOnlyList<object> requestValues, string key)
        {
            return TryGetResult(requestValues, key, out bool result)
                ? Task.FromResult((bool?)result)
                : Task.FromResult((bool?)null);
        }

        public bool TrySetResult(IReadOnlyList<object> requestValues, string key, bool result)
        {
            _memoryCache[key] = result;
            return true;
        }

        public Task<bool> TrySetResultAsync(IReadOnlyList<object> requestValues, string key, bool result)
        {
            return Task.FromResult(TrySetResult(requestValues, key, result));
        }

        public void Clear()
        {
            _memoryCache.Clear();
        }

#if !NET452
        public Task ClearAsync()
        {
            Clear();
            return Task.CompletedTask;
        }
#else
        public Task ClearAsync()
        {
            Clear();
            return Task.FromResult(false);
        }
#endif
    }
}
