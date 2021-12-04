using Casbin.Model;
using Casbin.Persist;

namespace Casbin
{
    public static class PolicyManagerExtension
    {
        internal static IPolicyManager SetAdapter(this IPolicyManager policyManager, IReadOnlyAdapter adapter)
        {
            policyManager.Adapter = adapter;
            return policyManager;
        }

        internal static IPolicyManager SetPolicy(this IPolicyManager policyManager, IPolicy policy)
        {
            policyManager.Policy = policy;
            return policyManager;
        }
    }
}
