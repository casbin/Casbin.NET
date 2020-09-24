using System.Collections.Generic;
using DynamicExpresso;
using NetCasbin.Model;

namespace NetCasbin.Abstractions
{
    internal interface IExpressionProvider
    {
        public Assertion RequestAssertion { get; }

        public Assertion PolicyAssertion { get; }

        public void SetFunction(string name, AbstractFunction function);

        public void SetGFunctions();

        public Lambda GetExpression(string expressionString, IReadOnlyList<object> requestValues);

        public IDictionary<string, Parameter> AddOrUpdateRequestParameters(IReadOnlyList<object> requestValues);

        public IDictionary<string, Parameter> AddOrUpdatePolicyParameters(IReadOnlyList<string> policyValues);
    }
}
