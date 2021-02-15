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
        private readonly IDictionary<string, Lambda> _expressionCache
            = new Dictionary<string, Lambda>();

        private readonly IDictionary<string, Func<string, string, string, string, string, string, bool>> _onlyStringFuncCache
            = new Dictionary<string, Func<string, string, string, string, string, string, bool>>();

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

        public IDictionary<string, Parameter> Parameters { get; }
            = new Dictionary<string, Parameter>();

        public void SetFunction(string name, Delegate function)
        {
            _expressionCache.Clear();
            _onlyStringFuncCache.Clear();
            var interpreter = GetInterpreter();
            interpreter.SetFunction(name, function);
        }

        public void SetGFunctions()
        {
            _expressionCache.Clear();
            _onlyStringFuncCache.Clear();
            var interpreter = GetInterpreter();
            SetGFunctions(interpreter);
        }

        
        public void EnsureCreated(string expressionString, IReadOnlyList<object> requestValues)
        {
            if (_expressionCache.ContainsKey(expressionString))
            {
                return;
            }

            if (_onlyStringFuncCache.ContainsKey(expressionString))
            {
                return;
            }

            Lambda expression = CreateExpression(expressionString, requestValues);

            if (RequestTokens.Count is 3 && PolicyTokens.Count is 3
                && CheckRequestValuesOnlyString(requestValues))
            {
                _onlyStringFuncCache[expressionString] =
                    expression.Compile<Func<string, string, string, string, string, string, bool>>();
                return;
            }

            _expressionCache[expressionString] = expression;
        }

        public bool Invoke(string expressionString, IReadOnlyList<object> requestValues)
        {
            EnsureCreated(expressionString, requestValues);

            if (_expressionCache.TryGetValue(expressionString, out var lambda))
            {
                var expressionResult = lambda.Invoke(_orderedParameters);
                return expressionResult is bool result && result;
            }

            if (_onlyStringFuncCache.TryGetValue(
                expressionString, out var func) is false)
            {
                throw new ArgumentException($"Can not find expression of the expression string {expressionString}");
            }

            var parameters = _orderedParameters;
            return func(
                parameters[0].Value as string,
                parameters[1].Value as string,
                parameters[2].Value as string,
                parameters[3].Value as string,
                parameters[4].Value as string,
                parameters[5].Value as  string);
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

        public void SetPolicyParameters(IReadOnlyList<string> policyValues = null)
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

        private Lambda CreateExpression(string expressionString, IReadOnlyList<object> requestValues)
        {
            Parameter[] parameterArray = GetParameters(requestValues);
            Interpreter interpreter = GetInterpreter();
            return interpreter.Parse(expressionString, parameterArray);;
        }

        private Parameter[] GetParameters(IReadOnlyList<object> requestValues = null)
        {
            SetRequestParameters(requestValues);
            SetPolicyParameters();
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
