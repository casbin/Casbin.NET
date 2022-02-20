using System;
using System.Collections.Generic;
using DynamicExpresso;

namespace Casbin.Caching;

internal class ExpressionCache : IExpressionCache<Lambda>
{
    private readonly Dictionary<string, Lambda> _cache = new();

    public bool TryGet(string expressionString, out Lambda lambda)
    {
        return _cache.TryGetValue(expressionString, out lambda);
    }

    public void Set(string expressionString, Lambda lambda)
    {
        _cache[expressionString] = lambda;
    }

    public void Clear() => _cache.Clear();
}

internal class ExpressionCache<TFunc> : IExpressionCache<TFunc> where TFunc : Delegate
{
    private readonly Dictionary<string, TFunc> _cache = new();

    public bool TryGet(string expressionString, out TFunc func)
    {
        return _cache.TryGetValue(expressionString, out func);
    }

    public void Set(string expressionString, TFunc func)
    {
        _cache[expressionString] = func;
    }

    public void Clear() => _cache.Clear();
}
