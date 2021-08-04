using System;
using System.Collections.Generic;
using System.Linq;
using Casbin.Model;
using Casbin.Util;
using DynamicExpresso;

namespace Casbin.Evaluation
{
    public class ExpressionHandler : IExpressionHandler
    {
        private readonly IDictionary<string, Lambda> _expressionCache
            = new Dictionary<string, Lambda>();

        private readonly IDictionary<string, Func<string, string, string, string, string, string, bool>> _onlyStringFuncCache
            = new Dictionary<string, Func<string, string, string, string, string, string, bool>>();

        private readonly FunctionMap _functionMap = FunctionMap.LoadFunctionMap();
        private readonly IModel _model;
        private Interpreter _interpreter;
        private Parameter[] _orderedParameters;

        private IReadOnlyDictionary<string, int> _requestTokenDic;
        private IEnumerable<string> _requestTokens;
        private IReadOnlyDictionary<string, int> _policyTokenDic;
        private IEnumerable<string> _policyTokens;

        public ExpressionHandler(IModel model)
        {
            _model = model;
        }

        public IDictionary<string, Parameter> Parameters { get; }
            = new Dictionary<string, Parameter>();

        public EnforceContext EnforceContext { get; private set; }

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


        public void SetEnforceContext(ref EnforceContext context)
        {
            EnforceContext = context;
            int parametersCount = 0;

            _requestTokenDic = context.RequestAssertion.Tokens;
            _requestTokens = _requestTokenDic.Keys;
            _policyTokenDic = context.PolicyAssertion.Tokens;
            _policyTokens = _policyTokenDic.Keys;

            parametersCount += _requestTokenDic.Count;
            parametersCount += _policyTokenDic.Count;
            _orderedParameters = new Parameter[parametersCount];
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

            if (_requestTokenDic.Count is 3 && _policyTokenDic.Count is 3
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
            foreach (string token in _requestTokens)
            {
                object requestValue = requestValues?[_requestTokenDic[token]];

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

                _orderedParameters[_requestTokenDic[token]] = Parameters[token];
            }
        }

        public void SetPolicyParameters(IReadOnlyList<string> policyValues = null)
        {
            int requestCount = _requestTokenDic.Count;
            foreach (string token in _policyTokens)
            {
                string policyValue = policyValues?[_policyTokenDic[token]];

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

                _orderedParameters[_policyTokenDic[token] + requestCount] = Parameters[token];
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
            if (_model.Sections.ContainsKey(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            foreach (KeyValuePair<string, Assertion> assertionKeyValue in _model.Sections[PermConstants.Section.RoleSection])
            {
                string key = assertionKeyValue.Key;
                Assertion assertion = assertionKeyValue.Value;
                interpreter.SetFunction(key, BuiltInFunctions.GenerateGFunction(key, assertion.RoleManager));
            }
        }

        private bool CheckRequestValuesOnlyString(IReadOnlyCollection<object> requestValues)
        {
            int count = _requestTokenDic.Count;

            if (requestValues.Count != count)
            {
                throw new ArgumentException("Request values count should equal to request tokens.");
            }

            return requestValues.All(value => value is string);
        }
    }
}
