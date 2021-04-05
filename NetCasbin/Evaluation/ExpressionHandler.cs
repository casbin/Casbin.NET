using System;
using System.Collections.Generic;
using DynamicExpresso;
using NetCasbin.Abstractions;
using NetCasbin.Model;
using NetCasbin.Util;

namespace NetCasbin.Evaluation
{
    public class ExpressionHandler : IExpressionHandler
    {
        private readonly IDictionary<Type, IExpressionCache> _cachePool
            = new Dictionary<Type, IExpressionCache>();

        private readonly FunctionMap _functionMap = FunctionMap.LoadFunctionMap();
        private readonly Model.Model _model;
        private Interpreter _interpreter;
        private readonly Parameter[] _orderedParameters;

        public ExpressionHandler(Model.Model model,
            string requestType = PermConstants.DefaultRequestType,
            string policyType = PermConstants.DefaultPolicyType)
        {
            _model = model;
            int parametersCount = 0;

            if (model.Model.ContainsKey(PermConstants.Section.RequestSection))
            {
                RequestTokens = model.Model[PermConstants.Section.RequestSection][requestType].Tokens;
                parametersCount += RequestTokens.Count;
            }

            if (model.Model.ContainsKey(PermConstants.Section.PolicySection))
            {
                PolicyTokens = model.Model[PermConstants.Section.PolicySection][policyType].Tokens;
                parametersCount += PolicyTokens.Count;
            }

            _orderedParameters = new Parameter[parametersCount];
        }

        public IDictionary<string, int> RequestTokens { get; }

        public IDictionary<string, int> PolicyTokens { get; }

        public IReadOnlyList<object> RequestValues { get; set; }

        public IReadOnlyList<string> PolicyValues { get; set; }

        public IDictionary<string, Parameter> Parameters { get; }
            = new Dictionary<string, Parameter>();

        public void SetFunction(string name, Delegate function)
        {
            ClearCache();
            var interpreter = GetInterpreter();
            interpreter.SetFunction(name, function);
        }

        public void SetGFunctions()
        {
            ClearCache();
            var interpreter = GetInterpreter();
            SetGFunctions(interpreter);
        }

        public bool Invoke(string expressionString)
        {
            var expressionResult = GetLambda(expressionString).Invoke(GetParameters());
            return expressionResult is bool result && result;
        }

        public bool Invoke<T1>(string expressionString, T1 value1) => throw new NotImplementedException();
        public bool Invoke<T1, T2>(string expressionString, T1 value1, T2 value2) => throw new NotImplementedException();
        public bool Invoke<T1, T2, T3>(string expressionString, T1 value1, T2 value2, T3 value3)
        {
            return InternalInvoke(expressionString, PolicyTokens.Count, value1, value2, value3);
        }
        public bool Invoke<T1, T2, T3, T4>(string expressionString, T1 value1, T2 value2, T3 value3, T4 value4) => throw new NotImplementedException();
        public bool Invoke<T1, T2, T3, T4, T5>(string expressionString, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) => throw new NotImplementedException();

        private bool InternalInvoke<T1, T2, T3>(string expressionString, int policyTokenCount, T1 value1, T2 value2, T3 value3)
        {
            var policyValues = PolicyValues;
            return policyTokenCount switch
            {
                0 => GetFunc<Func<T1, T2, T3, bool>>(expressionString)
                    (value1, value2, value3),
                1 => GetFunc<Func<T1, T2, T3, string, bool>>(expressionString)
                    (value1, value2, value3, policyValues[0]),
                2 => GetFunc<Func<T1, T2, T3, string, string, bool>>(expressionString)
                    (value1, value2, value3, policyValues[0], policyValues[1]),
                3 => GetFunc<Func<T1, T2, T3, string, string, string, bool>>(expressionString)
                    (value1, value2, value3, policyValues[0], policyValues[1], policyValues[2]),
                4 => GetFunc<Func<T1, T2, T3, string, string, string, string, bool>>(expressionString)
                    (value1, value2, value3, policyValues[0], policyValues[1], policyValues[2], policyValues[3]),
                5 => GetFunc<Func<T1, T2, T3, string, string, string, string, string, bool>>(expressionString)
                    (value1, value2, value3, policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4]),
                6 => GetFunc<Func<T1, T2, T3, string, string, string, string, string, string, bool>>(expressionString)
                    (value1, value2, value3, policyValues[0], policyValues[1], policyValues[2], policyValues[3], policyValues[4], policyValues[5]),
                _ => Invoke(expressionString)
            };
        }

