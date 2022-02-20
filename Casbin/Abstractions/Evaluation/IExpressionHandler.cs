using System;
using Casbin.Caching;
using Casbin.Effect;
using Casbin.Model;

namespace Casbin.Evaluation;

public interface IExpressionHandler
{
    public IExpressionCachePool Cache { get; set; }

    public void SetFunction(string name, Delegate function);

    public bool Invoke<TRequest, TPolicy>(in EnforceContext context, string expressionString, in TRequest request, in TPolicy policy)
        where TRequest : IRequestValues
        where TPolicy : IPolicyValues;
}
