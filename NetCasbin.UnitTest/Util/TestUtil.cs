using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using NetCasbin.Rbac;
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

        internal static void TestEnforceWithMatcher(this Enforcer e, string matcher, object sub, object obj, string act, bool res)
        {
            Assert.Equal(res, e.EnforceWithMatcher(matcher, sub, obj, act));
        }

        internal static async Task TestEnforceWithMatcherAsync(this Enforcer e, string matcher, object sub, object obj, string act, bool res)
        {
            Assert.Equal(res, await e.EnforceWithMatcherAsync(matcher, sub, obj, act));
        }

        internal static void TestEnforceEx(Enforcer e, object sub, object obj, string act, List<string> res)
        {
            var myRes = e.EnforceEx(sub, obj, act).Item2.ToList();
            string message = "Key: " + myRes + ", supposed to be " + res;
            if (myRes.Count > 0)
            {
                Assert.True(Utility.SetEquals(res, myRes[0].ToList()), message);
            }
        }

        internal static async Task TestEnforceExAsync(Enforcer e, object sub, object obj, string act, List<string> res)
        {
            var myRes = (await e.EnforceExAsync(sub, obj, act)).Item2.ToList();
            string message = "Key: " + myRes + ", supposed to be " + res;
            if (myRes.Count > 0)
            {
                Assert.True(Utility.SetEquals(res, myRes[0].ToList()), message);
            }
        }

        internal static void TestEnforceExWithMatcher(this Enforcer e, string matcher, object sub, object obj, string act, List<string> res)
        {
            var myRes = e.EnforceExWithMatcher(matcher, sub, obj, act).Item2.ToList();
            string message = "Key: " + myRes + ", supposed to be " + res;
            if (myRes.Count > 0)
            {
                Assert.True(Utility.SetEquals(res, myRes[0].ToList()), message);
            }
        }

        internal static async Task TestEnforceExWithMatcherAsync(this Enforcer e, string matcher, object sub, object obj, string act, List<string> res)
        {
            var myRes = (await e.EnforceExWithMatcherAsync(matcher, sub, obj, act)).Item2.ToList();
            string message = "Key: " + myRes + ", supposed to be " + res;
            if (myRes.Count > 0)
            {
                Assert.True(Utility.SetEquals(res, myRes[0].ToList()), message);
            }
        }

        internal static void TestEnforceWithoutUsers(Enforcer e, string obj, string act, bool res)
        {
            Assert.Equal(res, e.Enforce(obj, act));
        }

        internal static async Task TestEnforceWithoutUsersAsync(Enforcer e, string obj, string act, bool res)
        {
            Assert.Equal(res, await e.EnforceAsync(obj, act));
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

        internal static void TestGetRoles(Enforcer e, string name, List<string> res, string domain = null)
        {
            List<string> myRes = e.GetRolesForUser(name, domain);
            string message = "Roles for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes), message);
        }

        internal static void TestGetUsers(Enforcer e, string name, List<string> res, string domain = null)
        {
            List<string> myRes = e.GetUsersForRole(name, domain);
            string message = "Users for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes), message);
        }

        internal static void TestHasRole(Enforcer e, string name, string role, bool res, string domain = null)
        {
            bool myRes = e.HasRoleForUser(name, role, domain);
            Assert.Equal(res, myRes);
        }

        internal static void TestGetPermissions(Enforcer e, string name, List<List<string>> res, string domain = null)
        {
            List<List<string>> myRes = e.GetPermissionsForUser(name, domain);
            string message = "Permissions for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.Array2DEquals(res, myRes), message);
        }

        internal static void TestGetImplicitPermissions(Enforcer e, string name, List<List<string>> res, string domain = null)
        {
            List<List<string>> myRes = e.GetImplicitPermissionsForUser(name, domain);
            string message = "Implicit permissions for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.Array2DEquals(res, myRes), message);
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

        internal static void TestGetDomainsForUser(this Enforcer e, string name, IEnumerable<string> res)
        {
            List<string> myRes = e.GetDomainsForUser(name).ToList();
            string message = "Domains for " + name + " under " + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res.ToList(), myRes), message);
        }

        internal static void TestGetImplicitRolesInDomain(Enforcer e, string name, string domain, List<string> res)
        {
            List<string> myRes = e.GetImplicitRolesForUser(name, domain);
            string message = "Implicit roles in domain " + name + " under " + domain + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes), message);
        }

        internal static void TestGetPermissionsInDomain(Enforcer e, string name, string domain, List<List<string>> res)
        {
            List<List<string>> myRes = e.GetPermissionsForUserInDomain(name, domain);
            Assert.True(Utility.Array2DEquals(res, myRes), "Permissions for " + name + " under " + domain + ": " + myRes + ", supposed to be " + res);
        }

        #region RoleManger test

        internal static void TestRole(IRoleManager roleManager, string name1, string name2, bool expectResult)
        {
            bool result = roleManager.HasLink(name1, name2);
            Assert.Equal(expectResult, result);
        }

        internal static void TestDomainRole(IRoleManager roleManager, string name1, string name2, string domain, bool expectResult)
        {
            bool result = roleManager.HasLink(name1, name2, domain);
            Assert.Equal(expectResult, result);
        }

        internal static void TestGetRoles(IRoleManager roleManager, string name, List<string> expectResult)
        {
            List<string> result = roleManager.GetRoles(name);
            string message = $"{name}: {result}, supposed to be {expectResult}";
            Assert.True(Utility.SetEquals(expectResult, result), message);
        }

        internal static void TestGetRolesWithDomain(IRoleManager roleManager, string name, string domain, List<string> expectResult)
        {
            List<string> result = roleManager.GetRoles(name, domain);
            string message = $"{name}: {result}, supposed to be {expectResult}";
            Assert.True(Utility.SetEquals(expectResult, result), message);
        }

        #endregion
    }
}