        private Lambda GetLambda(string expressionString)
        {
            Type type = typeof(Lambda);
            if (_cachePool.TryGetValue(type, out var cache) is false)
            {
                cache = new ExpressionCache();
                _cachePool[type] = cache;
            }

            var cacheImpl = (ExpressionCache) cache;
            if (cacheImpl.TryGet(expressionString, out var func))
            {
                return func;
            }

            var lambda = CreateExpression(expressionString);
            cacheImpl.Set(expressionString, lambda);
            return lambda;
        }

        private TFunc GetFunc<TFunc>(string expressionString) where TFunc : Delegate
        {
            Type type = typeof(TFunc);
            if (_cachePool.TryGetValue(type, out var cache) is false)
            {
                cache = new ExpressionCache<TFunc>();
                _cachePool[type] = cache;
            }

            var cacheImpl = (IExpressionCache<TFunc>) cache;
            if (cacheImpl.TryGet(expressionString, out var func))
            {
                return func;
            }

            var expression = CreateExpression(expressionString);
            func = expression.Compile<TFunc>();
            cacheImpl.Set(expressionString, func);
            return func;
        }

        private void ClearCache()
        {
            foreach (var cache in _cachePool.Values)
            {
                cache?.Clear();
            }
        }

        private void SetRequestParameters(IReadOnlyList<object> requestValues)
        {
            foreach (string token in RequestTokens.Keys)
            {
                object requestValue = requestValues?[RequestTokens[token]];

                if (Parameters.ContainsKey(token))
                {
                    if (requestValue is not null)
                    {
                        Parameters[token] = new Parameter(token, requestValue);
                    }
                }
                else
                {
                    Parameters.Add(token, new Parameter(token, requestValue ?? string.Empty));
                }

                _orderedParameters[RequestTokens[token]] = Parameters[token];
            }
        }

        private void SetPolicyParameters(IReadOnlyList<string> policyValues = null)
        {
            int requestCount = RequestTokens.Count;
            foreach (string token in PolicyTokens.Keys)
            {
                string policyValue = policyValues?[PolicyTokens[token]];

                if (Parameters.ContainsKey(token))
                {
                    if (policyValue is not null)
                    {
                        Parameters[token] = new Parameter(token, policyValue);
                    }
                }
                else
                {
                    Parameters.Add(token, new Parameter(token, policyValue ?? string.Empty));
                }
                _orderedParameters[PolicyTokens[token] + requestCount] = Parameters[token];
            }
        }

        private Lambda CreateExpression(string expressionString)
        {
            Parameter[] parameterArray = GetParameters();
            Interpreter interpreter = GetInterpreter();
            return interpreter.Parse(expressionString, parameterArray);;
        }

        private Parameter[] GetParameters()
        {
            SetRequestParameters(RequestValues);
            SetPolicyParameters(PolicyValues);
            return _orderedParameters;
        }

        private Interpreter GetInterpreter()
        {
            if (_interpreter is not null)
            {
                return _interpreter;
            }

            _interpreter = CreateInterpreter();
            return _interpreter;
        }

        private Interpreter CreateInterpreter()
        {
            var interpreter = new Interpreter();
            SetFunctions(interpreter);
            SetGFunctions(interpreter);
            return interpreter;
        }

        private void SetFunctions(Interpreter interpreter)
        {
            foreach (KeyValuePair<string, Delegate> functionKeyValue in _functionMap.FunctionDict)
            {
                interpreter.SetFunction(functionKeyValue.Key, functionKeyValue.Value);
            }
        }

        private void SetGFunctions(Interpreter interpreter)
        {
            if (_model.Model.ContainsKey(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            foreach (KeyValuePair<string, Assertion> assertionKeyValue in _model.Model[PermConstants.Section.RoleSection])
            {
                string key = assertionKeyValue.Key;
                Assertion assertion = assertionKeyValue.Value;
                interpreter.SetFunction(key, BuiltInFunctions.GenerateGFunction(key, assertion.RoleManager));
            }
        }

    }
}
