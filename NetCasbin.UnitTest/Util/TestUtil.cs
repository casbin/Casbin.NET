using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetCasbin.Util;
using Xunit;

namespace NetCasbin.UnitTest.Util
{
    public static class TestUtil
    {
        internal static List<T> AsList<T>(params T[] values)
        {
            return values.ToList();
        }

        internal static List<string> AsList(params string[] values)
        {
            return values.ToList();
        }

        internal static void TestEnforce(Enforcer e, object sub, object obj, string act, bool res)
        {
            Assert.Equal(res, e.Enforce(sub, obj, act));
        }

        internal static async Task TestEnforceAsync(Enforcer e, object sub, object obj, string act, bool res)
        {
            Assert.Equal(res, await e.EnforceAsync(sub, obj, act));
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
            List<List<string>> myRes = e.GetPolicy();
            Assert.True(Utility.Array2DEquals(res, myRes));
        }

        internal static void TestGetFilteredPolicy(Enforcer e, int fieldIndex, List<List<string>> res, params string[] fieldValues)
        {
            List<List<string>> myRes = e.GetFilteredPolicy(fieldIndex, fieldValues);

            Assert.True(Utility.Array2DEquals(res, myRes));
        }

        internal static void TestGetGroupingPolicy(Enforcer e, List<List<string>> res)
        {
            List<List<string>> myRes = e.GetGroupingPolicy();
            Assert.Equal(res, myRes);
        }

        internal static void TestGetFilteredGroupingPolicy(Enforcer e, int fieldIndex, List<List<string>> res, params string[] fieldValues)
        {
            List<List<string>> myRes = e.GetFilteredGroupingPolicy(fieldIndex, fieldValues);
            Assert.Equal(res, myRes);
        }

        internal static void TestHasPolicy(Enforcer e, List<string> policy, bool res)
        {
            bool myRes = e.HasPolicy(policy);
            Assert.Equal(res, myRes);
        }

        internal static void TestHasGroupingPolicy(Enforcer e, List<string> policy, bool res)
        {
            bool myRes = e.HasGroupingPolicy(policy);
            Assert.Equal(res, myRes);
        }

        internal static void TestGetRoles(Enforcer e, string name, List<string> res)
        {
            List<string> myRes = e.GetRolesForUser(name);
            string message = "Roles for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes), message);
        }

        internal static void TestGetUsers(Enforcer e, string name, List<string> res)
        {
            List<string> myRes = e.GetUsersForRole(name);
            string message = "Users for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes), message);
        }

        internal static void TestHasRole(Enforcer e, string name, string role, bool res)
        {
            bool myRes = e.HasRoleForUser(name, role);
            Assert.Equal(res, myRes);
        }

        internal static void TestGetPermissions(Enforcer e, string name, List<List<string>> res)
        {
            List<List<string>> myRes = e.GetPermissionsForUser(name);
            string message = "Permissions for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.Array2DEquals(res, myRes));
        }

        internal static void TestHasPermission(Enforcer e, string name, List<string> permission, bool res)
        {
            bool myRes = e.HasPermissionForUser(name, permission);
            Assert.Equal(res, myRes);
        }

        internal static void TestGetRolesInDomain(Enforcer e, string name, string domain, List<string> res)
        {
            List<string> myRes = e.GetRolesForUserInDomain(name, domain);
            string message = "Roles for " + name + " under " + domain + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes), message);
        }

        internal static void TestGetPermissionsInDomain(Enforcer e, string name, string domain, List<List<string>> res)
        {
            List<List<string>> myRes = e.GetPermissionsForUserInDomain(name, domain);
            Assert.True(Utility.Array2DEquals(res, myRes), "Permissions for " + name + " under " + domain + ": " + myRes + ", supposed to be " + res);
        }
    }
}
