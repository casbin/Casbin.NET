using System;
using System.Collections.Generic;
using DynamicExpresso;

namespace Casbin
{
    public interface IExpressionHandler
    {
        public IReadOnlyDictionary<string, int> RequestTokens { get; }

        public IReadOnlyDictionary<string, int> PolicyTokens { get; }

        public IDictionary<string, Parameter> Parameters { get; } 

        public void SetFunction(string name, Delegate function);

        public void SetGFunctions();

        public void EnsureCreated(string expressionString, IReadOnlyList<object> requestValues);

        public bool Invoke(string expressionString, IReadOnlyList<object> requestValues);

        public void SetRequestParameters(IReadOnlyList<object> requestValues);

        public void SetPolicyParameters(IReadOnlyList<string> policyValues);
    }
}
