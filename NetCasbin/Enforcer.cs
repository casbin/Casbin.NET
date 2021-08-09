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
using System.Runtime.CompilerServices;
#if !NET45
using Microsoft.Extensions.Logging;
#endif

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
        public bool Enforce(in EnforceContext context, params object[] requestValues)
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

        private bool InternalEnforce(in EnforceContext context, IReadOnlyList<object> requestValues)
        {
            var session = new EnforceSession();
            var policyList = context.PolicyAssertion.Policy;
            session.RequestValues = requestValues;

            IChainEffector chainEffector = Effector as IChainEffector;
            session.IsChainEffector = chainEffector is not null;

            session = HandleInitialRequest(in context, ref session, chainEffector);

            if (session.PolicyCount != 0)
            {
                for (int policyIndex = 0; policyIndex < session.PolicyCount; policyIndex++)
                {
                    IReadOnlyList<string> policyValues = policyList[policyIndex];
                    session.PolicyValues = policyValues;
                    session.PolicyIndex = policyIndex;

                    session = HandleBeforeExpression(in context, ref session, chainEffector);
                    session.ExpressionResult = ExpressionHandler.Invoke(session.ExpressionString, requestValues);

                    session = session.IsChainEffector
                        ? HandleExpressionResult(in context, ref session, chainEffector)
                        : HandleExpressionResult(in context, ref session, Effector);

                    if (session.Determined)
                    {
                        break;
                    }
                }
            }
            else
            {
                session = HandleBeforeExpression(in context,ref session, chainEffector);
                session.ExpressionResult = ExpressionHandler.Invoke(session.ExpressionString, requestValues);

                session = session.IsChainEffector
                    ? HandleExpressionResult(in context, ref session, chainEffector)
                    : HandleExpressionResult(in context, ref session, Effector);
            }

            return session.EnforceResult;
        }

        private ref EnforceSession HandleInitialRequest(in EnforceContext context, ref EnforceSession session, IChainEffector chainEffector)
        {
            session.ExpressionString = context.Matcher;
            session.PolicyCount = context.PolicyAssertion.Policy.Count;

            int requestTokenCount = context.RequestAssertion.Tokens.Count;
            if (requestTokenCount != session.RequestValues.Count)
            {
                throw new ArgumentException($"Invalid request size: expected {requestTokenCount}, got {session.RequestValues.Count}.");
            }

            ExpressionHandler.SetEnforceContext(in context);
            ExpressionHandler.SetRequestParameters(session.RequestValues);

            if (session.IsChainEffector)
            {
                chainEffector.StartChain(context.Effect);
            }
            else
            {
                session.PolicyEffects = new PolicyEffect[session.PolicyCount];
            }

            session.EffectExpressionType = chainEffector?.EffectExpressionType ?? DefaultEffector.ParseEffectExpressionType(session.ExpressionString);
            session.HasPriority = context.PolicyAssertion.TryGetTokenIndex("priority", out int priorityIndex);
            session.PriorityIndex = priorityIndex;
            return ref session;
        }

        private ref EnforceSession HandleBeforeExpression(in EnforceContext context, ref EnforceSession session, IChainEffector chainEffector)
        {
            int policyTokenCount = context.PolicyAssertion.Tokens.Count;

            if (session.PolicyCount is 0)
            {
                if (context.HasEval)
                {
                    throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                }

                IReadOnlyList<string> tempPolicyValues = Enumerable.Repeat(string.Empty, policyTokenCount).ToArray();
                ExpressionHandler.SetPolicyParameters(tempPolicyValues);
                return ref session;
            }

            if (policyTokenCount != session.PolicyValues.Count)
            {
                throw new ArgumentException($"Invalid policy size: expected {policyTokenCount}, got {session.PolicyValues.Count}.");
            }

            if (session.IsChainEffector is false && session.EffectExpressionType is EffectExpressionType.PriorityDenyOverride)
            {
                ThrowHelper.ThrowNotSupportException($"Only {nameof(IChainEffector)} support {nameof(EffectExpressionType.PriorityDenyOverride)} policy effect expression.");
            }

            if (session.HasPriority && session.EffectExpressionType is EffectExpressionType.PriorityDenyOverride)
            {
                if (int.TryParse(session.PolicyValues[session.PriorityIndex], out int nowPriority))
                {
                    if (session.Priority.HasValue && nowPriority != session.Priority.Value
                        && chainEffector.HitPolicyCount > 0)
                    {
                        session.DetermineResult(chainEffector.Result);
                    }
                    session.Priority = nowPriority;
                }
            }

            ExpressionHandler.SetPolicyParameters(session.PolicyValues);

            if (context.HasEval)
            {
                session.ExpressionString = context.Matcher;
                session.ExpressionString = RewriteEval(session.ExpressionString, context.PolicyAssertion.Tokens, session.PolicyValues);
            }

            return ref session;
        }

        private static ref EnforceSession HandleExpressionResult(in EnforceContext context, ref EnforceSession session, IEffector effector)
        {
            PolicyEffect nowEffect;
            if (session.PolicyCount is 0)
            {
                nowEffect = GetEffect(session.ExpressionResult);
                nowEffect = effector.MergeEffects(context.Effect, new[] { nowEffect }, null, session.PolicyIndex, session.PolicyCount, out _);
                bool finalResult = nowEffect.ToNullableBool() ?? false;
                session.DetermineResult(finalResult);
                return ref session;
            }

            IReadOnlyList<string> policyValues = session.PolicyValues;
            nowEffect = GetEffect(session.ExpressionResult);

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

            session.PolicyEffects[session.PolicyIndex] = nowEffect;
            nowEffect = effector.MergeEffects(context.Effect, session.PolicyEffects, null, session.PolicyIndex, session.PolicyCount, out int hitPolicyIndex);

            if (context.Explain && hitPolicyIndex is not -1)
            {
                context.Explanations.Add(policyValues);
            }

            if (nowEffect is not PolicyEffect.Indeterminate)
            {
                session.DetermineResult(nowEffect.ToNullableBool() ?? false);
                return ref session;
            }

            session.EnforceResult = false;
            return ref session;
        }

        private static ref EnforceSession HandleExpressionResult(in EnforceContext context, ref EnforceSession session, IChainEffector chainEffector)
        {
            PolicyEffect nowEffect;
            if (session.PolicyCount is 0)
            {
                nowEffect = GetEffect(session.ExpressionResult);

                if (chainEffector.TryChain(nowEffect))
                {
                    session.DetermineResult(chainEffector.Result);
                    return ref session;
                }

                session.DetermineResult(false);
                return ref session;
            }

            IReadOnlyList<string> policyValues = session.PolicyValues;
            nowEffect = GetEffect(session.ExpressionResult);

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
                session.DetermineResult(chainEffector.Result);
                return ref session;
            }

            session.EnforceResult = chainEffector.Result;
            return ref session;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
