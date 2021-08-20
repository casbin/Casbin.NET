using System;
using System.Collections.Generic;
using DynamicExpresso;

namespace NetCasbin.Evaluation
{
    internal class ExpressionCache : IExpressionCache<Lambda>
    {
        private readonly Lazy<Dictionary<string, Lambda>> _cache = new();

        public bool TryGet(string expressionString, out Lambda lambda)
        {
            return _cache.Value.TryGetValue(expressionString, out lambda);
        }

        public void Set(string expressionString, Lambda lambda)
        {
            _cache.Value[expressionString] = lambda;
        }

        public void Clear() => _cache.Value.Clear();
    }

    internal class ExpressionCache<TFunc> : IExpressionCache<TFunc> where TFunc : Delegate
    {
        private readonly Lazy<Dictionary<string, TFunc>> _cache = new();

        public bool TryGet(string expressionString, out TFunc func)
        {
            return _cache.Value.TryGetValue(expressionString, out func);
        }

        public void Set(string expressionString, TFunc func)
        {
            _cache.Value[expressionString] = func;
        }

        public void Clear() => _cache.Value.Clear();
    }
}
