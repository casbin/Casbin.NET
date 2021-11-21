using System;
using Casbin.Evaluation;

namespace Casbin.Effect
{
    public struct EffectChain : IEffectChain
    {
        public EffectChain(string effectExpression)
        {
            EffectExpression = effectExpression ?? throw new ArgumentNullException(nameof(effectExpression));
            EffectExpressionType = DefaultEffector.ParseEffectExpressionType(EffectExpression);

            CanChain = true;
            Result = false;

            HitPolicy = false;
            HitPolicyCount = 0;
        }

        public EffectChain(string effectExpression, EffectExpressionType effectExpressionType)
        {
            EffectExpression = effectExpression;
            EffectExpressionType = effectExpressionType;

            CanChain = true;
            Result = false;

            HitPolicy = false;
            HitPolicyCount = 0;
        }

        public bool Result { get; private set; }

        public bool HitPolicy { get; private set; }

        public int HitPolicyCount { get; private set; }

        public bool CanChain { get; private set; }

        public string EffectExpression { get; }

        public EffectExpressionType EffectExpressionType { get; }

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
    }
}
