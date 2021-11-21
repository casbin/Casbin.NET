using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Caching
{
    public class EnforceCache : IEnforceCache
    {
        private readonly ReaderWriterLockSlim _lockSlim = new();
        private Dictionary<string, bool> _memoryCache = new();

        public EnforceCache(EnforceCacheOptions options)
        {
            if (options is not null)
            {
                CacheOptions = options;
            }
        }

        public EnforceCacheOptions CacheOptions { get; } = new();

        public bool TryGetResult<TRequest>(in TRequest requestValues, out bool result)
            where TRequest : IRequestValues
        {
            if (requestValues is null)
            {
                throw new ArgumentNullException(nameof(requestValues));
            }

            if (Request.TryGetStringKey(requestValues, out string key) is false)
            {
                result = false;
                return false;
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

        public Task<bool?> TryGetResultAsync<TRequest>(in TRequest requestValues)
            where TRequest : IRequestValues
        {
            return TryGetResult(requestValues, out bool result)
                ? Task.FromResult((bool?) result) : Task.FromResult((bool?) null);
        }

        public bool TrySetResult<TRequest>(in TRequest requestValues, bool result)
            where TRequest : IRequestValues
        {
            if (requestValues is null)
            {
                throw new ArgumentNullException(nameof(requestValues));
            }

            if (Request.TryGetStringKey(requestValues, out string key) is false)
            {
                return false;
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

        public Task<bool> TrySetResultAsync<TRequest>(in TRequest requestValues, bool result)
            where TRequest : IRequestValues
        {
            return Task.FromResult(TrySetResult(requestValues, result));
        }

        public void Clear()
        {
            _memoryCache = new Dictionary<string, bool>();
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
