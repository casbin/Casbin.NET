using System;
using Casbin.Model;
using DynamicExpresso;

namespace Casbin.UnitTests.Util;

public static class ExpressionUtil
{
    public static Func<TRequest, TPolicy, bool> Compile<TRequest, TPolicy>
    (
        Interpreter interpreter, string expressionText,
        string requestType, in TRequest r,
        string policyType, in TPolicy p)
        where TRequest : struct, IRequestValues where TPolicy : IPolicyValues
    {
        return interpreter.ParseAsDelegate<Func<TRequest, TPolicy, bool>>(
            expressionText, requestType, policyType
        );
    }
}
