using Casbin.Model;
using Casbin.Persist;

namespace Casbin.Extensions
{
    public static class PolicyManagerExtension
    {
        public static IPolicyManager SetAdapter(this IPolicyManager policyManager, IAdapter adapter)
        {
            policyManager.Adapter = adapter;
            return policyManager;
        }

        public static IPolicyManager SetPolicy(this IPolicyManager policyManager, IPolicy policy)
        {
            policyManager.Policy = policy;
            return policyManager;
        }
    }
}
