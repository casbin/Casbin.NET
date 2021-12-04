using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Adapter.File;
using Casbin.Caching;
using Casbin.Effect;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Rbac;
using Casbin.Util;
using Casbin.Evaluation;
using System.Runtime.CompilerServices;
#if !NET452
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

        public Enforcer(string modelPath, IReadOnlyAdapter adapter = null, bool lazyLoadPolicy = false)
            : this(DefaultModel.CreateFromFile(modelPath), adapter, lazyLoadPolicy)
        {
        }

        public Enforcer(IModel model, IReadOnlyAdapter adapter = null, bool lazyLoadPolicy = false)
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
        public IReadOnlyAdapter Adapter
        {
            get => PolicyManager?.Adapter;
            set => PolicyManager.SetAdapter(value);
        }
        public IWatcher Watcher { get; set; }
        public IRoleManager RoleManager { get; set; } = new DefaultRoleManager(10);
        public IEnforceCache EnforceCache { get; set; } = new ReaderWriterEnforceCache(new ReaderWriterEnforceCacheOptions());
#if !NET452
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
        /// <param name="context">Enforce context include all status on enforcing</param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public bool Enforce(EnforceContext context, params object[] requestValues)
        {
            if (context.HandleOptionAndCached)
            {
                return InternalEnforce(in context, PolicyManager, requestValues);
            }

            if (Enabled is false)
            {
                return true;
            }

            bool useCache = EnabledCache && requestValues.Any(requestValue => requestValue is not string) is false;
            string key = string.Empty;
            if (useCache)
            {
                key = string.Join("$$", requestValues);
                if (EnforceCache.TryGetResult(requestValues, key, out bool cachedResult))
                {
#if !NET452
                    this.LogEnforceCachedResult(requestValues, cachedResult);
#endif
                    return cachedResult;
                }
            }

            bool result = InternalEnforce(in context, PolicyManager, requestValues);

            if (useCache)
            {
                EnforceCache.TrySetResult(requestValues, key, result);
            }
#if !NET452
            this.LogEnforceResult(context, requestValues, result);
#endif
            return result;
        }

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="context">Enforce context</param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public async Task<bool> EnforceAsync(EnforceContext context, params object[] requestValues)
        {
            if (context.HandleOptionAndCached)
            {
                return await InternalEnforceAsync(context, PolicyManager, requestValues);
            }

            if (Enabled is false)
            {
                return true;
            }

            bool useCache = EnabledCache && requestValues.Any(requestValue => requestValue is not string) is false;
            string key = string.Empty;
            if (useCache)
            {
                key = string.Join("$$", requestValues);
                bool? cachedResult = await EnforceCache.TryGetResultAsync(requestValues, key);
                if (cachedResult.HasValue)
                {
#if !NET452
                    this.LogEnforceCachedResult(requestValues, cachedResult.Value);
#endif
                    return cachedResult.Value;
                }
            }

            context.HandleOptionAndCached = true;
            bool result = await InternalEnforceAsync(context, PolicyManager, requestValues);

            if (useCache)
            {
                await EnforceCache.TrySetResultAsync(requestValues, key, result);
            }
#if !NET452
            this.LogEnforceResult(context, requestValues, result);
#endif
            return result;
        }

        private Task<bool> InternalEnforceAsync(EnforceContext context, IPolicyManager policyManager, IReadOnlyList<object> requestValues)
        {
            bool CallInFunc() => InternalEnforce(in context, policyManager, requestValues);
            return policyManager.IsSynchronized ? Task.Run(CallInFunc) : Task.FromResult(CallInFunc());
        }

        private bool InternalEnforce(in EnforceContext context, IPolicyManager policyManager, IReadOnlyList<object> requestValues)
        {
            policyManager.StartRead();
            try
            {
                return InternalEnforce(in context, requestValues);
            }
            finally
            {
                policyManager.EndRead();
            }
        }

        private bool InternalEnforce(in EnforceContext context, IReadOnlyList<object> requestValues)
        {
            var session = new EnforceSession();
            var policyList = context.View.PolicyAssertion.Policy;
            session.RequestValues = requestValues;
            IChainEffector chainEffector = Effector as IChainEffector;
            session.IsChainEffector = chainEffector is not null;
            HandleInitialRequest(in context, ref session, chainEffector);
            if (session.PolicyCount != 0)
            {
                for (int policyIndex = 0; policyIndex < session.PolicyCount; policyIndex++)
                {
                    IReadOnlyList<string> policyValues = policyList[policyIndex];
                    session.PolicyValues = policyValues;
                    session.PolicyIndex = policyIndex;

                    HandleBeforeExpression(in context, ref session);
                    session.ExpressionResult = Model.ExpressionHandler.Invoke(in context, ref session);

                    if (session.IsChainEffector)
                    {
                        HandleExpressionResult(in context, ref session);
                    }
                    else
                    {
                        HandleExpressionResult(in context, ref session, Effector);
                    }

                    if (session.Determined)
                    {
                        break;
                    }
                }
            }
            else
            {
                HandleBeforeExpression(in context, ref session);
                session.ExpressionResult = Model.ExpressionHandler.Invoke(in context, ref session);

                if (session.IsChainEffector)
                {
                    HandleExpressionResult(in context, ref session);
                }
                else
                {
                    HandleExpressionResult(in context, ref session, Effector);
                }
            }
            return session.EnforceResult;
        }

        private static void HandleInitialRequest(in EnforceContext context, ref EnforceSession session, IChainEffector chainEffector)
        {
            session.ExpressionString = context.View.Matcher;
            session.PolicyCount = context.View.PolicyAssertion.Policy.Count;

            int requestTokenCount = context.View.RequestAssertion.Tokens.Count;
            if (requestTokenCount != session.RequestValues.Count)
            {
                throw new ArgumentException($"Invalid request size: expected {requestTokenCount}, got {session.RequestValues.Count}.");
            }
            
            if (session.IsChainEffector)
            {
                session.EffectChain = chainEffector.CreateChain(context.View.Effect, context.View.EffectExpressionType);
            }
            else
            {
                session.PolicyEffects = new PolicyEffect[session.PolicyCount];
            }
        }

        private static void HandleBeforeExpression(in EnforceContext context, ref EnforceSession session)
        {
            IEffectChain effectChain = session.EffectChain;
            int policyTokenCount = context.View.PolicyAssertion.Tokens.Count;

            if (session.PolicyCount is 0)
            {
                if (context.View.HasEval)
                {
                    throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                }

                session.PolicyValues = Enumerable.Repeat(string.Empty, policyTokenCount).ToArray();
                return;
            }

            if (policyTokenCount != session.PolicyValues.Count)
            {
                throw new ArgumentException($"Invalid policy size: expected {policyTokenCount}, got {session.PolicyValues.Count}.");
            }

            if (session.IsChainEffector is false && context.View.EffectExpressionType is EffectExpressionType.PriorityDenyOverride)
            {
                ThrowHelper.ThrowNotSupportException($"Only {nameof(IChainEffector)} support {nameof(EffectExpressionType.PriorityDenyOverride)} policy effect expression.");
            }

            if (context.View.HasPriority && context.View.EffectExpressionType is EffectExpressionType.PriorityDenyOverride)
            {
                if (int.TryParse(session.PolicyValues[context.View.PriorityIndex], out int nowPriority))
                {
                    if (session.Priority.HasValue && nowPriority != session.Priority.Value
                        && effectChain.HitPolicyCount > 0)
                    {
                        session.DetermineResult(effectChain.Result);
                    }
                    session.Priority = nowPriority;
                }
            }

            if (context.View.HasEval is false)
            {
                return;
            }

            session.ExpressionString = context.View.Matcher;
            session.ExpressionString = RewriteEval(session.ExpressionString, context.View.PolicyAssertion.Tokens, session.PolicyValues);
        }


        private static void HandleExpressionResult(in EnforceContext context, ref EnforceSession session, IEffector effector)
        {
            PolicyEffect nowEffect;
            if (session.PolicyCount is 0)
            {
                nowEffect = GetEffect(session.ExpressionResult);
                nowEffect = effector.MergeEffects(context.View.Effect, new[] { nowEffect }, null, session.PolicyIndex, session.PolicyCount, out _);
                bool finalResult = nowEffect.ToNullableBool() ?? false;
                session.DetermineResult(finalResult);
            }

            IReadOnlyList<string> policyValues = session.PolicyValues;
            nowEffect = GetEffect(session.ExpressionResult);

            if (nowEffect is not PolicyEffect.Indeterminate && context.View.PolicyAssertion.TryGetTokenIndex("eft", out int index))
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
            nowEffect = effector.MergeEffects(context.View.Effect, session.PolicyEffects, null, session.PolicyIndex, session.PolicyCount, out int hitPolicyIndex);

            if (context.Explain && hitPolicyIndex is not -1)
            {
                context.Explanations.Add(policyValues);
            }

            if (nowEffect is not PolicyEffect.Indeterminate)
            {
                session.DetermineResult(nowEffect.ToNullableBool() ?? false);
            }

            session.EnforceResult = false;
        }

        private static void HandleExpressionResult(in EnforceContext context, ref EnforceSession session)
        {
            IEffectChain effectChain = session.EffectChain;
            PolicyEffect nowEffect;
            if (session.PolicyCount is 0)
            {
                nowEffect = GetEffect(session.ExpressionResult);

                if (effectChain.TryChain(nowEffect))
                {
                    session.DetermineResult(effectChain.Result);
                }

                session.DetermineResult(false);
            }

            IReadOnlyList<string> policyValues = session.PolicyValues;
            nowEffect = GetEffect(session.ExpressionResult);

            if (nowEffect is not PolicyEffect.Indeterminate && context.View.PolicyAssertion.TryGetTokenIndex("eft", out int index))
            {
                string policyEffect = policyValues[index];
                nowEffect = policyEffect switch
                {
                    "allow" => PolicyEffect.Allow,
                    "deny" => PolicyEffect.Deny,
                    _ => PolicyEffect.Indeterminate
                };
            }

            bool chainResult = effectChain.TryChain(nowEffect);

            if (context.Explain && effectChain.HitPolicy)
            {
                context.Explanations.Add(policyValues);
            }

            if (chainResult is false || effectChain.CanChain is false)
            {
                session.DetermineResult(effectChain.Result);
            }

            session.EnforceResult = effectChain.Result;
        }

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
