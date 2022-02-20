using System;

namespace Casbin.Caching;

public interface IExpressionCachePool
{
    public void SetFunc<TFunc>(string expression, TFunc func) where TFunc : Delegate;

    public bool TryGetFunc<TFunc>(string expression, out TFunc func) where TFunc : Delegate;

    public void Clear();
}
