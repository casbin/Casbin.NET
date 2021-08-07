using System;
using System.Collections.Generic;
using Casbin.Evaluation;
using Casbin.Extensions;

namespace Casbin.Effect
{
    /// <summary>
    /// DefaultEffector is default effector for Casbin.
    /// </summary>
    public class DefaultEffector : IEffector, IChainEffector
    {
        /// <summary>
        /// Merges all matching results collected by the enforcer into a single decision.
        /// </summary>
        /// <param name="effectExpression"></param>
        /// <param name="effects"></param>
        /// <param name="matches"></param>
        /// <param name="hitPolicyIndex"></param>
        /// <returns></returns>
        public PolicyEffect MergeEffects(string effectExpression, IReadOnlyList<PolicyEffect> effects, IReadOnlyList<float> matches, int policyIndex, int policyCount, out int hitPolicyIndex)
        {
            EffectExpressionType effectExpressionType = ParseEffectExpressionType(effectExpression);
            bool? result = MergeEffects(effectExpressionType, effects, matches, policyIndex, policyCount, out hitPolicyIndex);
            return result.ToPolicyEffect();
        }

        /// <summary>
        /// Merges all matching results collected by the enforcer into a single decision.
        /// </summary>
        /// <param name="effectExpressionType"></param>
        /// <param name="effects"></param>
        /// <param name="matches"></param>
        /// <param name="hitPolicyIndex"></param>
        /// <returns></returns>
        private static bool? MergeEffects(EffectExpressionType effectExpressionType, IReadOnlyList<PolicyEffect> effects,
            IReadOnlyList<float> matches, int policyIndex, int policyCount, out int hitPolicyIndex)
        {
            hitPolicyIndex = -1;
            int effectCount = policyIndex + 1;
            bool finalResult = false;

            for (int index = 0; index < effectCount; index++)
            {
                if (EffectEvaluator.TryEvaluate(effects[index] , effectExpressionType,
                    ref finalResult, out bool hitPolicy))
                {
                    if (hitPolicy)
                    {
                        hitPolicyIndex = index;
                    }
                    return finalResult;
                }
            }

            if (effectCount == policyCount)
            {
                return finalResult;
            }
            return null;
        }

        public static EffectExpressionType ParseEffectExpressionType(string effectExpression) => effectExpression switch
        {
            PermConstants.PolicyEffect.AllowOverride => EffectExpressionType.AllowOverride,
            PermConstants.PolicyEffect.DenyOverride => EffectExpressionType.DenyOverride,
            PermConstants.PolicyEffect.AllowAndDeny => EffectExpressionType.AllowAndDeny,
            PermConstants.PolicyEffect.Priority => EffectExpressionType.Priority,
            PermConstants.PolicyEffect.PriorityDenyOverride => EffectExpressionType.PriorityDenyOverride,
            _ => throw new NotSupportedException("Not supported policy effect.")
        };

        #region IChainEffector

        public bool Result { get; private set; }

        public bool HitPolicy { get; private set; }

        public int HitPolicyCount { get; private set; }

        public bool CanChain { get; private set; }

        public string EffectExpression { get; private set; }

        public EffectExpressionType EffectExpressionType { get; private set; }

        public void StartChain(string effectExpression)
        {
            EffectExpression = effectExpression ?? throw new ArgumentNullException(nameof(effectExpression));
            EffectExpressionType = ParseEffectExpressionType(EffectExpression);
            CanChain = true;
            Result = false;
            HitPolicyCount = 0;
        }

        public bool Chain(PolicyEffect effect)
        {
            if (CanChain is false)
            {
                throw new InvalidOperationException();
            }

            bool result = Result;

            if (EffectEvaluator.TryEvaluate(effect, EffectExpressionType,
                ref result, out bool hitPolicy))
            {
                CanChain = false;
                Result = result;
                HitPolicy = hitPolicy;
                return true;
            }

            Result = result;
            HitPolicy = hitPolicy;
            return true;
        }

        
        public bool TryChain(PolicyEffect effect)
        {
            if (CanChain is false)
            {
                return false;
            }

            bool result = Result;
            if (EffectEvaluator.TryEvaluate(effect, EffectExpressionType,
                ref result, out bool hitPolicy))
            {
                CanChain = false;
                Result = result;
                HitPolicy = hitPolicy;
                if (hitPolicy)
                {
                    HitPolicyCount++;
                }
                return true;
            }

            Result = result;
            HitPolicy = hitPolicy;
            if (hitPolicy)
            {
                HitPolicyCount++;
            }
            return true;
        }

        public bool TryChain(PolicyEffect effect, out bool? result)
        {
            if (TryChain(effect))
            {
                result = Result;
                return true;
            }

            result = null;
            return false;
        }
        #endregion
    }
}
