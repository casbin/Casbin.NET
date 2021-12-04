using System;
using System.Collections.Generic;
using System.Linq;
using Casbin.Caching;
using Casbin.Model;
using Casbin.Util;
using DynamicExpresso;

namespace Casbin.Evaluation;

internal class ExpressionHandler : IExpressionHandler
{
    private readonly IDictionary<Type, IExpressionCache> _cachePool = new Dictionary<Type, IExpressionCache>();
    private readonly FunctionMap _functionMap = FunctionMap.LoadFunctionMap();
    private readonly Interpreter _interpreter;

    public ExpressionHandler()
    {
        _interpreter = CreateInterpreter();
    }

    public IDictionary<string, Parameter> Parameters { get; }
        = new Dictionary<string, Parameter>();

    public void SetFunction(string name, Delegate function)
    {
        ClearCache();
        _interpreter.SetFunction(name, function);
    }

    public bool Invoke(in EnforceContext context, ref EnforceSession session)
    {
        if (CheckRequestValuesOnlyString(session.RequestValues))
        {
            return InvokeOnlyString(in context, ref session);
        }

        Parameter[] parameters = GetParameters(in context, ref session);
        object expressionResult = GetLambda(in context, ref session).Invoke(parameters);
        return expressionResult is true;
    }
    
    private Lambda GetLambda(in EnforceContext context, ref EnforceSession session)
    {
        string expressionString = session.ExpressionString;
        Type type = typeof(Lambda);
        if (_cachePool.TryGetValue(type, out IExpressionCache cache) is false)
        {
            cache = new ExpressionCache();
            _cachePool[type] = cache;
        }

        var cacheImpl = (ExpressionCache)cache;
        if (cacheImpl.TryGet(expressionString, out Lambda func))
        {
            return func;
        }

        Lambda lambda = CreateExpression(in context, ref session);
        cacheImpl.Set(expressionString, lambda);
        return lambda;
    }

    private TFunc GetFunc<TFunc>(in EnforceContext context, ref EnforceSession session) where TFunc : Delegate
    {
        string expressionString = session.ExpressionString;
        Type type = typeof(TFunc);
        if (_cachePool.TryGetValue(type, out IExpressionCache cache) is false)
        {
            cache = new ExpressionCache<TFunc>();
            _cachePool[type] = cache;
        }

        var cacheImpl = (IExpressionCache<TFunc>)cache;
        if (cacheImpl.TryGet(expressionString, out TFunc func))
        {
            return func;
        }

        Lambda expression = CreateExpression(in context, ref session);
        func = expression.Compile<TFunc>();
        cacheImpl.Set(expressionString, func);
        return func;
    }

    private void ClearCache()
    {
        foreach (IExpressionCache cache in _cachePool.Values)
        {
            cache?.Clear();
        }
    }

    private Lambda CreateExpression(in EnforceContext context, ref EnforceSession session)
    {
        Parameter[] parameters = GetParameters(in context, ref session);
        return _interpreter.Parse(session.ExpressionString, parameters);
    }

    private static Parameter[] GetParameters(in EnforceContext context, ref EnforceSession session)
    {
        IReadOnlyDictionary<string, int> requestTokens = context.View.RequestAssertion.Tokens;
        IReadOnlyList<string> requestTokenKeys = context.View.RequestTokens;
        int requestCount = requestTokenKeys.Count;

        IReadOnlyDictionary<string, int> policyTokens = context.View.PolicyAssertion.Tokens;
        IReadOnlyList<string> policyTokenKeys = context.View.PolicyTokens;
        int policyCount = policyTokenKeys.Count;

        var parameters = new Parameter[requestCount + policyCount];

        IReadOnlyList<object> requestValues = session.RequestValues;
        foreach (string token in requestTokenKeys)
        {
            int index = requestTokens[token];
            parameters[index] = new Parameter(token,  requestValues[index]);
        }

        IReadOnlyList<string> policyValues = session.PolicyValues;
        foreach (string token in policyTokenKeys)
        {
            int index = policyTokens[token];
            parameters[index + requestCount] = new Parameter(token,  policyValues[index]);
        }
        return parameters;
    }

    private Interpreter CreateInterpreter()
    {
        var interpreter = new Interpreter();
        // TODO: Auto deconstruct can not support .NET 4.5.2
        foreach (KeyValuePair<string, Delegate> functionKeyValue in _functionMap.FunctionDict)
        {
            interpreter.SetFunction(functionKeyValue.Key, functionKeyValue.Value);
        }
        return interpreter;
    }

    private static bool CheckRequestValuesOnlyString(IEnumerable<object> requestValues)
    {
        return requestValues.All(value => value is string);
    }

