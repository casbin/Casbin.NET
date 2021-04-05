using System.Collections.Generic;
using NetCasbin.Abstractions;

namespace NetCasbin.Extensions
{
    internal static class ExpressionHandlerExtension
    {
        internal static IExpressionHandler SetRequest(this IExpressionHandler handler, params object[] requestValues)
        {
            handler.RequestValues = requestValues;
            return handler;
        }

        internal static  IExpressionHandler SetRequest(this IExpressionHandler handler, IReadOnlyList<object> requestValues)
        {
            handler.RequestValues = requestValues;
            return handler;
        }

        internal static IExpressionHandler SetPolicy(this IExpressionHandler handler, IReadOnlyList<string> policyValues)
        {
            handler.PolicyValues = policyValues;
            return handler;
        }

        internal static bool TryGetPolicyValue(this IExpressionHandler handler, string tokenName, out string value)
        {
            if (handler.PolicyTokens.TryGetValue(tokenName, out int index) is false)
            {
                value = null;
                return false;
            }

            value = handler.PolicyValues[index];
            return true;
        }
    }
}
