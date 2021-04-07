using System;
using NetCasbin.Model;
using NetCasbin.Rbac;

namespace NetCasbin.Extensions
{
    public static class ModelExtension
    {
        internal static IRoleManager GetRoleManger(this Model.Model model)
        {
            return model.GetNamedRoleManger(PermConstants.DefaultRoleType);
        }

        internal static IRoleManager GetNamedRoleManger(this Model.Model model, string roleType)
        {
            return model.GetExistAssertion(PermConstants.Section.RoleSection, roleType).RoleManager;
        }
    }
}
