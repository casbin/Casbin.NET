using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Casbin.Caching;

public class ExpressionCachePool : IExpressionCachePool
{
    private ConcurrentDictionary<Type, IExpressionCache> _cachePool = new();

    public void SetFunc<TFunc>(string expression, TFunc func) where TFunc : Delegate
    {
        Type type = typeof(TFunc);
        if (_cachePool.TryGetValue(type, out IExpressionCache cache) is false)
        {
            cache = new ExpressionCache<TFunc>();
            _cachePool[type] = cache;
        }

        var cacheImpl = (IExpressionCache<TFunc>)cache;
        cacheImpl.Set(expression, func);
    }

    public bool TryGetFunc<TFunc>(string expression, out TFunc func) where TFunc : Delegate
    {
        Type type = typeof(TFunc);
        if (_cachePool.TryGetValue(type, out IExpressionCache cache) is false)
        {
            cache = new ExpressionCache<TFunc>();
            _cachePool[type] = cache;
        }

        var cacheImpl = (IExpressionCache<TFunc>)cache;
        return cacheImpl.TryGet(expression, out func);
    }

    public void Clear()
    {
        ConcurrentDictionary<Type, IExpressionCache> cachePool = new ConcurrentDictionary<Type, IExpressionCache>();
        Interlocked.Exchange(ref _cachePool, cachePool);
    }
}
