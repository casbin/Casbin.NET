using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace NetCasbin.Test
{
    public  class TestUtil
    {

        internal List<T> asList<T>(params T[] values)
        {
            return values.ToList();
        }
        internal List<string> asList(params string[] values)
        {
            return values.ToList();
        }

        internal static void testEnforce(Enforcer e, String sub, Object obj, String act, Boolean res)
        { 
            Assert.Equal(res, e.Enforce(sub, obj, act));
        }

        internal static void testEnforceWithoutUsers(Enforcer e, String obj, String act, Boolean res)
        {
            Assert.Equal(res, e.Enforce(obj, act));
        }

        internal static void testDomainEnforce(Enforcer e, String sub, String dom, String obj, String act, Boolean res)
        {
            Assert.Equal(res, e.Enforce(sub, dom, obj, act));
        }

        internal static void testGetPolicy(Enforcer e, List<List<String>> res)
        {
            List<List<String>> myRes = e.GetPolicy();
             Assert.True(Util.Array2DEquals(res, myRes));
        }

        internal static void testGetFilteredPolicy(Enforcer e, int fieldIndex, List<List<String>> res, params string[] fieldValues)
        {
            List<List<String>> myRes = e.GetFilteredPolicy(fieldIndex, fieldValues);

            Assert.True(Util.Array2DEquals(res, myRes));
        }

        internal static void testGetGroupingPolicy(Enforcer e, List<List<String>> res)
        {
            List<List<String>> myRes = e.getGroupingPolicy(); 
            Assert.Equal(res, myRes);
        }

        internal static void testGetFilteredGroupingPolicy(Enforcer e, int fieldIndex, List<List<String>> res, params string[] fieldValues)
        {
            List<List<String>> myRes = e.GetFilteredGroupingPolicy(fieldIndex, fieldValues);
            Assert.Equal(res, myRes);
        }

        internal static void testHasPolicy(Enforcer e, List<String> policy, Boolean res)
        {
            Boolean myRes = e.HasPolicy(policy); 
            Assert.Equal(res, myRes);
        }

        internal static void testHasGroupingPolicy(Enforcer e, List<String> policy, Boolean res)
        {
            Boolean myRes = e.HasGroupingPolicy(policy);
            Assert.Equal(res, myRes);
        }

        internal static void testGetRoles(Enforcer e, String name, List<String> res)
        {
            List<String> myRes = e.GetRolesForUser(name);
            string message = "Roles for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Util.SetEquals(res, myRes), message);
        }

        internal static void testGetUsers(Enforcer e, String name, List<String> res)
        {
            List<String> myRes = e.GetUsersForRole(name);
            var message = "Users for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Util.SetEquals(res, myRes),message);
        }

        internal static void testHasRole(Enforcer e, String name, String role, Boolean res)
        {
            Boolean myRes = e.HasRoleForUser(name, role);
            Assert.Equal(res, myRes);
        }

        internal static void testGetPermissions(Enforcer e, String name, List<List<String>> res)
        {
            List<List<String>> myRes = e.GetPermissionsForUser(name);
            var message = "Permissions for " + name + ": " + myRes + ", supposed to be " + res;
            Assert.True(Util.Array2DEquals(res, myRes));
        }

        internal static void testHasPermission(Enforcer e, String name, List<String> permission, Boolean res)
        {
            Boolean myRes = e.HasPermissionForUser(name, permission);
            Assert.Equal(res, myRes);
        }

        internal static void testGetRolesInDomain(Enforcer e, String name, String domain, List<String> res)
        {
            List<String> myRes = e.GetRolesForUserInDomain(name, domain);
            var message = "Roles for " + name + " under " + domain + ": " + myRes + ", supposed to be " + res;
            Assert.True(Util.SetEquals(res, myRes), message);
        }

        internal static void testGetPermissionsInDomain(Enforcer e, String name, String domain, List<List<String>> res)
        {
            List<List<String>> myRes = e.GetPermissionsForUserInDomain(name, domain);
            Assert.True(Util.Array2DEquals(res, myRes), "Permissions for " + name + " under " + domain + ": " + myRes + ", supposed to be " + res); 
        }
    }
}
