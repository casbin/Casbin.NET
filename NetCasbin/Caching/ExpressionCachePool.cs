using System;
using System.Collections.Generic;
using DynamicExpresso;

namespace Casbin.Caching;

public class ExpressionCachePool : IExpressionCachePool
{
    private readonly Dictionary<Type, IExpressionCache> _cachePool = new();

    public void SetLambda(string expression, Lambda lambda)
    {
        Type type = typeof(Lambda);
        if (_cachePool.TryGetValue(type, out IExpressionCache cache) is false)
        {
            cache = new ExpressionCache();
            _cachePool[type] = cache;
        }
        var cacheImpl = (ExpressionCache) cache;
        cacheImpl.Set(expression, lambda);
    }

    public bool TryGetLambda(string expression, out Lambda lambda)
    {
        Type type = typeof(Lambda);
        if (_cachePool.TryGetValue(type, out IExpressionCache cache) is false)
        {
            lambda = default;
            return false;
        }
        var cacheImpl = (ExpressionCache) cache;
        return cacheImpl.TryGet(expression, out lambda);
    }

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
        foreach (IExpressionCache cache in _cachePool.Values)
        {
            cache?.Clear();
        }
    }
}
