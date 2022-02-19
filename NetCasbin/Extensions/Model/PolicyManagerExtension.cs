using Casbin.Persist;

namespace Casbin.Model
{
    public static class PolicyManagerExtension
    {
        internal static IPolicyManager SetAdapter(this IPolicyManager policyManager, IReadOnlyAdapter adapter)
        {
            policyManager.Adapter = adapter;
            return policyManager;
        }

        internal static IPolicyManager SetPolicy(this IPolicyManager policyManager, IPolicyStore policyStore)
        {
            policyManager.PolicyStore = policyStore;
            return policyManager;
        }
    }
}
