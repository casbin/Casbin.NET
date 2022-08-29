using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Casbin.Adapter.File;
using Casbin.Caching;
using Casbin.Effect;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Rbac;
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

            Model.SortPoliciesByPriority();
            Model.SortPoliciesBySubjectHierarchy();
        }

        public bool IsSynchronized => Model?.IsSynchronized ?? false;
        public string ModelPath => Model?.ModelPath;
        public bool IsFiltered => Adapter is IFilteredAdapter { IsFiltered: true };

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
        public IEnforceCache EnforceCache { get; set; } = new EnforceCache(new EnforceCacheOptions());
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
            var policyManager = PolicyManager;
            if (context.HandleOptionAndCached)
            {
                return InternalEnforce(in context, in policyManager, in requestValues);
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

            bool result = InternalEnforce(in context, in policyManager, in requestValues);

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
                return await InternalEnforceAsync(context, PolicyManager, requestValues);
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
            bool result = await InternalEnforceAsync(context, PolicyManager, requestValues);

            if (EnabledCache)
            {
                await EnforceCache.TrySetResultAsync(requestValues, result);
            }
#if !NET452
            this.LogEnforceResult(context, requestValues, result);
#endif
            return result;
        }

        /// <summary>
        /// Decides whether some "subject" can access corresponding "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="context">Enforce context include all status on enforcing</param>
        /// <param name="requestValues">The requests needs to be mediated, whose element is usually an array of strings
        /// but can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the requests.</returns>
        public IEnumerable<bool> BatchEnforce<TRequest>(EnforceContext context, 
            IEnumerable<TRequest> requestValues) where TRequest : IRequestValues
        {
            foreach(var requestValue in requestValues){
                yield return this.Enforce(context, requestValue);
            }
        }

        /// <summary>
        /// Decides whether some "subject" can access corresponding "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act). The method uses multi-thread
        /// to accelerate the process.
        /// </summary>
        /// <param name="context">Enforce context include all status on enforcing</param>
        /// <param name="requestValues">The requests needs to be mediated, whose element is usually an array of strings
        /// but can be class instances if ABAC is used.</param>
        /// <param name="maxDegreeOfParallelism">The max degree of parallelism of the process.</param>
        /// <returns>Whether to allow the requests.</returns>
        public IEnumerable<bool> ParallelBatchEnforce<TRequest>(EnforceContext context, 
            IReadOnlyList<TRequest> requestValues, int maxDegreeOfParallelism = -1) where TRequest : IRequestValues
        {
            int n = requestValues.Count;
            if(n == 0) return new bool[] { };
            bool[] res = new bool[n]; 
            Parallel.For(0, n, new ParallelOptions() { MaxDegreeOfParallelism = maxDegreeOfParallelism }, Index =>
            {
                res[Index] = this.Enforce(context, requestValues[Index]);
            });
            return res;
        }

        /// <summary>
        /// Decides whether some "subject" can access corresponding "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="context">Enforce context include all status on enforcing</param>
        /// <param name="requestValues">The requests needs to be mediated, whose element is usually an array of strings
        /// but can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the requests.</returns>
#if !NET452
        public async IAsyncEnumerable<bool> BatchEnforceAsync<TRequest>(EnforceContext context, IEnumerable<TRequest> requestValues) 
            where TRequest : IRequestValues
        {
            foreach(var requestValue in requestValues){
                bool res = await this.EnforceAsync(context, requestValue);
                yield return res;
            }
        }
#else
        public async Task<IEnumerable<bool>> BatchEnforceAsync<TRequest>(EnforceContext context, IEnumerable<TRequest> requestValues) 
            where TRequest : IRequestValues
        {
            List<bool> res = new List<bool>();
            foreach(var requestValue in requestValues){
                res.Add(await this.EnforceAsync(context, requestValue));
            }
            return res;
        }
#endif

        private Task<bool> InternalEnforceAsync<TRequest>(EnforceContext context, IPolicyManager policyManager,
            TRequest requestValues) where TRequest : IRequestValues
        {
            bool CallInFunc() => InternalEnforce(in context, in policyManager, in requestValues);
            return policyManager.IsSynchronized ? Task.Run(CallInFunc) : Task.FromResult(CallInFunc());
        }

        private bool InternalEnforce<TRequest>(in EnforceContext context, in IPolicyManager policyManager,
            in TRequest requestValues) where TRequest : IRequestValues
        {
            policyManager.StartRead();
            try
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
            finally
            {
                policyManager.EndRead();
            }
        }

        private bool InternalEnforce<TRequest, TPolicy>(in EnforceContext context, in TRequest requestValues)
            where TRequest : IRequestValues
            where TPolicy : IPolicyValues
        {
            var session = new EnforceSession();
            var expressionHandler = Model.ExpressionHandler;
            var policyList = context.View.PolicyAssertion.Policy;

            EffectChain effectChain = default;
            if (Effector is IChainEffector<EffectChain> effector)
            {
                session.IsChainEffector = true;
                effectChain = effector.CreateChain(context.View.Effect, context.View.EffectExpressionType);
            }

            HandleInitialRequest(in context, ref session, in requestValues);

            if (session.PolicyCount != 0 && context.View.HasPolicyParameter)
            {
                for (int policyIndex = 0; policyIndex < session.PolicyCount; policyIndex++)
                {
                    var policyValues = (TPolicy)policyList[policyIndex];
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
                        break;
                    }
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
            session.PolicyCount = context.View.PolicyAssertion.Policy.Count;

            int requestTokenCount = context.View.RequestAssertion.Tokens.Count;
            if (requestTokenCount > request.Count)
            {
                throw new ArgumentException(
                    $"Invalid request size: expected {requestTokenCount} at least, got {request.Count}.");
            }

            if (session.IsChainEffector is false)
            {
                session.PolicyEffects = new PolicyEffect[session.PolicyCount];
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

            if (session.PolicyCount is 0)
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
            if (session.PolicyCount is 0)
            {
                nowEffect = GetEffect(session.ExpressionResult);
                nowEffect = effector.MergeEffects(context.View.Effect, new[] { nowEffect }, null, session.PolicyIndex,
                    session.PolicyCount, out _);
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
            nowEffect = effector.MergeEffects(context.View.Effect, session.PolicyEffects, null, session.PolicyIndex,
                session.PolicyCount, out int hitPolicyIndex);

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
            if (session.PolicyCount is 0)
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
