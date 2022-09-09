﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Casbin.Effect;
using Casbin.Evaluation;
using Casbin.Model;
using Casbin.Util;

namespace Casbin;

public partial class Enforcer
{
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
        EnforceSession session = new EnforceSession();
        IExpressionHandler expressionHandler = Model.ExpressionHandler;
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
    private static PolicyEffect GetEffect(bool expressionResult) =>
        expressionResult ? PolicyEffect.Allow : PolicyEffect.Indeterminate;

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
}
