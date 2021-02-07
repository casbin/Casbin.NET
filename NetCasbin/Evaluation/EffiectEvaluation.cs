using System;
using Casbin.Effect;

namespace Casbin.Evaluation
{
    internal static class EffectEvaluator
    {
        internal static bool TryEvaluate(PolicyEffect effect, EffectExpressionType policyEffectType, ref bool result)
        {
            switch (policyEffectType)
            {
                case EffectExpressionType.AllowOverride:
                {
                    result = false;
                    if (effect is PolicyEffect.Allow)
                    {
                        result = true;
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
                        return true;
                    }
                }
                break;

                case EffectExpressionType.AllowAndDeny:
                    switch (effect)
                    {
                        case PolicyEffect.Allow:
                            result = true;
                            return false;
                        case PolicyEffect.Deny:
                            result = false;
                            return true;
                    }
                    break;

                case EffectExpressionType.Priority:
                    switch (effect)
                    {
                        case PolicyEffect.Allow:
                            result = true;
                            return true;
                        case PolicyEffect.Deny:
                            result = false;
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
