using System;
using System.Collections.Generic;
using DynamicExpresso;

namespace Casbin.Evaluation
{
    public interface IExpressionHandler
    {
        public EnforceContext EnforceContext { get; }

        public IDictionary<string, Parameter> Parameters { get; }

        public void SetEnforceContext(in EnforceContext context);

        public void SetFunction(string name, Delegate function);

        public void SetGFunctions();

        public void EnsureCreated(string expressionString, IReadOnlyList<object> requestValues);

        public bool Invoke(string expressionString, IReadOnlyList<object> requestValues);

        public void SetRequestParameters(IReadOnlyList<object> requestValues);

        public void SetPolicyParameters(IReadOnlyList<string> policyValues);
    }
}
