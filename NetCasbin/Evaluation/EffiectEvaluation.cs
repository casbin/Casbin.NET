using System;
using Casbin.Effect;

namespace Casbin.Evaluation
{
    internal static class EffectEvaluator
    {
        internal static bool TryEvaluate(PolicyEffect effect, EffectExpressionType effectExpressionType,
            ref bool result, out bool hitPolicy)
        {
            hitPolicy = false;

            switch (effectExpressionType)
            {
                case EffectExpressionType.AllowOverride:
                {
                    result = false;
                    if (effect is PolicyEffect.Allow)
                    {
                        result = true;
                        hitPolicy = true;
                        return true;
                    }
                }
                break;

                case EffectExpressionType.DenyOverride:
                {
                    result = true;
                    if (effect is PolicyEffect.Deny)
                    {
                        result = false;
                        hitPolicy = true;
                        return true;
                    }
                }
                break;

                case EffectExpressionType.AllowAndDeny:
                    switch (effect)
                    {
                        case PolicyEffect.Allow:
                            result = true;
                            hitPolicy = true;
                            return false;
                        case PolicyEffect.Deny:
                            result = false;
                            hitPolicy = true;
                            return true;
                    }
                    break;

                case EffectExpressionType.Priority:
                    switch (effect)
                    {
                        case PolicyEffect.Allow:
                            result = true;
                            hitPolicy = true;
                            return true;
                        case PolicyEffect.Deny:
                            result = false;
                            hitPolicy = true;
                            return true;
                    }
                    break;

                case EffectExpressionType.Custom:
                    // TODO: Support custom policy effect.
                    break;

                default:
                    throw new NotSupportedException("Not supported policy effect type.");
            }

            return false;
        }

    }
}
