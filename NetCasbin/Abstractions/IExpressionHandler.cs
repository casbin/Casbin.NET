using System;
using System.Collections.Generic;
using DynamicExpresso;

namespace NetCasbin.Abstractions
{
    internal interface IExpressionHandler
    {
        public IDictionary<string, int> RequestTokens { get; }

        public IDictionary<string, int> PolicyTokens { get; }

        public IDictionary<string, Parameter> Parameters { get; } 

        public void SetFunction(string name, Delegate function);

        public void SetGFunctions();

        public void EnsureCreated(string expressionString, IReadOnlyList<object> requestValues);

        public bool Invoke(string expressionString, IReadOnlyList<object> requestValues);

        public void SetRequestParameters(IReadOnlyList<object> requestValues);

        public void SetPolicyParameters(IReadOnlyList<string> policyValues);
    }
}
