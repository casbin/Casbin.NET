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

        public Enforcer(string modelPath, string policyPath)
            : this(modelPath, new FileAdapter(policyPath))
        {
        }

        public Enforcer(string modelPath, IAdapter adapter = null)
            : this(DefaultModel.CreateFromFile(modelPath), adapter)
        {
        }

        public Enforcer(IModel model, IAdapter adapter = null)
        {
            this.SetModel(model);
            if (adapter is not null)
            {
                this.SetAdapter(adapter);
            }
            this.LoadPolicy();
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
        public bool Enforce(params object[] requestValues)
        {
            if (Enabled is false)
            {
                return true;
            }

            if (EnabledCache is false)
            {
                return InternalEnforce(PolicyManager, requestValues);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return InternalEnforce(PolicyManager, requestValues);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            if (EnforceCache.TryGetResult(requestValues, key, out bool cachedResult))
            {
#if !NET45
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
#endif
                return cachedResult;
            }

            bool result  = InternalEnforce(PolicyManager, requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            EnforceCache.TrySetResult(requestValues, key, result);
            return result;
        }

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public async Task<bool> EnforceAsync(params object[] requestValues)
        {
            if (Enabled is false)
            {
                return true;
            }

            if (EnabledCache is false)
            {
                return await InternalEnforceAsync(PolicyManager, requestValues);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return await InternalEnforceAsync(PolicyManager, requestValues);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            bool? tryGetCachedResult = await EnforceCache.TryGetResultAsync(requestValues, key);
            if (tryGetCachedResult.HasValue)
            {
                bool cachedResult = tryGetCachedResult.Value;
#if !NET45
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
#endif
                return cachedResult;
            }

            bool result = await InternalEnforceAsync(PolicyManager, requestValues);

            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            await EnforceCache.TrySetResultAsync(requestValues, key, result);
            return result;
        }

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
#if !NET45
        public (bool Result, IEnumerable<IEnumerable<string>> Explains)
            EnforceEx(params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            if (Enabled is false)
            {
                return (true, explains);
            }

            if (EnabledCache is false)
            {
                return (InternalEnforce(PolicyManager, requestValues, null, explains), explains);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return (InternalEnforce(PolicyManager, requestValues, null, explains), explains);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            if (EnforceCache.TryGetResult(requestValues, key, out bool cachedResult))
            {
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
                return (cachedResult, explains);
            }

            bool result  = InternalEnforce(PolicyManager, requestValues, null, explains);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            EnforceCache.TrySetResult(requestValues, key, result);
            return (result, explains);
        }
#else
        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
        public Tuple<bool, IEnumerable<IEnumerable<string>>>
            EnforceEx(params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            bool result = InternalEnforce(PolicyManager, requestValues, null, explains);
            return new Tuple<bool, IEnumerable<IEnumerable<string>>>(result, explains);
        }
#endif

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
#if !NET45
        public async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
            EnforceExAsync(params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            if (Enabled is false)
            {
                return (true, explains);
            }

            if (EnabledCache is false)
            {
                return (await InternalEnforceAsync(PolicyManager, requestValues, null, explains), explains);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return (await InternalEnforceAsync(PolicyManager, requestValues, null, explains), explains);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            if (EnforceCache.TryGetResult(requestValues, key, out bool cachedResult))
            {
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
                return (cachedResult, explains);
            }

            bool result  = await InternalEnforceAsync(PolicyManager, requestValues, null, explains);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            await EnforceCache.TrySetResultAsync(requestValues, key, result);
            return (result, explains);
        }
#else
        public async Task<Tuple<bool, IEnumerable<IEnumerable<string>>>>
            EnforceExAsync(params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            bool result = await InternalEnforceAsync(PolicyManager, requestValues, null, explains);
            return new Tuple<bool, IEnumerable<IEnumerable<string>>>(result, explains);
        }
#endif

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public bool EnforceWithMatcher(string matcher, params object[] requestValues)
        {
            if (Enabled is false)
            {
                return true;
            }

            if (EnabledCache is false)
            {
                return InternalEnforce(PolicyManager, requestValues, matcher);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return InternalEnforce(PolicyManager, requestValues, matcher);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            if (EnforceCache.TryGetResult(requestValues, key, out bool cachedResult))
            {
#if !NET45
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
#endif
                return cachedResult;
            }

            bool result = InternalEnforce(PolicyManager, requestValues, matcher);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            EnforceCache.TrySetResult(requestValues, key, result);
            return result;
        }

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public async Task<bool> EnforceWithMatcherAsync(string matcher, params object[] requestValues)
        {
            if (Enabled is false)
            {
                return true;
            }

            if (EnabledCache is false)
            {
                return await InternalEnforceAsync(PolicyManager, requestValues, matcher);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return await InternalEnforceAsync(PolicyManager, requestValues, matcher);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            bool? tryGetCachedResult = await EnforceCache.TryGetResultAsync(requestValues, key);
            if (tryGetCachedResult.HasValue)
            {
                bool cachedResult = tryGetCachedResult.Value;
#if !NET45
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
#endif
                return cachedResult;
            }

            bool result = await InternalEnforceAsync(PolicyManager, requestValues, matcher);

            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            await EnforceCache.TrySetResultAsync(requestValues, key, result);
            return result;
        }

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
#if !NET45
        public (bool Result, IEnumerable<IEnumerable<string>> Explains)
            EnforceExWithMatcher(string matcher, params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            if (Enabled is false)
            {
                return (true, explains);
            }

            if (EnabledCache is false)
            {
                return (InternalEnforce(PolicyManager, requestValues, matcher, explains), explains);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return (InternalEnforce(PolicyManager, requestValues, matcher, explains), explains);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            if (EnforceCache.TryGetResult(requestValues, key, out bool cachedResult))
            {
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
                return (cachedResult, explains);
            }

            bool result = InternalEnforce(PolicyManager, requestValues, matcher, explains);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            EnforceCache.TrySetResult(requestValues, key, result);
            return (result, explains);
        }
#else
        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
        public Tuple<bool, IEnumerable<IEnumerable<string>>>
            EnforceExWithMatcher(string matcher, params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            bool result = InternalEnforce(PolicyManager, requestValues, matcher, explains);
            return new Tuple<bool, IEnumerable<IEnumerable<string>>>(result, explains);
        }
#endif

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
#if !NET45
        public async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
            EnforceExWithMatcherAsync(string matcher, params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            if (Enabled is false)
            {
                return (true, explains);
            }

            if (EnabledCache is false)
            {
                return (await InternalEnforceAsync(PolicyManager, requestValues, matcher, explains), explains);
            }

            if (requestValues.Any(requestValue => requestValue is not string))
            {
                return (await InternalEnforceAsync(PolicyManager, requestValues, matcher, explains), explains);
            }

            string key = string.Join("$$", requestValues);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            if (EnforceCache.TryGetResult(requestValues, key, out bool cachedResult))
            {
                Logger?.LogEnforceCachedResult(requestValues, cachedResult);
                return (cachedResult, explains);
            }

            bool result = await InternalEnforceAsync(PolicyManager, requestValues, matcher, explains);
            EnforceCache ??= new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
            await EnforceCache.TrySetResultAsync(requestValues, key, result);
            return (result, explains);
        }
#else
        public async Task<Tuple<bool, IEnumerable<IEnumerable<string>>>>
            EnforceExWithMatcherAsync(string matcher, params object[] requestValues)
        {
            var explains = new List<IEnumerable<string>>();
            bool result = await InternalEnforceAsync(PolicyManager, requestValues, matcher, explains);
            return new Tuple<bool, IEnumerable<IEnumerable<string>>>(result, explains);
        }
#endif

        private async Task<bool> InternalEnforceAsync(IPolicyManager policyManager, IReadOnlyList<object> requestValues,
            string matcher = null, ICollection<IEnumerable<string>> explains = null)
        {
            if (policyManager.IsSynchronized)
            {
                return await Task.Run(() => InternalEnforce(requestValues, matcher, explains));
            }
            return InternalEnforce(policyManager, requestValues, matcher, explains);
        }

        private bool InternalEnforce(IPolicyManager policyManager, IReadOnlyList<object> requestValues,
            string matcher = null, ICollection<IEnumerable<string>> explains = null)
        {
            try
            {
                policyManager.StartRead();
                return InternalEnforce(requestValues, matcher, explains);
            }
            finally
            {
                policyManager.EndRead();
            }
        }

        private bool InternalEnforce(IReadOnlyList<object> requestValues, string matcher = null, ICollection<IEnumerable<string>> explains = null)
        {
            bool explain = explains is not null;
            string effect = Model.Sections[PermConstants.Section.PolicyEffectSection][PermConstants.DefaultPolicyEffectType].Value;
            var policyList = Model.Sections[PermConstants.Section.PolicySection][PermConstants.DefaultPolicyType].Policy;
            int policyCount = Model.Sections[PermConstants.Section.PolicySection][PermConstants.DefaultPolicyType].Policy.Count;
            string expressionString = matcher is not null
                ? Utility.EscapeAssertion(matcher)
                : Model.Sections[PermConstants.Section.MatcherSection][PermConstants.DefaultMatcherType].Value;

            int requestTokenCount = ExpressionHandler.RequestTokens.Count;
            if (requestTokenCount != requestValues.Count)
            {
                throw new ArgumentException($"Invalid request size: expected {requestTokenCount}, got {requestValues.Count}.");
            }
            int policyTokenCount = ExpressionHandler.PolicyTokens.Count;

            ExpressionHandler.SetRequestParameters(requestValues);

            bool hasEval = Utility.HasEval(expressionString);

            bool finalResult = false;
            IChainEffector chainEffector = Effector as IChainEffector;
            bool isChainEffector = chainEffector is not null;
            if (isChainEffector)
            {
                chainEffector.StartChain(effect);

                if (policyCount is not 0)
                {
                    foreach (IReadOnlyList<string> policyValues in policyList)
                    {
                        if (policyTokenCount != policyValues.Count)
                        {
                            throw new ArgumentException($"Invalid policy size: expected {policyTokenCount}, got {policyValues.Count}.");
                        }

                        ExpressionHandler.SetPolicyParameters(policyValues);

                        bool expressionResult;

                        if (hasEval)
                        {
                            string expressionStringWithRule = RewriteEval(expressionString, ExpressionHandler.PolicyTokens, policyValues);
                            expressionResult = ExpressionHandler.Invoke(expressionStringWithRule, requestValues);
                        }
                        else
                        {
                            expressionResult = ExpressionHandler.Invoke(expressionString, requestValues);
                        }

                        var nowEffect = GetEffect(expressionResult);

                        if (nowEffect is not PolicyEffect.Indeterminate && ExpressionHandler.Parameters.TryGetValue("p_eft", out Parameter parameter))
                        {
                            string policyEffect = parameter.Value as string;
                            nowEffect = policyEffect switch
                            {
                                "allow" => PolicyEffect.Allow,
                                "deny" => PolicyEffect.Deny,
                                _ => PolicyEffect.Indeterminate
                            };
                        }

                        bool chainResult = chainEffector.TryChain(nowEffect);

                        if (explain && chainEffector.HitPolicy)
                        {
                            explains.Add(policyValues);
                        }

                        if (chainResult is false || chainEffector.CanChain is false)
                        {
                            break;
                        }
                    }

                    finalResult = chainEffector.Result;
                }
                else
                {
                    if (hasEval)
                    {
                        throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                    }

                    IReadOnlyList<string> policyValues = Enumerable.Repeat(string.Empty, policyTokenCount).ToArray();
                    ExpressionHandler.SetPolicyParameters(policyValues);
                    var nowEffect = GetEffect(ExpressionHandler.Invoke(expressionString, requestValues));

                    if (chainEffector.TryChain(nowEffect))
                    {
                        finalResult = chainEffector.Result;
                    }

                    if (explain && chainEffector.HitPolicy)
                    {
                        explains.Add(policyValues);
                    }
                }

#if !NET45
                if (explain)
                {
                    Logger?.LogEnforceResult(requestValues, finalResult, explains);
                }
                else
                {
                    Logger?.LogEnforceResult(requestValues, finalResult);
                }
#endif
                return finalResult;
            }

            int hitPolicyIndex;
            if (policyCount != 0)
            {
                PolicyEffect[] policyEffects = new PolicyEffect[policyCount];

                for (int i = 0; i < policyCount; i++)
                {
                    IReadOnlyList<string> policyValues = policyList[i];

                    if (policyTokenCount != policyValues.Count)
                    {
                        throw new ArgumentException($"Invalid policy size: expected {policyTokenCount}, got {policyValues.Count}.");
                    }

                    ExpressionHandler.SetPolicyParameters(policyValues);

                    bool expressionResult;

                    if (hasEval)
                    {
                        string expressionStringWithRule = RewriteEval(expressionString, ExpressionHandler.PolicyTokens, policyValues);
                        expressionResult = ExpressionHandler.Invoke(expressionStringWithRule, requestValues);
                    }
                    else
                    {
                        expressionResult = ExpressionHandler.Invoke(expressionString, requestValues);
                    }

                    var nowEffect = GetEffect(expressionResult);

                    if (nowEffect is PolicyEffect.Indeterminate)
                    {
                        policyEffects[i] = nowEffect;
                        continue;
                    }

                    if (ExpressionHandler.Parameters.TryGetValue("p_eft", out Parameter parameter))
                    {
                        string policyEffect = parameter.Value as string;
                        nowEffect = policyEffect switch
                        {
                            "allow" => PolicyEffect.Allow,
                            "deny" => PolicyEffect.Deny,
                            _ => PolicyEffect.Indeterminate
                        };
                    }

                    policyEffects[i] = nowEffect;

                    if (effect.Equals(PermConstants.PolicyEffect.Priority))
                    {
                        break;
                    }
                }

                finalResult = Effector.MergeEffects(effect, policyEffects, null, out hitPolicyIndex);
            }
            else
            {
                if (hasEval)
                {
                    throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                }

                IReadOnlyList<string> policyValues = Enumerable.Repeat(string.Empty, policyTokenCount).ToArray();
                ExpressionHandler.SetPolicyParameters(policyValues);
                var nowEffect = GetEffect(ExpressionHandler.Invoke(expressionString, requestValues));
                finalResult = Effector.MergeEffects(effect, new[] { nowEffect }, null, out hitPolicyIndex);
            }

            if (explain && hitPolicyIndex is not -1)
            {
                explains.Add(policyList[hitPolicyIndex]);
            }

#if !NET45
            if (explain)
            {
                Logger?.LogEnforceResult(requestValues, finalResult, explains);
            }
            else
            {
                Logger?.LogEnforceResult(requestValues, finalResult);
            }
#endif
            return finalResult;
        }

        private static PolicyEffect GetEffect(bool expressionResult)
        {
            return expressionResult ? PolicyEffect.Allow : PolicyEffect.Indeterminate;
        }

        private static string RewriteEval(string expressionString, IDictionary<string, int> policyTokens, IReadOnlyList<string> policyValues)
        {
            if (Utility.TryGetEvalRuleNames(expressionString, out IEnumerable<string> ruleNames) is false)
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
                rules[ruleName] = Utility.EscapeAssertion(policyValues[ruleIndex]);
            }

            expressionString = Utility.ReplaceEval(expressionString, rules);
            return expressionString;
        }

        #endregion
    }
}
