using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Rbac;
using Casbin.Util;
using Xunit;

namespace Casbin.UnitTests.Util
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

        internal static void TestEnforce(IEnforcer e, object sub, object obj, string act, bool res)
        {
            Assert.Equal(res, e.Enforce(sub, obj, act));
        }

        internal static async Task TestEnforceAsync(IEnforcer e, object sub, object obj, string act, bool res)
        {
            Assert.Equal(res, await e.EnforceAsync(sub, obj, act));
        }

        internal static void TestEnforceWithMatcher(this IEnforcer e, string matcher, object sub, object obj, string act, bool res)
        {
            Assert.Equal(res, e.EnforceWithMatcher(matcher, sub, obj, act));
        }

        internal static async Task TestEnforceWithMatcherAsync(this IEnforcer e, string matcher, object sub, object obj, string act, bool res)
        {
            Assert.Equal(res, await e.EnforceWithMatcherAsync(matcher, sub, obj, act));
        }

        internal static void TestEnforceEx(IEnforcer e, object sub, object obj, string act, List<string> res)
        {
            var myRes = e.EnforceEx(sub, obj, act).Item2.ToList();
            string message = "Key: " + myRes + ", supposed to be " + res;
            if (myRes.Count > 0)
            {
                Assert.True(Utility.SetEquals(res, myRes[0].ToList()), message);
            }
        }

        internal static async Task TestEnforceExAsync(IEnforcer e, object sub, object obj, string act, List<string> res)
        {
            var myRes = (await e.EnforceExAsync(sub, obj, act)).Item2.ToList();
            string message = "Key: " + myRes + ", supposed to be " + res;
            if (myRes.Count > 0)
            {
                Assert.True(Utility.SetEquals(res, myRes[0].ToList()), message);
            }
        }

        internal static void TestEnforceExWithMatcher(this IEnforcer e, string matcher, object sub, object obj, string act, List<string> res)
        {
            var myRes = e.EnforceExWithMatcher(matcher, sub, obj, act).Item2.ToList();
            string message = "Key: " + myRes + ", supposed to be " + res;
            if (myRes.Any())
            {
                Assert.True(Utility.SetEquals(res, myRes[0].ToList()), message);
            }
        }

        internal static async Task TestEnforceExWithMatcherAsync(this IEnforcer e, string matcher, object sub, object obj, string act, List<string> res)
        {
            var myRes = (await e.EnforceExWithMatcherAsync(matcher, sub, obj, act)).Item2.ToList();
            string message = "Key: " + myRes + ", supposed to be " + res;
            if (myRes.Any())
            {
                Assert.True(Utility.SetEquals(res, myRes[0].ToList()), message);
            }
        }

        internal static void TestEnforceWithoutUsers(IEnforcer e, string obj, string act, bool res)
        {
            Assert.Equal(res, e.Enforce(obj, act));
        }

        internal static async Task TestEnforceWithoutUsersAsync(IEnforcer e, string obj, string act, bool res)
        {
            Assert.Equal(res, await e.EnforceAsync(obj, act));
        }

        internal static void TestDomainEnforce(IEnforcer e, string sub, string dom, string obj, string act, bool res)
        {
            Assert.Equal(res, e.Enforce(sub, dom, obj, act));
        }

        internal static void TestGetPolicy(IEnforcer e, List<List<string>> res)
        {
            var myRes = e.GetPolicy();
            Assert.True(res.DeepEquals(myRes));
        }

        internal static void TestGetFilteredPolicy(IEnforcer e, int fieldIndex, List<List<string>> res, params string[] fieldValues)
        {
            var myRes = e.GetFilteredPolicy(fieldIndex, fieldValues);
            Assert.True(res.DeepEquals(myRes));
        }

        internal static void TestGetGroupingPolicy(IEnforcer e, List<List<string>> res)
        {
            var myRes = e.GetGroupingPolicy();
            Assert.Equal(res, myRes);
        }

        internal static void TestGetFilteredGroupingPolicy(IEnforcer e, int fieldIndex, List<List<string>> res, params string[] fieldValues)
        {
            var myRes = e.GetFilteredGroupingPolicy(fieldIndex, fieldValues);
            Assert.Equal(res, myRes);
        }

        internal static void TestHasPolicy(IEnforcer e, List<string> policy, bool res)
        {
            bool myRes = e.HasPolicy(policy);
            Assert.Equal(res, myRes);
        }

        internal static void TestHasGroupingPolicy(IEnforcer e, List<string> policy, bool res)
        {
            bool myRes = e.HasGroupingPolicy(policy);
            Assert.Equal(res, myRes);
        }

        internal static void TestGetRoles(IEnforcer e, string name, List<string> res, string domain = null)
        {
            List<string> myRes = e.GetRolesForUser(name, domain).ToList();
            string message = "Roles for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes), message);
        }

        internal static void TestGetUsers(IEnforcer e, string name, List<string> res, string domain = null)
        {
            List<string> myRes = e.GetUsersForRole(name, domain).ToList();
            string message = "Users for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes), message);
        }

        internal static void TestHasRole(IEnforcer e, string name, string role, bool res, string domain = null)
        {
            bool myRes = e.HasRoleForUser(name, role, domain);
            Assert.Equal(res, myRes);
        }

        internal static void TestGetPermissions(IEnforcer e, string name, List<List<string>> res, string domain = null)
        {
            var myRes = e.GetPermissionsForUser(name, domain);
            string message = "Permissions for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(res.DeepEquals(myRes), message);
        }

        internal static void TestGetImplicitPermissions(IEnforcer e, string name, List<List<string>> res, string domain = null)
        {
            var myRes = e.GetImplicitPermissionsForUser(name, domain);
            string message = "Implicit permissions for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(res.DeepEquals(myRes), message);
        }

        internal static void TestHasPermission(IEnforcer e, string name, List<string> permission, bool res)
        {
            bool myRes = e.HasPermissionForUser(name, permission);
            Assert.Equal(res, myRes);
        }

        internal static void TestGetRolesInDomain(IEnforcer e, string name, string domain, List<string> res)
        {
            List<string> myRes = e.GetRolesForUserInDomain(name, domain).ToList();
            string message = "Roles for " + name + " under " + domain + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes), message);
        }

        internal static void TestGetDomainsForUser(this IEnforcer e, string name, IEnumerable<string> res)
        {
            List<string> myRes = e.GetDomainsForUser(name).ToList();
            string message = "Domains for " + name + " under " + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res.ToList(), myRes), message);
        }

        internal static void TestGetImplicitRolesInDomain(IEnforcer e, string name, string domain, List<string> res)
        {
            List<string> myRes = e.GetImplicitRolesForUser(name, domain).ToList();
            string message = "Implicit roles in domain " + name + " under " + domain + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes), message);
        }

        internal static void TestGetPermissionsInDomain(IEnforcer e, string name, string domain, List<List<string>> res)
        {
            var myRes = e.GetPermissionsForUserInDomain(name, domain);
            Assert.True(res.DeepEquals(myRes), "Permissions for " + name + " under " + domain + ": " + myRes + ", supposed to be " + res);
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
            List<string> result = roleManager.GetRoles(name).ToList();
            string message = $"{name}: {result}, supposed to be {expectResult}";
            Assert.True(Utility.SetEquals(expectResult, result), message);
        }

        internal static void TestGetRolesWithDomain(IRoleManager roleManager, string name, string domain, List<string> expectResult)
        {
            List<string> result = roleManager.GetRoles(name, domain).ToList();
            string message = $"{name}: {result}, supposed to be {expectResult}";
            Assert.True(Utility.SetEquals(expectResult, result), message);
        }

        #endregion
    }
}
