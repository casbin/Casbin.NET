using System.Collections.Generic;
using System.Linq;
using NetCasbin.Util;
using Xunit;

namespace NetCasbin.UnitTest
{
    public  class TestUtil
    {

        internal List<T> AsList<T>(params T[] values)
        {
            return values.ToList();
        }
        internal List<string> AsList(params string[] values)
        {
            return values.ToList();
        }

        internal static void TestEnforce(Enforcer e, string sub, object obj, string act, bool res)
        { 
            Assert.Equal(res, e.Enforce(sub, obj, act));
        }

        internal static void TestEnforceWithoutUsers(Enforcer e, string obj, string act, bool res)
        {
            Assert.Equal(res, e.Enforce(obj, act));
        }

        internal static void TestDomainEnforce(Enforcer e, string sub, string dom, string obj, string act, bool res)
        {
            Assert.Equal(res, e.Enforce(sub, dom, obj, act));
        }

        internal static void TestGetPolicy(Enforcer e, List<List<string>> res)
        {
            var myRes = e.GetPolicy();
             Assert.True(Utility.Array2DEquals(res, myRes));
        }

        internal static void TestGetFilteredPolicy(Enforcer e, int fieldIndex, List<List<string>> res, params string[] fieldValues)
        {
            var myRes = e.GetFilteredPolicy(fieldIndex, fieldValues);

            Assert.True(Utility.Array2DEquals(res, myRes));
        }

        internal static void TestGetGroupingPolicy(Enforcer e, List<List<string>> res)
        {
            var myRes = e.GetGroupingPolicy(); 
            Assert.Equal(res, myRes);
        }

        internal static void TestGetFilteredGroupingPolicy(Enforcer e, int fieldIndex, List<List<string>> res, params string[] fieldValues)
        {
            var myRes = e.GetFilteredGroupingPolicy(fieldIndex, fieldValues);
            Assert.Equal(res, myRes);
        }

        internal static void TestHasPolicy(Enforcer e, List<string> policy, bool res)
        {
            var myRes = e.HasPolicy(policy); 
            Assert.Equal(res, myRes);
        }

        internal static void TestHasGroupingPolicy(Enforcer e, List<string> policy, bool res)
        {
            var myRes = e.HasGroupingPolicy(policy);
            Assert.Equal(res, myRes);
        }

        internal static void TestGetRoles(Enforcer e, string name, List<string> res)
        {
            var myRes = e.GetRolesForUser(name);
            var message = "Roles for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes), message);
        }

        internal static void TestGetUsers(Enforcer e, string name, List<string> res)
        {
            var myRes = e.GetUsersForRole(name);
            var message = "Users for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes),message);
        }

        internal static void TestHasRole(Enforcer e, string name, string role, bool res)
        {
            var myRes = e.HasRoleForUser(name, role);
            Assert.Equal(res, myRes);
        }

        internal static void TestGetPermissions(Enforcer e, string name, List<List<string>> res)
        {
            var myRes = e.GetPermissionsForUser(name);
            var message = "Permissions for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.Array2DEquals(res, myRes));
        }

        internal static void TestHasPermission(Enforcer e, string name, List<string> permission, bool res)
        {
            var myRes = e.HasPermissionForUser(name, permission);
            Assert.Equal(res, myRes);
        }

        internal static void TestGetRolesInDomain(Enforcer e, string name, string domain, List<string> res)
        {
            var myRes = e.GetRolesForUserInDomain(name, domain);
            var message = "Roles for " + name + " under " + domain + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes), message);
        }

        internal static void TestGetPermissionsInDomain(Enforcer e, string name, string domain, List<List<string>> res)
        {
            var myRes = e.GetPermissionsForUserInDomain(name, domain);
            Assert.True(Utility.Array2DEquals(res, myRes), "Permissions for " + name + " under " + domain + ": " + myRes + ", supposed to be " + res); 
        }
    }
}
