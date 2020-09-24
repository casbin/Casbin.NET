using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using NetCasbin.Abstractions;
using NetCasbin.Evaluation;
using NetCasbin.Model;

namespace NetCasbin.Effect
{
    /// <summary>
    /// DefaultEffector is default effector for Casbin.
    /// </summary>
    public class DefaultEffector : IEffector, IChainEffector
    {
        /// <summary>
        /// Merges all matching results collected by the enforcer into a single decision.
        /// </summary>
        /// <param name="policyEffect"></param>
        /// <param name="effects"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public bool MergeEffects(string policyEffect, Effect[] effects, float[] results)
        {
            return MergeEffects(policyEffect, effects.AsSpan(), results.AsSpan());
        }


        /// <summary>
        /// Merges all matching results collected by the enforcer into a single decision.
        /// </summary>
        /// <param name="policyEffect"></param>
        /// <param name="effects"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        private bool MergeEffects(string policyEffect, Span<Effect> effects, Span<float> results)
        {
            PolicyEffectType = ParsePolicyEffectType(policyEffect);
            return MergeEffects(PolicyEffectType, effects, results);
        }

        /// <summary>
        /// Merges all matching results collected by the enforcer into a single decision.
        /// </summary>
        /// <param name="policyEffectType"></param>
        /// <param name="effects"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        private bool MergeEffects(PolicyEffectType policyEffectType, Span<Effect> effects, Span<float> results)
        {
            bool finalResult = false;
            foreach (var effect in effects)
            {
                if (EffectEvaluator.TryEvaluate(effect, policyEffectType, ref finalResult))
                {
                    return finalResult;
                }
            }
            return finalResult;
        }

        public static PolicyEffectType ParsePolicyEffectType(string policyEffect) => policyEffect switch
        {
            PermConstants.PolicyEffect.AllowOverride => PolicyEffectType.AllowOverride,
            PermConstants.PolicyEffect.DenyOverride => PolicyEffectType.DenyOverride,
            PermConstants.PolicyEffect.AllowAndDeny => PolicyEffectType.AllowAndDeny,
            PermConstants.PolicyEffect.Priority => PolicyEffectType.Priority,
            _ => throw new NotSupportedException("Not supported policy effect.")
        };

        #region IChainEffector

        public bool Result { get; private set; }

        public bool CanChain { get; private set; }

        public string EffectExpression { get; private set; }

        public PolicyEffectType PolicyEffectType { get; private set; }

        public void StartChain(string effectExpression)
        {
            EffectExpression = effectExpression ?? throw new ArgumentNullException(nameof(effectExpression));
            PolicyEffectType = ParsePolicyEffectType(EffectExpression);
            CanChain = true;
            Result = false;
        }

        public bool Chain(Effect effect)
        {
            if (CanChain is false)
            {
                throw new InvalidOperationException();
            }

            bool result = Result;

            if (EffectEvaluator.TryEvaluate(effect, PolicyEffectType, ref result))
            {
                CanChain = false;
                Result = result;
                return true;
            }

            Result = result;
            return true;
        }

        
        public bool TryChain(Effect effect)
        {
            if (CanChain is false)
            {
                return false;
            }

            bool result = Result;
            if (EffectEvaluator.TryEvaluate(effect, PolicyEffectType, ref result))
            {
                CanChain = false;
                Result = result;
                return true;
            }

            Result = result;
            return true;
        }

        public bool TryChain(Effect effect, out bool? result)
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
