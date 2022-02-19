using Casbin.Model;
using Casbin.Rbac;

namespace Casbin.Model
{
    public static class ModelExtension
    {
        public static IModel SetPolicyManager(this IModel model, IPolicyManager policyManager)
        {
            model.PolicyManager = policyManager;
            return model;
        }

        public static IModel ReplacePolicyManager(this IModel model, IPolicyManager policyManager)
        {
            model.SetPolicyManager(policyManager
                    .SetAdapter(model.PolicyManager.Adapter)
                    .SetPolicy(model.PolicyManager.PolicyStore));
            return model;
        }

        internal static IRoleManager GetRoleManger(this IModel model, string roleType = PermConstants.DefaultRoleType)
        {
            return model.Sections[PermConstants.Section.RoleSection][roleType].RoleManager;
        }

        internal static IReadOnlyAssertion GetRequiredAssertion(this IModel model, string section, string type)
        {
            return model.PolicyManager.PolicyStore.GetRequiredAssertion(section, type);
        }
    }
}
