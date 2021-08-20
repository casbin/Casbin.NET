using System;
using System.Collections.Generic;
using DynamicExpresso;

namespace NetCasbin.Evaluation
{
    internal interface IExpressionHandler
    {
        public IReadOnlyDictionary<string, int> RequestTokens { get; }

        public IReadOnlyDictionary<string, int> PolicyTokens { get; }

        public IDictionary<string, Parameter> Parameters { get; } 

        public void SetFunction(string name, Delegate function);

        public void SetGFunctions();

        public bool Invoke(string expressionString);

        public void SetRequest(IReadOnlyList<object> requestValues);

        public void SetPolicy(IReadOnlyList<string> policyValues);
    }
}
