using System;
using System.Collections.Generic;
using System.Threading;
using Casbin.Caching;
using Casbin.Model;
using DynamicExpresso;

namespace Casbin.Evaluation;

internal class ExpressionHandler : IExpressionHandler
{
    private readonly FunctionMap _functionMap = FunctionMap.LoadFunctionMap();
    private IExpressionCachePool _cachePool = new ExpressionCachePool();
#if !NET452
    private readonly Interpreter _interpreter;
#else
    private Interpreter _interpreter;
#endif

    public ExpressionHandler()
    {
        _interpreter = CreateInterpreter();
    }

    public IDictionary<string, Parameter> Parameters { get; }
        = new Dictionary<string, Parameter>();

    public void SetFunction(string name, Delegate function)
    {
#if !NET452
        _interpreter.SetFunction(name, function);
#else
        List<Identifier> identifiers = new();
        bool exist = false;
        foreach (var identifier in _interpreter.Identifiers)
        {
            if (identifier.Name == name)
            {
                exist = true;
                continue;
            }
            identifiers.Add(identifier);
        }

        if (exist is false)
        {
            _interpreter.SetFunction(name, function);
            return;
        }

        var interpreter = new Interpreter();
        interpreter.SetIdentifiers(identifiers);
        interpreter.SetFunction(name, function);
        Interlocked.Exchange(ref _interpreter, interpreter);
#endif
        ExpressionCachePool cachePool = new ExpressionCachePool();
        Interlocked.Exchange(ref _cachePool, cachePool);
    }

    public bool Invoke<TRequest, TPolicy>(in EnforceContext context, string expressionString, in TRequest request,
        in TPolicy policy)
        where TRequest : IRequestValues
        where TPolicy : IPolicyValues
    {
        if (context.View.SupportGeneric is false)
        {
            if (_cachePool.TryGetFunc<Func<IRequestValues, IPolicyValues, bool>>(expressionString,
                    out Func<IRequestValues, IPolicyValues, bool> func))
            {
                return func(request, policy);
            }

            func = CompileExpression<IRequestValues, IPolicyValues>(in context, expressionString);
            _cachePool.SetFunc(expressionString, func);
            return func(request, policy);
        }

        if (_cachePool.TryGetFunc<Func<TRequest, TPolicy, bool>>(expressionString,
                out Func<TRequest, TPolicy, bool> genericFunc) is not false)
        {
            return genericFunc(request, policy);
        }

        genericFunc = CompileExpression<TRequest, TPolicy>(in context, expressionString);
        _cachePool.SetFunc(expressionString, genericFunc);
        return genericFunc(request, policy);
    }

    private Func<TRequest, TPolicy, bool> CompileExpression<TRequest, TPolicy>(in EnforceContext context,
        string expressionString)
        where TRequest : IRequestValues
        where TPolicy : IPolicyValues
    {
        return _interpreter.ParseAsDelegate<Func<TRequest, TPolicy, bool>>(expressionString,
            context.View.RequestType, context.View.PolicyType);
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
}
