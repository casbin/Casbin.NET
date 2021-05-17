using System;
using NetCasbin.Model;
using NetCasbin.Rbac;

namespace NetCasbin.Extensions
{
    public static class ModelExtension
    {
        internal static IRoleManager GetRoleManger(this Model.Model model, string roleType = PermConstants.DefaultRoleType)
        {
            return model.GetExistAssertion(PermConstants.Section.RoleSection, roleType).RoleManager;
        }
    }
}
