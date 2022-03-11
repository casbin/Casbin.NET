using System;
using NetCasbin.Model;

namespace NetCasbin.Extensions
{
    public static class EnforcerExtension
    {
        public static Enforcer AddMatchingFunc(this Enforcer enforcer, Func<string, string, bool> func)
        {
            enforcer.AddNamedMatchingFunc(PermConstants.DefaultRoleType, func);
            return enforcer;
        }

        public static Enforcer AddDomainMatchingFunc(this Enforcer enforcer, Func<string, string, bool> func)
        {
            enforcer.AddNamedDomainMatchingFunc(PermConstants.DefaultRoleType, func);
            return enforcer;
        }

        public static Enforcer AddNamedMatchingFunc(this Enforcer enforcer, string roleType, Func<string, string, bool> func)
        {
            enforcer.GetModel().GetRoleManger(roleType).AddMatchingFunc(func);
            return enforcer;
        }

        public static Enforcer AddNamedDomainMatchingFunc(this Enforcer enforcer, string roleType,  Func<string, string, bool> func)
        {
            enforcer.GetModel().GetRoleManger(roleType).AddDomainMatchingFunc(func);
            return enforcer;
        }
    }
}
