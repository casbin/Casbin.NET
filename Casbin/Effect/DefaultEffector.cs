using System;
using System.Collections.Generic;
using Casbin.Evaluation;

namespace Casbin.Effect
{
    /// <summary>
    /// DefaultEffector is default effector for Casbin.
    /// </summary>
    public class DefaultEffector : IEffector, IChainEffector<EffectChain>
    {
        /// <summary>
        /// Merges all matching results collected by the enforcer into a single decision.
        /// </summary>
        /// <param name="effectExpression"></param>
        /// <param name="effects"></param>
        /// <param name="matches"></param>
        /// <param name="policyIndex"></param>
        /// <param name="policyCount"></param>
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
        /// <param name="policyCount"></param>
        /// <param name="hitPolicyIndex"></param>
        /// <param name="policyIndex"></param>
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

        public EffectChain CreateChain(string effectExpression) => new(effectExpression);

        public EffectChain CreateChain(string effectExpression, EffectExpressionType effectExpressionType) => new(effectExpression, effectExpressionType);
    }
}
