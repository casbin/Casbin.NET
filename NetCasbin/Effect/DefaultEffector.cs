using System;
using Casbin.Evaluation;

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
        /// <param name="results"></param>
        /// <param name="hitPolicyIndex"></param>
        /// <returns></returns>
        public bool MergeEffects(string effectExpression, PolicyEffect[] effects, float[] results, out int hitPolicyIndex)
        {
            return MergeEffects(effectExpression, effects.AsSpan(), results.AsSpan(), out hitPolicyIndex);
        }


        /// <summary>
        /// Merges all matching results collected by the enforcer into a single decision.
        /// </summary>
        /// <param name="effectExpression"></param>
        /// <param name="effects"></param>
        /// <param name="results"></param>
        /// <param name="hitPolicyIndex"></param>
        /// <returns></returns>
        private bool MergeEffects(string effectExpression, Span<PolicyEffect> effects, Span<float> results, out int hitPolicyIndex)
        {
            PolicyEffectType = ParsePolicyEffectType(effectExpression);
            return MergeEffects(PolicyEffectType, effects, results, out hitPolicyIndex);
        }

        /// <summary>
        /// Merges all matching results collected by the enforcer into a single decision.
        /// </summary>
        /// <param name="effectExpressionType"></param>
        /// <param name="effects"></param>
        /// <param name="results"></param>
        /// <param name="hitPolicyIndex"></param>
        /// <returns></returns>
        private bool MergeEffects(EffectExpressionType effectExpressionType, Span<PolicyEffect> effects, Span<float> results, out int hitPolicyIndex)
        {
            bool finalResult = false;
            hitPolicyIndex = -1;
            for (int index = 0; index < effects.Length; index++)
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

            return finalResult;
        }

        public static EffectExpressionType ParsePolicyEffectType(string effectExpression) => effectExpression switch
        {
            PermConstants.PolicyEffect.AllowOverride => EffectExpressionType.AllowOverride,
            PermConstants.PolicyEffect.DenyOverride => EffectExpressionType.DenyOverride,
            PermConstants.PolicyEffect.AllowAndDeny => EffectExpressionType.AllowAndDeny,
            PermConstants.PolicyEffect.Priority => EffectExpressionType.Priority,
            _ => throw new NotSupportedException("Not supported policy effect.")
        };

        #region IChainEffector

        public bool Result { get; private set; }

        public bool HitPolicy { get; private set; }

        public bool CanChain { get; private set; }

        public string EffectExpression { get; private set; }

        public EffectExpressionType PolicyEffectType { get; private set; }

        public void StartChain(string effectExpression)
        {
            EffectExpression = effectExpression ?? throw new ArgumentNullException(nameof(effectExpression));
            PolicyEffectType = ParsePolicyEffectType(EffectExpression);
            CanChain = true;
            Result = false;
        }

        public bool Chain(PolicyEffect effect)
        {
            if (CanChain is false)
            {
                throw new InvalidOperationException();
            }

            bool result = Result;

            if (EffectEvaluator.TryEvaluate(effect, PolicyEffectType,
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
            if (EffectEvaluator.TryEvaluate(effect, PolicyEffectType,
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
