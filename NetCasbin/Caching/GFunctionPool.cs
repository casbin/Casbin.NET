using System.Collections.Generic;

namespace Casbin.Caching;

public class GFunctionCachePool : IGFunctionCachePool
{
    private readonly Dictionary<string, IGFunctionCache> _cachePool = new();

    public void Clear(string roleType)
    {
        if (_cachePool.TryGetValue(roleType, out IGFunctionCache cache))
        {
            cache.Clear();
        }
    }

    public IGFunctionCache GetCache(string roleType)
    {
        if (_cachePool.TryGetValue(roleType, out IGFunctionCache cache))
        {
            return cache;
        }

        cache = new GFunctionCache();
        _cachePool[roleType] = cache;
        return cache;
    }
}
