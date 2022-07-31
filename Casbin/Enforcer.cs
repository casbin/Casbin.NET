using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Casbin.Adapter.File;
using Casbin.Caching;
using Casbin.Effect;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Util;
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

            model.SortPolicy();
        }

        #region Options

        public bool Enabled { get; set; } = true;
        public bool EnabledCache { get; set; } = true;

        public bool AutoBuildRoleLinks { get; set; } = true;
        public bool AutoNotifyWatcher { get; set; } = true;
        public bool AutoCleanEnforceCache { get; set; } = true;

        #endregion

        #region Extensions

        public IEffector Effector { get; set; } = new DefaultEffector();
        public IWatcher Watcher { get; set; }
        public IModel Model { get; set; }

        public IReadOnlyAdapter Adapter
        {
            get => Model.AdapterHolder.Adapter;
            set => Model.AdapterHolder.Adapter = value;
        }

        public IEnforceCache EnforceCache
        {
            get => Model.EnforceCache;
            set => Model.EnforceCache = value;
        }

#if !NET452
        public ILogger Logger { get; set; }
#endif

        #endregion

        #region Enforce method

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="context">Enforce context include all status on enforcing</param>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings,
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public bool Enforce<TRequest>(EnforceContext context, TRequest requestValues) where TRequest : IRequestValues
        {
            if (context.HandleOptionAndCached)
            {
                return InternalEnforce(in context, in requestValues);
            }

            if (Enabled is false)
            {
                return true;
            }

            if (EnabledCache)
            {
                if (EnforceCache.TryGetResult(requestValues, out bool cachedResult))
                {
#if !NET452
                    this.LogEnforceCachedResult(requestValues, cachedResult);
#endif
                    return cachedResult;
                }
            }

            bool result = InternalEnforce(in context, in requestValues);

            if (EnabledCache)
            {
                EnforceCache.TrySetResult(requestValues, result);
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
        public async Task<bool> EnforceAsync<TRequest>(EnforceContext context, TRequest requestValues)
            where TRequest : IRequestValues
        {
            if (context.HandleOptionAndCached)
            {
                return await InternalEnforceAsync(context, requestValues);
            }

            if (Enabled is false)
            {
                return true;
            }

            if (EnabledCache)
            {
                bool? cachedResult = await EnforceCache.TryGetResultAsync(requestValues);
                if (cachedResult.HasValue)
                {
#if !NET452
                    this.LogEnforceCachedResult(requestValues, cachedResult.Value);
#endif
                    return cachedResult.Value;
                }
            }

            context.HandleOptionAndCached = true;
            bool result = await InternalEnforceAsync(context, requestValues);

            if (EnabledCache)
            {
                await EnforceCache.TrySetResultAsync(requestValues, result);
            }
#if !NET452
            this.LogEnforceResult(context, requestValues, result);
#endif
            return result;
        }

        private Task<bool> InternalEnforceAsync<TRequest>(EnforceContext context, TRequest requestValues)
            where TRequest : IRequestValues
        {
            bool CallInFunc()
            {
                return InternalEnforce(in context, in requestValues);
            }

            return Task.Run(CallInFunc);
        }

        private bool InternalEnforce<TRequest>(in EnforceContext context, in TRequest requestValues)
            where TRequest : IRequestValues
        {
            if (context.View.SupportGeneric is false)
            {
                return InternalEnforce<IRequestValues, IPolicyValues>(in context, requestValues);
            }

            return context.View.PolicyTokens.Count switch
            {
                1 => InternalEnforce<TRequest, PolicyValues<string>>(in context, requestValues),
                2 => InternalEnforce<TRequest, PolicyValues<string, string>>(in context, requestValues),
                3 => InternalEnforce<TRequest, PolicyValues<string, string, string>>(in context, requestValues),
                4 => InternalEnforce<TRequest, PolicyValues<string, string, string, string>>(in context,
                    requestValues),
                5 => InternalEnforce<TRequest, PolicyValues<string, string, string, string, string>>(in context,
                    requestValues),
                6 => InternalEnforce<TRequest, PolicyValues<string, string, string, string, string, string>>(
                    in context,
                    requestValues),
                7 => InternalEnforce<TRequest,
                    PolicyValues<string, string, string, string, string, string, string>>(
                    in context, requestValues),
                8 => InternalEnforce<TRequest,
                    PolicyValues<string, string, string, string, string, string, string, string>>(in context,
                    requestValues),
                9 => InternalEnforce<TRequest,
                    PolicyValues<string, string, string, string, string, string, string, string, string>>(
                    in context,
                    requestValues),
                10 => InternalEnforce<TRequest, PolicyValues<string, string, string, string, string, string, string,
                    string, string, string>>(in context, requestValues),
                11 => InternalEnforce<TRequest, PolicyValues<string, string, string, string, string, string, string,
                    string, string, string, string>>(in context, requestValues),
                12 => InternalEnforce<TRequest, PolicyValues<string, string, string, string, string, string, string,
                    string, string, string, string, string>>(in context, requestValues),
                _ => InternalEnforce<IRequestValues, IPolicyValues>(in context, requestValues)
            };
        }

        private bool InternalEnforce<TRequest, TPolicy>(in EnforceContext context, in TRequest requestValues)
            where TRequest : IRequestValues
            where TPolicy : IPolicyValues
        {
            var session = new EnforceSession();
            var expressionHandler = Model.ExpressionHandler;
            PolicyScanner<TRequest> scanner = context.View.PolicyAssertion.Scan(in requestValues);

            EffectChain effectChain = default;
            if (Effector is IChainEffector<EffectChain> effector)
            {
                session.IsChainEffector = true;
                effectChain = effector.CreateChain(context.View.Effect, context.View.EffectExpressionType);
            }

            session.HasNextPolicy = scanner.HasNext();
            HandleInitialRequest(in context, ref session, in requestValues);

            if (context.View.HasPolicyParameter && session.HasNextPolicy)
            {
                int policyIndex = 0;
                while (scanner.GetNext(out IPolicyValues outValues))
                {
                    TPolicy policyValues = (TPolicy)outValues;
                    session.PolicyIndex = policyIndex;

                    HandleBeforeExpression(in context, ref session, in requestValues, in policyValues, ref effectChain);
                    session.ExpressionResult = expressionHandler.Invoke(in context, session.ExpressionString,
                        in requestValues, in policyValues);

                    if (session.IsChainEffector)
                    {
                        HandleExpressionResult(in context, ref session, in requestValues, in policyValues,
                            ref effectChain);
                    }
                    else
                    {
                        HandleExpressionResult(in context, ref session, in requestValues, in policyValues, Effector);
                    }

                    if (session.Determined)
                    {
                        scanner.Interrupt();
                        break;
                    }

                    policyIndex++;
                }
            }
            else
            {
                StringPolicyValues policyValues = StringPolicyValues.Empty;
                HandleBeforeExpression(in context, ref session, in requestValues, in policyValues, ref effectChain);
                session.ExpressionResult = expressionHandler.Invoke(in context, session.ExpressionString,
                    in requestValues, in policyValues);

                if (session.IsChainEffector)
                {
                    HandleExpressionResult(in context, ref session, in requestValues, in policyValues, ref effectChain);
                }
                else
                {
                    HandleExpressionResult(in context, ref session, in requestValues, in policyValues, Effector);
                }
            }

            return session.EnforceResult;
        }

        private static void HandleInitialRequest<TRequest>(
            in EnforceContext context, ref EnforceSession session, in TRequest request)
            where TRequest : IRequestValues
        {
            session.ExpressionString = context.View.HasEval ? context.View.Matcher : context.View.TransformedMatcher;

            int requestTokenCount = context.View.RequestAssertion.Tokens.Count;
            if (requestTokenCount > request.Count)
            {
                throw new ArgumentException(
                    $"Invalid request size: expected {requestTokenCount} at least, got {request.Count}.");
            }

            if (session.IsChainEffector is false)
            {
                session.PolicyEffects = new List<PolicyEffect>();
            }
        }

        private static void HandleBeforeExpression<TRequest, TPolicy, TChain>(
            in EnforceContext context, ref EnforceSession session,
            in TRequest request, in TPolicy policy, ref TChain effectChain)
            where TRequest : IRequestValues
            where TPolicy : IPolicyValues
            where TChain : IEffectChain
        {
            int policyTokenCount = context.View.PolicyAssertion.Tokens.Count;

            if (session.HasNextPolicy is false)
            {
                if (context.View.HasEval)
                {
                    throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
                }

                return;
            }

            if (policyTokenCount > policy.Count)
            {
                throw new ArgumentException(
                    $"Invalid policy size: expected {policyTokenCount} at least, got {policy.Count}.");
            }

            if (session.IsChainEffector is false &&
                context.View.EffectExpressionType is EffectExpressionType.PriorityDenyOverride)
            {
                ThrowHelper.ThrowNotSupportException(
                    $"Only IChainEffector<T> support {nameof(EffectExpressionType.PriorityDenyOverride)} policy effect expression.");
            }

            if (context.View.HasPriority &&
                context.View.EffectExpressionType is EffectExpressionType.PriorityDenyOverride)
            {
                if (int.TryParse(policy[context.View.PriorityIndex], out int nowPriority))
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
            session.ExpressionString = RewriteEval(in context, session.ExpressionString, policy);
            session.ExpressionString = EnforceView.TransformMatcher(context.View, session.ExpressionString);
        }

        private static void HandleExpressionResult<TRequest, TPolicy>(
            in EnforceContext context, ref EnforceSession session,
            in TRequest request, in TPolicy policy, IEffector effector)
            where TRequest : IRequestValues
            where TPolicy : IPolicyValues
        {
            PolicyEffect nowEffect;
            if (session.HasNextPolicy is false)
            {
                nowEffect = GetEffect(session.ExpressionResult);
                nowEffect = effector.MergeEffects(context.View.Effect, new[] { nowEffect }, null, 0,
                    0, out _);
                bool finalResult = nowEffect.ToNullableBool() ?? false;
                session.DetermineResult(finalResult);
            }

            nowEffect = GetEffect(session.ExpressionResult);

            if (nowEffect is not PolicyEffect.Indeterminate && context.View.HasEffect)
            {
                string policyEffect = policy[context.View.EffectIndex];
                nowEffect = policyEffect switch
                {
                    "allow" => PolicyEffect.Allow,
                    "deny" => PolicyEffect.Deny,
                    _ => PolicyEffect.Indeterminate
                };
            }

            session.PolicyEffects[session.PolicyIndex] = nowEffect;

            int policyCount = session.HasNextPolicy ? session.PolicyIndex + 1 : session.PolicyIndex;
            nowEffect = effector.MergeEffects(context.View.Effect, session.PolicyEffects, null, session.PolicyIndex,
                policyCount, out int hitPolicyIndex);

            if (context.Explain && hitPolicyIndex is not -1)
            {
                context.Explanations.Add(policy);
            }

            if (nowEffect is not PolicyEffect.Indeterminate)
            {
                session.DetermineResult(nowEffect.ToNullableBool() ?? false);
            }

            session.EnforceResult = false;
        }

        private static void HandleExpressionResult<TRequest, TPolicy, TChain>(
            in EnforceContext context, ref EnforceSession session,
            in TRequest request, in TPolicy policy, ref TChain effectChain)
            where TRequest : IRequestValues
            where TPolicy : IPolicyValues
            where TChain : IEffectChain
        {
            PolicyEffect nowEffect;
            if (session.HasNextPolicy is false)
            {
                nowEffect = GetEffect(session.ExpressionResult);
                if (effectChain.TryChain(nowEffect))
                {
                    session.DetermineResult(effectChain.Result);
                }

                session.DetermineResult(false);
            }

            nowEffect = GetEffect(session.ExpressionResult);

            if (nowEffect is not PolicyEffect.Indeterminate && context.View.HasEffect)
            {
                string policyEffect = policy[context.View.EffectIndex];
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
                context.Explanations.Add(policy);
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

        private static string RewriteEval(in EnforceContext context, string expressionString,
            IPolicyValues policyValues)
        {
            if (context.View.EvalRules is null)
            {
                return expressionString;
            }

            Dictionary<string, string> rules = new();
            foreach (KeyValuePair<string, int> rule in context.View.EvalRules)
            {
                rules[rule.Key] = policyValues[rule.Value];
            }

            expressionString = StringUtil.ReplaceEval(expressionString, rules);
            return expressionString;
        }

        #endregion
    }
}
