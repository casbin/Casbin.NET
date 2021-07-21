using System;
using System.Collections.Generic;
using System.Linq;
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
        private IReadOnlyList<object> _requestValues;
        private IReadOnlyList<string> _policyValues;
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

        public IReadOnlyDictionary<string, int> RequestTokens { get; }

        public IReadOnlyDictionary<string, int> PolicyTokens { get; }

        public IDictionary<string, Parameter> Parameters { get; }
            = new Dictionary<string, Parameter>();

        public void SetFunction(string name, Delegate function)
        {
            ClearCache();
            Interpreter interpreter = GetInterpreter();
            interpreter.SetFunction(name, function);
        }

        public void SetGFunctions()
        {
            ClearCache();
            Interpreter interpreter = GetInterpreter();
            SetGFunctions(interpreter);
        }

        public bool Invoke(string expressionString)
        {
            if (CheckRequestValuesOnlyString(_requestValues))
            {
                return InvokeOnlyString(expressionString);
            }

            Parameter[] parameters = GetParameters();
            object expressionResult = GetLambda(expressionString).Invoke(parameters);
            return expressionResult is bool result && result;
        }

        public bool InvokeOnlyString(string expressionString)
        {
            bool result = false;
            int policyTokenCount = PolicyTokens.Count;
            int requestTokenCount = RequestTokens.Count;

            if (requestTokenCount is 0)
            {
                result = policyTokenCount switch
                {
                    0 => GetFunc<Func<bool>>(expressionString)(),

                    1 => GetFunc<Func<string, bool>>(expressionString)
                        (_policyValues[0]),

                    2 => GetFunc<Func<string, string, bool>>(expressionString)
                        (_policyValues[0], _policyValues[1]),

                    3 => GetFunc<Func<string, string, string, bool>>(expressionString)
                        (_policyValues[0], _policyValues[1], _policyValues[2]),

                    4 => GetFunc<Func<string, string, string, string, bool>>(expressionString)
                        (_policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3]),

                    5 => GetFunc<Func<string, string, string, string, string, bool>>(expressionString)
                        (_policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3], _policyValues[4]),

                    6 => GetFunc<Func<string, string, string, string, string, string, bool>>(expressionString)
                        (_policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3], _policyValues[4], _policyValues[5]),

                    _ => Invoke(expressionString)
                };
            }
            else if (requestTokenCount is 1)
            {
                result = policyTokenCount switch
                {
                    0 => GetFunc<Func<string, bool>>(expressionString)
                        (_requestValues[0] as string),

                    1 => GetFunc<Func<string, string, bool>>(expressionString)
                        (_requestValues[0] as string,
                        _policyValues[0]),

                    2 => GetFunc<Func<string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string,
                        _policyValues[0], _policyValues[1]),

                    3 => GetFunc<Func<string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2]),

                    4 => GetFunc<Func<string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3]),

                    5 => GetFunc<Func<string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3], _policyValues[4]),

                    6 => GetFunc<Func<string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3], _policyValues[4], _policyValues[5]),

                    _ => Invoke(expressionString)
                };
            }
            else if(requestTokenCount is 2)
            {
                result = policyTokenCount switch
                {
                    0 => GetFunc<Func<string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string),

                    1 => GetFunc<Func<string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string,
                        _policyValues[0]),

                    2 => GetFunc<Func<string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string,
                        _policyValues[0], _policyValues[1]),

                    3 => GetFunc<Func<string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2]),

                    4 => GetFunc<Func<string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3]),

                    5 => GetFunc<Func<string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3], _policyValues[4]),

                    6 => GetFunc<Func<string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3], _policyValues[4], _policyValues[5]),

                    _ => Invoke(expressionString)
                };
            }
            else if (requestTokenCount is 3)
            {
                result = policyTokenCount switch
                {
                    0 => GetFunc<Func<string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string),

                    1 => GetFunc<Func<string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string,
                        _policyValues[0]),

                    2 => GetFunc<Func<string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string,
                        _policyValues[0], _policyValues[1]),

                    3 => GetFunc<Func<string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2]),

                    4 => GetFunc<Func<string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3]),

                    5 => GetFunc<Func<string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3], _policyValues[4]),

                    6 => GetFunc<Func<string, string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3], _policyValues[4], _policyValues[5]),

                    _ => Invoke(expressionString)
                };
            }
            else if (requestTokenCount is 4)
            {
                result = policyTokenCount switch
                {
                    0 => GetFunc<Func<string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string, _requestValues[3] as string),

                    1 => GetFunc<Func<string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string, _requestValues[3] as string,
                        _policyValues[0]),

                    2 => GetFunc<Func<string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string, _requestValues[3] as string,
                        _policyValues[0], _policyValues[1]),

                    3 => GetFunc<Func<string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string, _requestValues[3] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2]),

                    4 => GetFunc<Func<string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string, _requestValues[3] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3]),

                    5 => GetFunc<Func<string, string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string, _requestValues[3] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3], _policyValues[4]),

                    6 => GetFunc<Func<string, string, string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string, _requestValues[3] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3], _policyValues[4], _policyValues[5]),

                    _ => Invoke(expressionString)
                };
            }
            else if (requestTokenCount is 5)
            {
                result = policyTokenCount switch
                {
                    0 => GetFunc<Func<string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string, _requestValues[3] as string, _requestValues[4] as string),

                    1 => GetFunc<Func<string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string, _requestValues[3] as string, _requestValues[4] as string,
                        _policyValues[0]),

                    2 => GetFunc<Func<string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string, _requestValues[3] as string, _requestValues[4] as string,
                        _policyValues[0], _policyValues[1]),

                    3 => GetFunc<Func<string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string, _requestValues[3] as string, _requestValues[4] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2]),

                    4 => GetFunc<Func<string, string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string, _requestValues[3] as string, _requestValues[4] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3]),

                    5 => GetFunc<Func<string, string, string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string, _requestValues[3] as string, _requestValues[4] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3], _policyValues[4]),

                    6 => GetFunc<Func<string, string, string, string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[1] as string, _requestValues[2] as string, _requestValues[3] as string, _requestValues[4] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3], _policyValues[4], _policyValues[5]),

                    _ => Invoke(expressionString)
                };
            }
            else if (requestTokenCount is 6)
            {
                result = policyTokenCount switch
                {
                    0 => GetFunc<Func<string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[0] as string, _requestValues[2] as string, _requestValues[3] as string, _requestValues[4] as string, _requestValues[5] as string),

                    1 => GetFunc<Func<string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[0] as string, _requestValues[2] as string, _requestValues[3] as string, _requestValues[4] as string, _requestValues[5] as string,
                        _policyValues[0]),

                    2 => GetFunc<Func<string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[0] as string, _requestValues[2] as string, _requestValues[3] as string, _requestValues[4] as string, _requestValues[5] as string,
                        _policyValues[0], _policyValues[1]),

                    3 => GetFunc<Func<string, string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[0] as string, _requestValues[2] as string, _requestValues[3] as string, _requestValues[4] as string, _requestValues[5] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2]),

                    4 => GetFunc<Func<string, string, string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[0] as string, _requestValues[2] as string, _requestValues[3] as string, _requestValues[4] as string, _requestValues[5] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3]),

                    5 => GetFunc<Func<string, string, string, string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[0] as string, _requestValues[2] as string, _requestValues[3] as string, _requestValues[4] as string, _requestValues[5] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3], _policyValues[4]),

                    6 => GetFunc<Func<string, string, string, string, string, string, string, string, string, string, string, string, bool>>(expressionString)
                        (_requestValues[0] as string, _requestValues[0] as string, _requestValues[2] as string, _requestValues[3] as string, _requestValues[4] as string, _requestValues[5] as string,
                        _policyValues[0], _policyValues[1], _policyValues[2], _policyValues[3], _policyValues[4], _policyValues[5]),

                    _ => Invoke(expressionString)
                };
            }
            else
            {
                result = Invoke(expressionString);
            }

            return result;
        }

        public void SetRequest(IReadOnlyList<object> requestValues)
        {
            _requestValues = requestValues;
        }

        public void SetPolicy(IReadOnlyList<string> policyValues)
        {
            _policyValues = policyValues;
        }

        private Lambda CreateExpression(string expressionString)
        {
            Parameter[] parameterArray = GetParameters();
            Interpreter interpreter = GetInterpreter();
            return interpreter.Parse(expressionString, parameterArray);;
        }

        private Lambda GetLambda(string expressionString)
        {
            Type type = typeof(Lambda);
            if (_cachePool.TryGetValue(type, out IExpressionCache cache) is false)
            {
                cache = new ExpressionCache();
                _cachePool[type] = cache;
            }

            var cacheImpl = (ExpressionCache) cache;
            if (cacheImpl.TryGet(expressionString, out Lambda func))
            {
                return func;
            }

            Lambda lambda = CreateExpression(expressionString);
            cacheImpl.Set(expressionString, lambda);
            return lambda;
        }

        private TFunc GetFunc<TFunc>(string expressionString) where TFunc : Delegate
        {
            Type type = typeof(TFunc);
            if (_cachePool.TryGetValue(type, out IExpressionCache cache) is false)
            {
                cache = new ExpressionCache<TFunc>();
                _cachePool[type] = cache;
            }

            var cacheImpl = (IExpressionCache<TFunc>) cache;
            if (cacheImpl.TryGet(expressionString, out TFunc func))
            {
                return func;
            }

            Lambda expression = CreateExpression(expressionString);
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

        private Interpreter GetInterpreter()
        {
            if (_interpreter is not null)
            {
                return _interpreter;
            }

            _interpreter = CreateInterpreter();
            return _interpreter;
        }

        private Parameter[] GetParameters()
        {
            SetRequestParameters(_requestValues);
            SetPolicyParameters(_policyValues);
            return _orderedParameters;
        }

        public void SetRequestParameters(IReadOnlyList<object> requestValues)
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

        public void SetPolicyParameters(IReadOnlyList<string> policyValues)
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

        private bool CheckRequestValuesOnlyString(IReadOnlyCollection<object> requestValues)
        {
            int count = RequestTokens.Count;

            if (requestValues.Count != count)
            {
                throw new ArgumentException("Request values count should equal to request tokens.");
            }

            return requestValues.All(value => value is string);
        }
    }
}
