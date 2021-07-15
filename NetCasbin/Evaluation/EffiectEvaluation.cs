using System;
using NetCasbin.Effect;

namespace NetCasbin.Evaluation
{
    internal static class EffectEvaluator
    {
        internal static bool TryEvaluate(Effect.Effect effect, PolicyEffectType policyEffectType,
            ref bool result, out bool hitPolicy)
        {
            hitPolicy = false;

            switch (policyEffectType)
            {
                case PolicyEffectType.AllowOverride:
                {
                    result = false;
                    if (effect is Effect.Effect.Allow)
                    {
                        result = true;
                        hitPolicy = true;
                        return true;
                    }
                }
                break;

                case PolicyEffectType.DenyOverride:
                {
                    result = true;
                    if (effect is Effect.Effect.Deny)
                    {
                        result = false;
                        hitPolicy = true;
                        return true;
                    }
                }
                break;

                case PolicyEffectType.AllowAndDeny:
                    switch (effect)
                    {
                        case Effect.Effect.Allow:
                            result = true;
                            hitPolicy = true;
                            return false;
                        case Effect.Effect.Deny:
                            result = false;
                            hitPolicy = true;
                            return true;
                    }
                    break;

                case PolicyEffectType.Priority:
                    switch (effect)
                    {
                        case Effect.Effect.Allow:
                            result = true;
                            hitPolicy = true;
                            return true;
                        case Effect.Effect.Deny:
                            result = false;
                            hitPolicy = true;
                            return true;
                    }
                    break;

                case PolicyEffectType.PriorityDenyOverride:
                    switch (effect)
                    {
                        case Effect.Effect.Allow:
                            result = true;
                            hitPolicy = true;
                            return false; 
                        case Effect.Effect.Deny:
                            result = false;
                            hitPolicy = true;
                            return true;
                    }
                    break;

                case PolicyEffectType.Custom:
                    // TODO: Support custom policy effect.
                    break;

                default:
                    throw new NotSupportedException("Not supported policy effect type.");
            }

            return false;
        }

    }
}
