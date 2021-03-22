#if !NET45
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NetCasbin.Abstractions;

namespace NetCasbin.Caching
{
    public class ReaderWriterEnforceCache : IEnforceCache<ReaderWriterEnforceCacheOptions>
    {
        private readonly ReaderWriterLockSlim _lockSlim = new();
        private Dictionary<string, bool> _memoryCache = new();

        public ReaderWriterEnforceCache(IOptions<ReaderWriterEnforceCacheOptions> options)
        {
            if (options?.Value is not null)
            {
                CacheOptions = options.Value;
            }
        }

        public ReaderWriterEnforceCacheOptions CacheOptions { get; } = new();

        public bool TryGetResult(IReadOnlyList<object> requestValues, string key, out bool result)
        {
            if (requestValues is null)
            {
                throw new ArgumentNullException(nameof(requestValues));
            }

            if (_lockSlim.TryEnterReadLock(CacheOptions.WaitTimeOut) is false)
            {
                result = false;
                return false;
            }

            try
            {
                return _memoryCache.TryGetValue(key, out result);
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }
        }

        public Task<bool?> TryGetResultAsync(IReadOnlyList<object> requestValues, string key)
        {
            return TryGetResult(requestValues, key, out bool result)
                ? Task.FromResult((bool?) result) : null;
        }

        public bool TrySetResult(IReadOnlyList<object> requestValues, string key, bool result)
        {
            if (requestValues is null)
            {
                throw new ArgumentNullException(nameof(requestValues));
            }

            if (_lockSlim.TryEnterWriteLock(CacheOptions.WaitTimeOut) is false)
            {
                return false;
            }

            try
            {
                _memoryCache[key] = result;
                return true;
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public Task<bool> TrySetResultAsync(IReadOnlyList<object> requestValues, string key, bool result)
        {
            return Task.FromResult(TrySetResult(requestValues, key, result));
        }

        public void Clear()
        {
            _memoryCache = new Dictionary<string, bool>();
        }

        public Task ClearAsync()
        {
            Clear();
            return Task.CompletedTask;
        }
    }
}
#endif
