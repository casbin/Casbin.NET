using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Adapter.File;
using Casbin.Caching;
using Casbin.Effect;
using Casbin.Extensions;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Rbac;
using Casbin.Util;
using Casbin.Evaluation;
#if !NET45
using Microsoft.Extensions.Logging;
#endif
using DynamicExpresso;

namespace Casbin
{
    public class Enforcer : IEnforcer
    {
        public Enforcer()
        {
        }

        public Enforcer(string modelPath, string policyPath, bool lazyLoadPolicy = false)
            : this(modelPath, new FileAdapter(policyPath), lazyLoadPolicy)
        {
        }

        public Enforcer(string modelPath, IAdapter adapter = null, bool lazyLoadPolicy = false)
            : this(DefaultModel.CreateFromFile(modelPath), adapter, lazyLoadPolicy)
        {
        }

        public Enforcer(IModel model, IAdapter adapter = null, bool lazyLoadPolicy = false)
        {
            this.SetModel(model);
            if (adapter is not null)
            {
                this.SetAdapter(adapter);
            }
            if (lazyLoadPolicy is false)
            {
                this.LoadPolicy();
            }
        }

        #region Options
        public bool Enabled { get; set; } = true;
        public bool EnabledCache { get; set; } = true;
        public bool AutoSave
        {
            get => PolicyManager.AutoSave;
            set => PolicyManager.AutoSave = value;
        }
        public bool AutoBuildRoleLinks { get; set; } = true;
        public bool AutoNotifyWatcher { get; set; } = true;
        public bool AutoCleanEnforceCache { get; set; } = true;
        #endregion

        #region Extensions
        public IEffector Effector { get; set; } = new DefaultEffector();
        public IModel Model { get; set; }
        public IPolicyManager PolicyManager
        {
            get => Model?.PolicyManager;
            set => Model.SetPolicyManager(value);
        }
        public IAdapter Adapter
        {
            get => PolicyManager?.Adapter;
            set => PolicyManager.SetAdapter(value);
        }
        public IWatcher Watcher { get; set; }
        public IRoleManager RoleManager { get; set; } = new DefaultRoleManager(10);
        public IEnforceCache EnforceCache { get; set; }
        public IExpressionHandler ExpressionHandler { get; set; }
#if !NET45
        public ILogger Logger { get; set; }
#endif
        #endregion
        public bool IsSynchronized => Model?.IsSynchronized ?? false;
        public string ModelPath => Model?.ModelPath;
        public bool IsFiltered => Adapter is IFilteredAdapter {IsFiltered: true};

