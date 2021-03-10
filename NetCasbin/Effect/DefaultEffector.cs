using System;
using Casbin.Evaluation;

namespace Casbin.Effect
{
    /// <summary>
    /// DefaultEffector is default effector for Casbin.
    /// </summary>
    public class DefaultEffector : IEffector
    {
        public bool Result { get; private set; }

        public bool HitPolicy { get; private set; }

        public bool CanChain { get; private set; }

        public string EffectExpression { get; private set; }

        public EffectExpressionType PolicyEffectType { get; private set; }

        public void StartChain(string effectExpression)
        {
            EffectExpression = effectExpression ?? throw new ArgumentNullException(nameof(effectExpression));
            PolicyEffectType = ParseEffectExpressionType(EffectExpression);
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

        private static EffectExpressionType ParseEffectExpressionType(string effectExpression) => effectExpression switch
        {
            PermConstants.PolicyEffect.AllowOverride => EffectExpressionType.AllowOverride,
            PermConstants.PolicyEffect.DenyOverride => EffectExpressionType.DenyOverride,
            PermConstants.PolicyEffect.AllowAndDeny => EffectExpressionType.AllowAndDeny,
            PermConstants.PolicyEffect.Priority => EffectExpressionType.Priority,
            _ => throw new NotSupportedException("Not supported policy effect.")
        };
    }
}