    #region Generic
    public bool InvokeOnlyString(in EnforceContext context, ref EnforceSession session)
    {
        bool result;
        int policyTokenCount = context.View.PolicyTokens.Count;
        int requestTokenCount = context.View.RequestTokens.Count;
        var requestValues = session.RequestValues;
        var policyValues = session.PolicyValues; 

        if (requestTokenCount is 0)
        {
            result = policyTokenCount switch
            {
                0 => GetFunc<Func<bool>>(in context, ref session)(),

                1 => GetFunc<Func<string, bool>>(in context, ref session)
                    (policyValues[0]),

                2 => GetFunc<Func<string, string, bool>>(in context, ref session)
                    (policyValues[0], policyValues[1]),

                3 => GetFunc<Func<string, string, string, bool>>(in context, ref session)
                    (policyValues[0], policyValues[1], policyValues[2]),

                4 => GetFunc<Func<string, string, string, string, bool>>(in context, ref session)
                    (policyValues[0], policyValues[1], policyValues[2], policyValues[3]),

                5 => GetFunc<Func<string, string, string, string, string, bool>>(in context, ref session)
                    (policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4]),

                6 => GetFunc<Func<string, string, string, string, string, string, bool>>(in context, ref session)
                    (policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4], policyValues[5]),

                _ => Invoke(in context, ref session)
            };
        }
        else if (requestTokenCount is 1)
        {
            result = policyTokenCount switch
            {
                0 => GetFunc<Func<string, bool>>(in context, ref session)
                    (requestValues[0] as string),

                1 => GetFunc<Func<string, string, bool>>(in context, ref session)
                    (requestValues[0] as string,
                    policyValues[0]),

                2 => GetFunc<Func<string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string,
                    policyValues[0], policyValues[1]),

                3 => GetFunc<Func<string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string,
                    policyValues[0], policyValues[1], policyValues[2]),

                4 => GetFunc<Func<string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3]),

                5 => GetFunc<Func<string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4]),

                6 => GetFunc<Func<string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4], policyValues[5]),

                _ => Invoke(in context, ref session)
            };
        }
        else if(requestTokenCount is 2)
        {
            result = policyTokenCount switch
            {
                0 => GetFunc<Func<string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string),

                1 => GetFunc<Func<string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string,
                    policyValues[0]),

                2 => GetFunc<Func<string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string,
                    policyValues[0], policyValues[1]),

                3 => GetFunc<Func<string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string,
                    policyValues[0], policyValues[1], policyValues[2]),

                4 => GetFunc<Func<string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3]),

                5 => GetFunc<Func<string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4]),

                6 => GetFunc<Func<string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4], policyValues[5]),

                _ => Invoke(in context, ref session)
            };
        }
        else if (requestTokenCount is 3)
        {
            result = policyTokenCount switch
            {
                0 => GetFunc<Func<string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string),

                1 => GetFunc<Func<string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string,
                    policyValues[0]),

                2 => GetFunc<Func<string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string,
                    policyValues[0], policyValues[1]),

                3 => GetFunc<Func<string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string,
                    policyValues[0], policyValues[1], policyValues[2]),

                4 => GetFunc<Func<string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3]),

                5 => GetFunc<Func<string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4]),

                6 => GetFunc<Func<string, string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4], policyValues[5]),

                _ => Invoke(in context, ref session)
            };
        }
        else if (requestTokenCount is 4)
        {
            result = policyTokenCount switch
            {
                0 => GetFunc<Func<string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string, requestValues[3] as string),

                1 => GetFunc<Func<string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string, requestValues[3] as string,
                    policyValues[0]),

                2 => GetFunc<Func<string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string, requestValues[3] as string,
                    policyValues[0], policyValues[1]),

                3 => GetFunc<Func<string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string, requestValues[3] as string,
                    policyValues[0], policyValues[1], policyValues[2]),

                4 => GetFunc<Func<string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string, requestValues[3] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3]),

                5 => GetFunc<Func<string, string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string, requestValues[3] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4]),

                6 => GetFunc<Func<string, string, string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string, requestValues[3] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4], policyValues[5]),

                _ => Invoke(in context, ref session)
            };
        }
        else if (requestTokenCount is 5)
        {
            result = policyTokenCount switch
            {
                0 => GetFunc<Func<string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string, requestValues[3] as string, requestValues[4] as string),

                1 => GetFunc<Func<string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string, requestValues[3] as string, requestValues[4] as string,
                    policyValues[0]),

                2 => GetFunc<Func<string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string, requestValues[3] as string, requestValues[4] as string,
                    policyValues[0], policyValues[1]),

                3 => GetFunc<Func<string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string, requestValues[3] as string, requestValues[4] as string,
                    policyValues[0], policyValues[1], policyValues[2]),

                4 => GetFunc<Func<string, string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string, requestValues[3] as string, requestValues[4] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3]),

                5 => GetFunc<Func<string, string, string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string, requestValues[3] as string, requestValues[4] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4]),

                6 => GetFunc<Func<string, string, string, string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[1] as string, requestValues[2] as string, requestValues[3] as string, requestValues[4] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4], policyValues[5]),

                _ => Invoke(in context, ref session)
            };
        }
        else if (requestTokenCount is 6)
        {
            result = policyTokenCount switch
            {
                0 => GetFunc<Func<string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[0] as string, requestValues[2] as string, requestValues[3] as string, requestValues[4] as string, requestValues[5] as string),

                1 => GetFunc<Func<string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[0] as string, requestValues[2] as string, requestValues[3] as string, requestValues[4] as string, requestValues[5] as string,
                    policyValues[0]),

                2 => GetFunc<Func<string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[0] as string, requestValues[2] as string, requestValues[3] as string, requestValues[4] as string, requestValues[5] as string,
                    policyValues[0], policyValues[1]),

                3 => GetFunc<Func<string, string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[0] as string, requestValues[2] as string, requestValues[3] as string, requestValues[4] as string, requestValues[5] as string,
                    policyValues[0], policyValues[1], policyValues[2]),

                4 => GetFunc<Func<string, string, string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[0] as string, requestValues[2] as string, requestValues[3] as string, requestValues[4] as string, requestValues[5] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3]),

                5 => GetFunc<Func<string, string, string, string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[0] as string, requestValues[2] as string, requestValues[3] as string, requestValues[4] as string, requestValues[5] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4]),

                6 => GetFunc<Func<string, string, string, string, string, string, string, string, string, string, string, string, bool>>(in context, ref session)
                    (requestValues[0] as string, requestValues[0] as string, requestValues[2] as string, requestValues[3] as string, requestValues[4] as string, requestValues[5] as string,
                    policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4], policyValues[5]),

                _ => Invoke(in context, ref session)
            };
        }
        else
        {
            result = Invoke(in context, ref session);
        }

        return result;
    }
    #endregion
}