        #region Enforce method

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public bool Enforce(EnforceContext context, params object[] requestValues)
        {
            if (Enabled is false)
            {
                return true;
            }

            if (EnabledCache is false)
            {
                return InternalEnforce(context, PolicyManager, requestValues);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return InternalEnforce(context, PolicyManager, requestValues);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            if (EnforceCache.TryGetResult(requestValues, key, out bool cachedResult))
            {
#if !NET45
                LogEnforceResult(context, requestValues, cachedResult, true);
#endif
                return cachedResult;
            }

            bool result  = InternalEnforce(context, PolicyManager, requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            EnforceCache.TrySetResult(requestValues, key, result);

#if !NET45
            LogEnforceResult(context, requestValues, result);
#endif
            return result;
        }

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public async Task<bool> EnforceAsync(EnforceContext context, params object[] requestValues)
        {
            if (Enabled is false)
            {
                return true;
            }

            if (EnabledCache is false)
            {
                return await InternalEnforceAsync(context, PolicyManager, requestValues);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return await InternalEnforceAsync(context, PolicyManager, requestValues);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            bool? tryGetCachedResult = await EnforceCache.TryGetResultAsync(requestValues, key);
            if (tryGetCachedResult.HasValue)
            {
                bool cachedResult = tryGetCachedResult.Value;
#if !NET45
                LogEnforceResult(context, requestValues, cachedResult, true);
#endif
                return cachedResult;
            }

            bool result = await InternalEnforceAsync(context, PolicyManager, requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            await EnforceCache.TrySetResultAsync(requestValues, key, result);

#if !NET45
            LogEnforceResult(context, requestValues, result);
#endif

            return result;
        }

        private async Task<bool> InternalEnforceAsync(EnforceContext context, IPolicyManager policyManager, IReadOnlyList<object> requestValues)
        {
            if (policyManager.IsSynchronized)
            {
                return await Task.Run(() => InternalEnforce(context, requestValues));
            }
            return InternalEnforce(in context, policyManager, requestValues);
        }

        private bool InternalEnforce(in EnforceContext context, IPolicyManager policyManager, IReadOnlyList<object> requestValues)
        {
            policyManager.StartRead();

            try
            {
                return InternalEnforce(context, requestValues);
            }
            finally
            {
                policyManager.EndRead();
            }
        }

        private bool InternalEnforce(EnforceContext context, IReadOnlyList<object> requestValues)
        {
            string expressionString = context.Matcher;
            var policyList = context.PolicyAssertion.Policy;
            int policyCount = policyList.Count;
            int requestTokenCount = context.RequestAssertion.Tokens.Count;
            if (requestTokenCount != requestValues.Count)
            {
                throw new ArgumentException($"Invalid request size: expected {requestTokenCount}, got {requestValues.Count}.");
            }
            ExpressionHandler.SetEnforceContext(ref context);
            ExpressionHandler.SetRequestParameters(requestValues);

            IChainEffector chainEffector = Effector as IChainEffector;
            bool isChainEffector = chainEffector is not null;
            bool expressionResult;
            bool? nowResult = null;

            PolicyEffect[] policyEffects = null;
            if (isChainEffector)
            {
                chainEffector.StartChain(context.Effect);
            }
            else
            {
                policyEffects = new PolicyEffect[policyCount];
            }

            if (policyCount != 0)
            {
                for (int policyIndex = 0; policyIndex < policyCount; policyIndex++)
                {
                    IReadOnlyList<string> policyValues = policyList[policyIndex];
                    string actualExpressionString = HandleBeforeExpression(context, expressionString, policyValues, policyCount);
                    expressionResult = ExpressionHandler.Invoke(actualExpressionString, requestValues);

                    bool determined = isChainEffector
                        ? HandleExpressionResult(context, expressionResult, chainEffector, policyValues, policyCount, out nowResult)
                        : HandleExpressionResult(context, expressionResult, Effector, policyEffects, policyValues, policyIndex, policyCount, out nowResult);

                    if (determined)
                    {
                        break;
                    }
                }
            }
            else
            {
                string actualExpressionString = HandleBeforeExpression(context, expressionString, null, policyCount);
                expressionResult = ExpressionHandler.Invoke(actualExpressionString, requestValues);

                _ = isChainEffector
                    ? HandleExpressionResult(context, expressionResult, chainEffector, null, policyCount, out nowResult)
                    : HandleExpressionResult(context, expressionResult, Effector, policyEffects, null, 0, policyCount, out nowResult);
            }
            bool finalResult = nowResult ?? false;
            return finalResult;
        }

        private string HandleBeforeExpression(in EnforceContext context, string expressionString, IReadOnlyList<string> policyValues, int policyCount)
        {
            int policyTokenCount = context.PolicyAssertion.Tokens.Count;

            if (policyCount is 0)
            {
                if (context.HasEval)
                {
                    throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                }

                IReadOnlyList<string> tempPolicyValues = Enumerable.Repeat(string.Empty, policyTokenCount).ToArray();
                ExpressionHandler.SetPolicyParameters(tempPolicyValues);
                return expressionString;
            }

            if (policyTokenCount != policyValues.Count)
            {
                throw new ArgumentException($"Invalid policy size: expected {policyTokenCount}, got {policyValues.Count}.");
            }

            ExpressionHandler.SetPolicyParameters(policyValues);

            if (context.HasEval)
            {
                expressionString = RewriteEval(expressionString, context.PolicyAssertion.Tokens, policyValues);
            }
            return expressionString;
        }

        private static bool HandleExpressionResult(in EnforceContext context, bool expressionResult, IEffector effector, PolicyEffect[] policyEffects,
            IReadOnlyList<string> policyValues, int policyIndex, int policyCount, out bool? nowResult)
        {
            PolicyEffect nowEffect;
            if (policyCount is 0)
            {
                nowEffect = GetEffect(expressionResult);
                nowEffect = effector.MergeEffects(context.Effect, new[] { nowEffect }, null, policyIndex, policyCount, out _);
                bool finalResult = nowEffect.ToNullableBool() ?? false;
                nowResult = finalResult;
                return true;
            }

            nowEffect = GetEffect(expressionResult);

            if (nowEffect is not PolicyEffect.Indeterminate && context.PolicyAssertion.TryGetTokenIndex("eft", out int index))
            {
                string policyEffect = policyValues[index];
                nowEffect = policyEffect switch
                {
                    "allow" => PolicyEffect.Allow,
                    "deny" => PolicyEffect.Deny,
                    _ => PolicyEffect.Indeterminate
                };
            }

            policyEffects[policyIndex] = nowEffect;
            nowEffect = effector.MergeEffects(context.Effect, policyEffects, null, policyIndex, policyCount, out int hitPolicyIndex);
            nowResult = nowEffect.ToNullableBool();

            if (context.Explain && hitPolicyIndex is not -1)
            {
                context.Explanations.Add(policyValues);
            }

            if (nowResult is not null)
            {
                return true;
            }

            nowResult = false;
            return false;
        }

        private static bool HandleExpressionResult(in EnforceContext context, bool expressionResult, IChainEffector chainEffector,
            IReadOnlyList<string> policyValues, int policyCount, out bool? nowResult)
        {
            PolicyEffect nowEffect;
            if (policyCount is 0)
            {
                nowEffect = GetEffect(expressionResult);

                if (chainEffector.TryChain(nowEffect))
                {
                    nowResult = chainEffector.Result;
                    return true;
                }

                nowResult = false;
                return true;
            }

            nowEffect = GetEffect(expressionResult);

            if (nowEffect is not PolicyEffect.Indeterminate && context.PolicyAssertion.TryGetTokenIndex("eft", out int index))
            {
                string policyEffect = policyValues[index];
                nowEffect = policyEffect switch
                {
                    "allow" => PolicyEffect.Allow,
                    "deny" => PolicyEffect.Deny,
                    _ => PolicyEffect.Indeterminate
                };
            }

            bool chainResult = chainEffector.TryChain(nowEffect);

            if (context.Explain && chainEffector.HitPolicy)
            {
                context.Explanations.Add(policyValues);
            }

            if (chainResult is false || chainEffector.CanChain is false)
            {
                nowResult = chainEffector.Result;
                return true;
            }

            nowResult = chainEffector.Result;
            return false;
        }

#if !NET45
        private void LogEnforceResult(in EnforceContext context, IReadOnlyList<object> requestValues, bool finalResult, bool cachedResult = false)
        {
            if (cachedResult)
            {
                Logger?.LogEnforceCachedResult(requestValues, finalResult);
                return;
            }

            if (context.Explain)
            {
                Logger?.LogEnforceResult(requestValues, finalResult, context.Explanations);
                return;
            }

            Logger?.LogEnforceResult(requestValues, finalResult);
        }
#endif

        private static PolicyEffect GetEffect(bool expressionResult)
        {
            return expressionResult ? PolicyEffect.Allow : PolicyEffect.Indeterminate;
        }

        private static string RewriteEval(string expressionString, IReadOnlyDictionary<string, int> policyTokens, IReadOnlyList<string> policyValues)
        {
            if (StringUtil.TryGetEvalRuleNames(expressionString, out IEnumerable<string> ruleNames) is false)
            {
                return expressionString;
            }

            Dictionary<string, string> rules = new();
            foreach (string ruleName in ruleNames)
            {
                if (policyTokens.TryGetValue(ruleName, out int ruleIndex) is false)
                {
                    throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                }
                rules[ruleName] = StringUtil.EscapeAssertion(policyValues[ruleIndex]);
            }

            expressionString = StringUtil.ReplaceEval(expressionString, rules);
            return expressionString;
        }

        #endregion
    }
}
