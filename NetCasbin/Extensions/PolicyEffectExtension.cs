using Casbin.Effect;

namespace Casbin.Extensions
{
    internal static class PolicyEffectExtension
    {
        internal static bool? ToNullableBool(this PolicyEffect policyEffect)
        {
            if (policyEffect is PolicyEffect.Allow)
            {
                return true;
            }

            if (policyEffect is PolicyEffect.Deny)
            {
                return false;
            }

            return null;
        }

        internal static PolicyEffect ToPolicyEffect(this bool? result)
        {
            if (result is true)
            {
                return PolicyEffect.Allow;
            }

            if (result is false)
            {
                return PolicyEffect.Deny;
            }

            return PolicyEffect.Indeterminate;
        }
    }
}
