using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCasbin.Abstractions
{
    public interface IEnforceCache<out TOptions> : IEnforceCache
    {
        public TOptions CacheOptions { get; }
    }

    public interface IEnforceCache
    {
        public bool TryGetResult(IReadOnlyList<object> requestValues, string key, out bool result);

        public Task<bool?> TryGetResultAsync(IReadOnlyList<object> requestValues, string key);

        public bool TrySetResult(IReadOnlyList<object> requestValues, string key, bool result);

        public Task<bool> TrySetResultAsync(IReadOnlyList<object> requestValues, string key, bool result);

        public void Clear();

        public Task ClearAsync();
    }
}
