using System;
using System.Collections.Concurrent;

namespace Casbin.Caching;

internal class ExpressionCache<TFunc> : IExpressionCache<TFunc> where TFunc : Delegate
{
    private readonly ConcurrentDictionary<string, TFunc> _cache = new();

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
