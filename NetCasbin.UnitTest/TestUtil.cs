using NetCasbin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NetCasbin.Test
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

        internal static void TestEnforce(Enforcer e, String sub, Object obj, String act, Boolean res)
        { 
            Assert.Equal(res, e.Enforce(sub, obj, act));
        }

        internal static void TestEnforceWithoutUsers(Enforcer e, String obj, String act, Boolean res)
        {
            Assert.Equal(res, e.Enforce(obj, act));
        }

        internal static void TestDomainEnforce(Enforcer e, String sub, String dom, String obj, String act, Boolean res)
        {
            Assert.Equal(res, e.Enforce(sub, dom, obj, act));
        }

        internal static void TestGetPolicy(Enforcer e, List<List<String>> res)
        {
            List<List<String>> myRes = e.GetPolicy();
             Assert.True(Utility.Array2DEquals(res, myRes));
        }

        internal static void TestGetFilteredPolicy(Enforcer e, int fieldIndex, List<List<String>> res, params string[] fieldValues)
        {
            List<List<String>> myRes = e.GetFilteredPolicy(fieldIndex, fieldValues);

            Assert.True(Utility.Array2DEquals(res, myRes));
        }

        internal static void TestGetGroupingPolicy(Enforcer e, List<List<String>> res)
        {
            List<List<String>> myRes = e.GetGroupingPolicy(); 
            Assert.Equal(res, myRes);
        }

        internal static void TestGetFilteredGroupingPolicy(Enforcer e, int fieldIndex, List<List<String>> res, params string[] fieldValues)
        {
            List<List<String>> myRes = e.GetFilteredGroupingPolicy(fieldIndex, fieldValues);
            Assert.Equal(res, myRes);
        }

        internal static void TestHasPolicy(Enforcer e, List<String> policy, Boolean res)
        {
            Boolean myRes = e.HasPolicy(policy); 
            Assert.Equal(res, myRes);
        }

        internal static void TestHasGroupingPolicy(Enforcer e, List<String> policy, Boolean res)
        {
            Boolean myRes = e.HasGroupingPolicy(policy);
            Assert.Equal(res, myRes);
        }

        internal static void TestGetRoles(Enforcer e, String name, List<String> res)
        {
            List<String> myRes = e.GetRolesForUser(name);
            string message = "Roles for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes), message);
        }

        internal static void TestGetUsers(Enforcer e, String name, List<String> res)
        {
            List<String> myRes = e.GetUsersForRole(name);
            var message = "Users for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes),message);
        }

        internal static void TestHasRole(Enforcer e, String name, String role, Boolean res)
        {
            Boolean myRes = e.HasRoleForUser(name, role);
            Assert.Equal(res, myRes);
        }

        internal static void TestGetPermissions(Enforcer e, String name, List<List<String>> res)
        {
            List<List<String>> myRes = e.GetPermissionsForUser(name);
            var message = "Permissions for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.Array2DEquals(res, myRes));
        }

        internal static void TestHasPermission(Enforcer e, String name, List<String> permission, Boolean res)
        {
            Boolean myRes = e.HasPermissionForUser(name, permission);
            Assert.Equal(res, myRes);
        }

        internal static void TestGetRolesInDomain(Enforcer e, String name, String domain, List<String> res)
        {
            List<String> myRes = e.GetRolesForUserInDomain(name, domain);
            var message = "Roles for " + name + " under " + domain + ": " + myRes + ", supposed to be " + res;
            Assert.True(Utility.SetEquals(res, myRes), message);
        }

        internal static void TestGetPermissionsInDomain(Enforcer e, String name, String domain, List<List<String>> res)
        {
            List<List<String>> myRes = e.GetPermissionsForUserInDomain(name, domain);
            Assert.True(Utility.Array2DEquals(res, myRes), "Permissions for " + name + " under " + domain + ": " + myRes + ", supposed to be " + res); 
        }
    }
}
