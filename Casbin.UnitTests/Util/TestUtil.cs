using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Rbac;
using Casbin.Util;
using Xunit;

namespace Casbin.UnitTests.Util;

public static class TestUtil
{
    internal static List<T> AsList<T>(params T[] values) => values.ToList();

    internal static List<string> AsList(params string[] values) => values.ToList();

    internal static void TestEnforceWithoutUsers<T1, T2>(IEnforcer e, T1 obj, T2 act, bool res) =>
        Assert.Equal(res, e.Enforce(obj, act));

    internal static async Task TestEnforceWithoutUsersAsync<T1, T2>(IEnforcer e, T1 obj, T2 act, bool res) =>
        Assert.Equal(res, await e.EnforceAsync(obj, act));

    internal static void TestEnforce<T1, T2, T3>(IEnforcer e, T1 sub, T2 obj, T3 act, bool res) =>
        Assert.Equal(res, e.Enforce(sub, obj, act));

    internal static async Task TestEnforceAsync<T1, T2, T3>(IEnforcer e, T1 sub, T2 obj, T3 act, bool res) =>
        Assert.Equal(res, await e.EnforceAsync(sub, obj, act));

    internal static void TestDomainEnforce<T1, T2, T3, T4>(IEnforcer e, T1 sub, T2 dom, T3 obj, T4 act, bool res) =>
        Assert.Equal(res, e.Enforce(sub, dom, obj, act));

    internal static void TestEnforceWithMatcher<T1, T2, T3>(this IEnforcer e, string matcher, T1 sub, T2 obj, T3 act,
        bool res) => Assert.Equal(res, e.EnforceWithMatcher(matcher, sub, obj, act));

    internal static async Task TestEnforceWithMatcherAsync<T1, T2, T3>(this IEnforcer e, string matcher, T1 sub, T2 obj,
        T3 act, bool res) => Assert.Equal(res, await e.EnforceWithMatcherAsync(matcher, sub, obj, act));

    internal static void TestEnforceEx<T1, T2, T3>(IEnforcer e, T1 sub, T2 obj, T3 act, List<string> res)
    {
        List<IEnumerable<string>> myRes = e.EnforceEx(sub, obj, act).Item2.ToList();
        string message = "Key: " + myRes + ", supposed to be " + res;
        if (myRes.Count > 0)
        {
            Assert.True(Utility.SetEquals(res, myRes[0].ToList()), message);
        }
    }

    internal static async Task TestEnforceExAsync<T1, T2, T3>(IEnforcer e, T1 sub, T2 obj, T3 act, List<string> res)
    {
        List<IEnumerable<string>> myRes = (await e.EnforceExAsync(sub, obj, act)).Item2.ToList();
        string message = "Key: " + myRes + ", supposed to be " + res;
        if (myRes.Count > 0)
        {
            Assert.True(Utility.SetEquals(res, myRes[0].ToList()), message);
        }
    }

    internal static void TestEnforceExWithMatcher<T1, T2, T3>(this IEnforcer e, string matcher, T1 sub, T2 obj, T3 act,
        List<string> res)
    {
        List<IEnumerable<string>> myRes = e.EnforceExWithMatcher(matcher, sub, obj, act).Item2.ToList();
        string message = "Key: " + myRes + ", supposed to be " + res;
        if (myRes.Any())
        {
            Assert.True(Utility.SetEquals(res, myRes[0].ToList()), message);
        }
    }

    internal static async Task TestEnforceExWithMatcherAsync<T1, T2, T3>(this IEnforcer e, string matcher, T1 sub,
        T2 obj, T3 act, List<string> res)
    {
        List<IEnumerable<string>> myRes = (await e.EnforceExWithMatcherAsync(matcher, sub, obj, act)).Item2.ToList();
        string message = "Key: " + myRes + ", supposed to be " + res;
        if (myRes.Any())
        {
            Assert.True(Utility.SetEquals(res, myRes[0].ToList()), message);
        }
    }

    internal static void TestStringList(GetAllList getAllList, List<string> res)
    {
        IEnumerable<string> myRes = getAllList();
        Assert.True(res.DeepEquals(myRes));
    }

    internal static void TestGetPolicy(IEnforcer e, List<List<string>> res)
    {
        IEnumerable<IEnumerable<string>> myRes = e.GetPolicy();
        Assert.True(res.DeepEquals(myRes));
    }

    internal static void TestGetFilteredPolicy(IEnforcer e, int fieldIndex, List<List<string>> res,
        params string[] fieldValues)
    {
        IEnumerable<IEnumerable<string>> myRes = e.GetFilteredPolicy(fieldIndex, fieldValues);
        Assert.True(res.DeepEquals(myRes));
    }

    internal static void TestGetGroupingPolicy(IEnforcer e, List<List<string>> res)
    {
        IEnumerable<IEnumerable<string>> myRes = e.GetGroupingPolicy();
        Assert.Equal(res, myRes);
    }

    internal static void TestGetFilteredGroupingPolicy(IEnforcer e, int fieldIndex, List<List<string>> res,
        params string[] fieldValues)
    {
        IEnumerable<IEnumerable<string>> myRes = e.GetFilteredGroupingPolicy(fieldIndex, fieldValues);
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
        IEnumerable<IEnumerable<string>> myRes = e.GetPermissionsForUser(name, domain);
        string message = "Permissions for " + name + ": " + myRes + ", supposed to be " + res;
        Assert.True(res.DeepEquals(myRes), message);
    }

    internal static void TestGetImplicitPermissions(IEnforcer e, string name, List<List<string>> res,
        string domain = null)
    {
        IEnumerable<IEnumerable<string>> myRes = e.GetImplicitPermissionsForUser(name, domain);
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
        string message = "Implicit roles in domain " + name + " under " + domain + ": " + myRes + ", supposed to be " +
                         res;
        Assert.True(Utility.SetEquals(res, myRes), message);
    }

    internal static void TestGetPermissionsInDomain(IEnforcer e, string name, string domain, List<List<string>> res)
    {
        IEnumerable<IEnumerable<string>> myRes = e.GetPermissionsForUserInDomain(name, domain);
        Assert.True(res.DeepEquals(myRes),
            "Permissions for " + name + " under " + domain + ": " + myRes + ", supposed to be " + res);
    }

    internal delegate IEnumerable<string> GetAllList();

    #region RoleManger test

    internal static void TestRole(IRoleManager roleManager, string name1, string name2, bool expectResult)
    {
        bool result = roleManager.HasLink(name1, name2);
        Assert.Equal(expectResult, result);
    }

    internal static void TestDomainRole(IRoleManager roleManager, string name1, string name2, string domain,
        bool expectResult)
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

    internal static void TestGetRolesWithDomain(IRoleManager roleManager, string name, string domain,
        List<string> expectResult)
    {
        List<string> result = roleManager.GetRoles(name, domain).ToList();
        string message = $"{name}: {result}, supposed to be {expectResult}";
        Assert.True(Utility.SetEquals(expectResult, result), message);
    }

    #endregion
}
