using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NetCasbin.UnitTest.Fixtures;
using Xunit;
using static NetCasbin.UnitTest.Util.TestUtil;

namespace NetCasbin.UnitTest
{
    [Collection("Model collection")]
    public class RbacApiTest
    {
        private readonly TestModelFixture _testModelFixture;

        public RbacApiTest(TestModelFixture testModelFixture)
        {
            _testModelFixture = testModelFixture;
        }

        [Fact]
        public void TestRoleApi()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.BuildRoleLinks();

            TestGetRoles(e, "alice", AsList("data2_admin"));
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());
            TestGetRoles(e, "non_exist", AsList());

            TestHasRole(e, "alice", "data1_admin", false);
            TestHasRole(e, "alice", "data2_admin", true);

            e.AddRoleForUser("alice", "data1_admin");

            TestGetRoles(e, "alice", AsList("data1_admin", "data2_admin"));
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());

            e.DeleteRoleForUser("alice", "data1_admin");

            TestGetRoles(e, "alice", AsList("data2_admin"));
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());

            e.DeleteRolesForUser("alice");

            TestGetRoles(e, "alice", AsList());
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());

            e.AddRoleForUser("alice", "data1_admin");
            e.DeleteUser("alice");

            TestGetRoles(e, "alice", AsList());
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());

            e.AddRoleForUser("alice", "data2_admin");

            TestEnforce(e, "alice", "data1", "read", false);
            TestEnforce(e, "alice", "data1", "write", false);

            TestEnforce(e, "alice", "data2", "read", true);
            TestEnforce(e, "alice", "data2", "write", true);

            TestEnforce(e, "bob", "data1", "read", false);
            TestEnforce(e, "bob", "data1", "write", false);
            TestEnforce(e, "bob", "data2", "read", false);
            TestEnforce(e, "bob", "data2", "write", true);

            e.DeleteRole("data2_admin");

            TestEnforce(e, "alice", "data1", "read", false);
            TestEnforce(e, "alice", "data1", "write", false);
            TestEnforce(e, "alice", "data2", "read", false);
            TestEnforce(e, "alice", "data2", "write", false);
            TestEnforce(e, "bob", "data1", "read", false);
            TestEnforce(e, "bob", "data1", "write", false);
            TestEnforce(e, "bob", "data2", "read", false);
            TestEnforce(e, "bob", "data2", "write", true);
        }

        [Fact]
        public async Task TestRoleApiAsync()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.BuildRoleLinks();

            TestGetRoles(e, "alice", AsList("data2_admin"));
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());
            TestGetRoles(e, "non_exist", AsList());

            TestHasRole(e, "alice", "data1_admin", false);
            TestHasRole(e, "alice", "data2_admin", true);

            await e.AddRoleForUserAsync("alice", "data1_admin");

            TestGetRoles(e, "alice", AsList("data1_admin", "data2_admin"));
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());

            await e.DeleteRoleForUserAsync("alice", "data1_admin");

            TestGetRoles(e, "alice", AsList("data2_admin"));
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());

            await e.DeleteRolesForUserAsync("alice");

            TestGetRoles(e, "alice", AsList());
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());

            await e.AddRoleForUserAsync("alice", "data1_admin");
            await e.DeleteUserAsync("alice");

            TestGetRoles(e, "alice", AsList());
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());

            await e.AddRoleForUserAsync("alice", "data2_admin");

            await TestEnforceAsync(e, "alice", "data1", "read", false);
            await TestEnforceAsync(e, "alice", "data1", "write", false);

            await TestEnforceAsync(e, "alice", "data2", "read", true);
            await TestEnforceAsync(e, "alice", "data2", "write", true);

            await TestEnforceAsync(e, "bob", "data1", "read", false);
            await TestEnforceAsync(e, "bob", "data1", "write", false);
            await TestEnforceAsync(e, "bob", "data2", "read", false);
            await TestEnforceAsync(e, "bob", "data2", "write", true);

            await e.DeleteRoleAsync("data2_admin");

            await TestEnforceAsync(e, "alice", "data1", "read", false);
            await TestEnforceAsync(e, "alice", "data1", "write", false);
            await TestEnforceAsync(e, "alice", "data2", "read", false);
            await TestEnforceAsync(e, "alice", "data2", "write", false);
            await TestEnforceAsync(e, "bob", "data1", "read", false);
            await TestEnforceAsync(e, "bob", "data1", "write", false);
            await TestEnforceAsync(e, "bob", "data2", "read", false);
            await TestEnforceAsync(e, "bob", "data2", "write", true);
        }

        [Fact]
        public void TestRoleApiWithDomains()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacWithDomainsTestModel());
            e.BuildRoleLinks();

            TestHasRole(e, "alice", "admin", true, "domain1");
            TestHasRole(e, "alice", "admin", false, "domain2");

            TestGetRoles(e, "alice", AsList("admin"), "domain1");
            TestGetRoles(e, "bob", AsList(), "domain1");
            TestGetRoles(e, "admin", AsList(), "domain1");
            TestGetRoles(e, "non_exist", AsList(), "domain1");
            TestGetRoles(e, "alice", AsList(), "domain2");
            TestGetRoles(e, "bob", AsList("admin"), "domain2");
            TestGetRoles(e, "admin", AsList(), "domain2");
            TestGetRoles(e, "non_exist", AsList(), "domain2");

            _ = e.DeleteRoleForUser("alice", "admin", "domain1");

            _ = e.AddRoleForUser("bob", "admin", "domain1");

            TestGetRoles(e, "alice", AsList(), "domain1");
            TestGetRoles(e, "bob", AsList("admin"), "domain1");
            TestGetRoles(e, "admin", AsList(), "domain1");
            TestGetRoles(e, "non_exist", AsList(), "domain1");
            TestGetRoles(e, "alice", AsList(), "domain2");
            TestGetRoles(e, "bob", AsList("admin"), "domain2");
            TestGetRoles(e, "admin", AsList(), "domain2");
            TestGetRoles(e, "non_exist", AsList(), "domain2");

            _ = e.AddRoleForUser("alice", "admin", "domain1");

            _ = e.DeleteRolesForUser("bob", "domain1");

            TestGetRoles(e, "alice", AsList("admin"), "domain1");
            TestGetRoles(e, "bob", AsList(), "domain1");
            TestGetRoles(e, "admin", AsList(), "domain1");
            TestGetRoles(e, "non_exist", AsList(), "domain1");
            TestGetRoles(e, "alice", AsList(), "domain2");
            TestGetRoles(e, "bob", AsList("admin"), "domain2");
            TestGetRoles(e, "admin", AsList(), "domain2");
            TestGetRoles(e, "non_exist", AsList(), "domain2");

            _ = e.AddRolesForUser("bob", AsList("admin", "admin1", "admin2"), "domain1");

            TestGetRoles(e, "bob", AsList("admin", "admin1", "admin2"), "domain1");

            TestGetPermissions(e, "admin", AsList(
                    AsList("admin", "domain1", "data1", "read"),
                    AsList("admin", "domain1", "data1", "write")),
                "domain1");
            TestGetPermissions(e, "admin", AsList(
                    AsList("admin", "domain2", "data2", "read"),
                    AsList("admin", "domain2", "data2", "write")),
                "domain2");
        }

        [Fact]
        public async Task TestRoleApiWithDomainsAsync()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacWithDomainsTestModel());
            e.BuildRoleLinks();

            TestHasRole(e, "alice", "admin", true, "domain1");
            TestHasRole(e, "alice", "admin", false, "domain2");

            TestGetRoles(e, "alice", AsList("admin"), "domain1");
            TestGetRoles(e, "bob", AsList(), "domain1");
            TestGetRoles(e, "admin", AsList(), "domain1");
            TestGetRoles(e, "non_exist", AsList(), "domain1");
            TestGetRoles(e, "alice", AsList(), "domain2");
            TestGetRoles(e, "bob", AsList("admin"), "domain2");
            TestGetRoles(e, "admin", AsList(), "domain2");
            TestGetRoles(e, "non_exist", AsList(), "domain2");

            _ = await e.DeleteRoleForUserAsync("alice", "admin", "domain1");

            _ = await e.AddRoleForUserAsync("bob", "admin", "domain1");

            TestGetRoles(e, "alice", AsList(), "domain1");
            TestGetRoles(e, "bob", AsList("admin"), "domain1");
            TestGetRoles(e, "admin", AsList(), "domain1");
            TestGetRoles(e, "non_exist", AsList(), "domain1");
            TestGetRoles(e, "alice", AsList(), "domain2");
            TestGetRoles(e, "bob", AsList("admin"), "domain2");
            TestGetRoles(e, "admin", AsList(), "domain2");
            TestGetRoles(e, "non_exist", AsList(), "domain2");

            _ = await e.AddRoleForUserAsync("alice", "admin", "domain1");

            _ = await e.DeleteRolesForUserAsync("bob", "domain1");

            TestGetRoles(e, "alice", AsList("admin"), "domain1");
            TestGetRoles(e, "bob", AsList(), "domain1");
            TestGetRoles(e, "admin", AsList(), "domain1");
            TestGetRoles(e, "non_exist", AsList(), "domain1");
            TestGetRoles(e, "alice", AsList(), "domain2");
            TestGetRoles(e, "bob", AsList("admin"), "domain2");
            TestGetRoles(e, "admin", AsList(), "domain2");
            TestGetRoles(e, "non_exist", AsList(), "domain2");

            _ = await e.AddRolesForUserAsync("bob", AsList("admin", "admin1", "admin2"), "domain1");

            TestGetRoles(e, "bob", AsList("admin", "admin1", "admin2"), "domain1");

            TestGetPermissions(e, "admin", AsList(
                    AsList("admin", "domain1", "data1", "read"),
                    AsList("admin", "domain1", "data1", "write")),
                "domain1");
            TestGetPermissions(e, "admin", AsList(
                    AsList("admin", "domain2", "data2", "read"),
                    AsList("admin", "domain2", "data2", "write")),
                "domain2");
        }

        [Fact]
        public void TestAddRolesForUser()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.BuildRoleLinks();

            _ = e.AddRolesForUser("alice", AsList("data1_admin", "data2_admin", "data3_admin"));
            // The "alice" already has "data2_admin" , it will be return false. So "alice" just has "data2_admin".
            TestGetRoles(e, "alice", AsList("data2_admin"));
            // delete role
            _ = e.DeleteRoleForUser("alice", "data2_admin");

            _ = e.AddRolesForUser("alice", AsList("data1_admin", "data2_admin", "data3_admin"));
            TestGetRoles(e, "alice", AsList("data1_admin", "data2_admin", "data3_admin"));
            TestEnforce(e, "alice", "data1", "read", true);
            TestEnforce(e, "alice", "data2", "read", true);
            TestEnforce(e, "alice", "data2", "write", true);
        }

        [Fact]
        public async Task TestAddRolesForUserAsync()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.BuildRoleLinks();

            _ = await e.AddRolesForUserAsync("alice", AsList("data1_admin", "data2_admin", "data3_admin"));
            // The "alice" already has "data2_admin" , it will be return false. So "alice" just has "data2_admin".
            TestGetRoles(e, "alice", AsList("data2_admin"));
            // delete role
            _ = await e.DeleteRoleForUserAsync("alice", "data2_admin");

            _ = await e.AddRolesForUserAsync("alice", AsList("data1_admin", "data2_admin", "data3_admin"));
            TestGetRoles(e, "alice", AsList("data1_admin", "data2_admin", "data3_admin"));
            await TestEnforceAsync(e, "alice", "data1", "read", true);
            await TestEnforceAsync(e, "alice", "data2", "read", true);
            await TestEnforceAsync(e, "alice", "data2", "write", true);
        }

        [Fact]
        public void TestPermissionApi()
        {
            var e = new Enforcer(_testModelFixture.GetBasicWithoutResourceTestModel());
            e.BuildRoleLinks();

            TestEnforceWithoutUsers(e, "alice", "read", true);
            TestEnforceWithoutUsers(e, "alice", "write", false);
            TestEnforceWithoutUsers(e, "bob", "read", false);
            TestEnforceWithoutUsers(e, "bob", "write", true);

            TestGetPermissions(e, "alice", AsList(AsList("alice", "read")));
            TestGetPermissions(e, "bob", AsList(AsList("bob", "write")));
            
            TestHasPermission(e, "alice", AsList("read"), true);
            TestHasPermission(e, "alice", AsList("write"), false);
            TestHasPermission(e, "bob", AsList("read"), false);
            TestHasPermission(e, "bob", AsList("write"), true);

            _ = e.DeletePermission("read");

            TestEnforceWithoutUsers(e, "alice", "read", false);
            TestEnforceWithoutUsers(e, "alice", "write", false);
            TestEnforceWithoutUsers(e, "bob", "read", false);
            TestEnforceWithoutUsers(e, "bob", "write", true);

            _ = e.AddPermissionForUser("bob", "read");

            TestEnforceWithoutUsers(e, "alice", "read", false);
            TestEnforceWithoutUsers(e, "alice", "write", false);
            TestEnforceWithoutUsers(e, "bob", "read", true);
            TestEnforceWithoutUsers(e, "bob", "write", true);

            _ = e.DeletePermissionForUser("bob", "read");

            TestEnforceWithoutUsers(e, "alice", "read", false);
            TestEnforceWithoutUsers(e, "alice", "write", false);
            TestEnforceWithoutUsers(e, "bob", "read", false);
            TestEnforceWithoutUsers(e, "bob", "write", true);

            _ = e.DeletePermissionsForUser("bob");

            TestEnforceWithoutUsers(e, "alice", "read", false);
            TestEnforceWithoutUsers(e, "alice", "write", false);
            TestEnforceWithoutUsers(e, "bob", "read", false);
            TestEnforceWithoutUsers(e, "bob", "write", false);
        }

        [Fact]
        public async Task TestPermissionApiAsync()
        {
            var e = new Enforcer(_testModelFixture.GetBasicWithoutResourceTestModel());
            e.BuildRoleLinks();

            await TestEnforceWithoutUsersAsync(e, "alice", "read", true);
            await TestEnforceWithoutUsersAsync(e, "alice", "write", false);
            await TestEnforceWithoutUsersAsync(e, "bob", "read", false);
            await TestEnforceWithoutUsersAsync(e, "bob", "write", true);

            TestGetPermissions(e, "alice", AsList(AsList("alice", "read")));
            TestGetPermissions(e, "bob", AsList(AsList("bob", "write")));
            
            TestHasPermission(e, "alice", AsList("read"), true);
            TestHasPermission(e, "alice", AsList("write"), false);
            TestHasPermission(e, "bob", AsList("read"), false);
            TestHasPermission(e, "bob", AsList("write"), true);

            _ = await e.DeletePermissionAsync("read");

            await TestEnforceWithoutUsersAsync(e, "alice", "read", false);
            await TestEnforceWithoutUsersAsync(e, "alice", "write", false);
            await TestEnforceWithoutUsersAsync(e, "bob", "read", false);
            await TestEnforceWithoutUsersAsync(e, "bob", "write", true);

            _ = await e.AddPermissionForUserAsync("bob", "read");

            await TestEnforceWithoutUsersAsync(e, "alice", "read", false);
            await TestEnforceWithoutUsersAsync(e, "alice", "write", false);
            await TestEnforceWithoutUsersAsync(e, "bob", "read", true);
            await TestEnforceWithoutUsersAsync(e, "bob", "write", true);

            _ = await e.DeletePermissionForUserAsync("bob", "read");

            await TestEnforceWithoutUsersAsync(e, "alice", "read", false);
            await TestEnforceWithoutUsersAsync(e, "alice", "write", false);
            await TestEnforceWithoutUsersAsync(e, "bob", "read", false);
            await TestEnforceWithoutUsersAsync(e, "bob", "write", true);

            _ = await e.DeletePermissionsForUserAsync("bob");

            await TestEnforceWithoutUsersAsync(e, "alice", "read", false);
            await TestEnforceWithoutUsersAsync(e, "alice", "write", false);
            await TestEnforceWithoutUsersAsync(e, "bob", "read", false);
            await TestEnforceWithoutUsersAsync(e, "bob", "write", false);
        }

        [Fact]
        public void TestGetImplicitPermissionsForUser()
        {
            var e = new Enforcer(TestModelFixture.GetNewTestModel(
                _testModelFixture._rbacModelText,
                _testModelFixture._rbacWithHierarchyPolicyText));
            e.BuildRoleLinks();

            TestGetPermissions(e, "alice", AsList(
                AsList("alice", "data1", "read")));
            TestGetPermissions(e, "bob", AsList(
                AsList("bob", "data2", "write")));

            TestGetImplicitPermissions(e, "alice", AsList(
                AsList("alice", "data1", "read"),
                AsList("data1_admin", "data1", "read"),
                AsList("data1_admin", "data1", "write"),
                AsList("data2_admin", "data2", "read"),
                AsList("data2_admin", "data2", "write")));
            TestGetImplicitPermissions(e, "bob", AsList(
                AsList("bob", "data2", "write")));
        }

        [Fact]
        public void TestGetImplicitPermissionsForUserWithDomain()
        {
            var e = new Enforcer(TestModelFixture.GetNewTestModel(
                _testModelFixture._rbacWithDomainsModelText,
                _testModelFixture._rbacWithHierarchyWithDomainsPolicyText));
            e.BuildRoleLinks();

            TestGetImplicitPermissions(e, "alice", AsList(
                AsList("alice", "domain1", "data2", "read"),
                AsList("role:reader", "domain1", "data1", "read"),
                AsList("role:writer", "domain1", "data1", "write")),
                "domain1");
        }

        [Fact]
        public void GetImplicitRolesForUser()
        {
            // Arrange
            var e = new Enforcer(TestModelFixture.GetNewTestModel(
                _testModelFixture._rbacModelText,
                _testModelFixture._rbacWithHierarchyPolicyText));
            e.BuildRoleLinks();

            // Assert
            TestGetPermissions(e, "alice", AsList(
                AsList("alice", "data1", "read")));
            TestGetPermissions(e, "bob", AsList(
                AsList("bob", "data2", "write")));
            Assert.Equal(new[] { "admin", "data1_admin", "data2_admin" },
                e.GetImplicitRolesForUser("alice"));
            Assert.Equal(new string[0],
                e.GetImplicitRolesForUser("bob"));
        }

        [Fact]
        public void TestGetImplicitUsersForPermission()
        {
            // Arrange
            var e = new Enforcer(TestModelFixture.GetNewTestModel(
                _testModelFixture._rbacModelText,
                _testModelFixture._rbacWithHierarchyPolicyText));
            e.BuildRoleLinks();

            Assert.Equal(new[] { "alice" }, e.GetImplicitUsersForPermission("data1", "read"));
            Assert.Equal(new[] { "alice" }, e.GetImplicitUsersForPermission("data1", "write"));
            Assert.Equal(new[] { "alice" }, e.GetImplicitUsersForPermission("data2", "read"));
            Assert.Equal(new[] { "alice", "bob" }, e.GetImplicitUsersForPermission("data2", "write"));

            // Act
            e.GetModel().ClearPolicy();
            _ = e.AddPolicy("admin", "data1", "read");
            _ = e.AddPolicy("bob", "data1", "read");
            _ = e.AddGroupingPolicy("alice", "admin");

            // Assert
            Assert.Equal(new[] { "bob", "alice" }, e.GetImplicitUsersForPermission("data1", "read"));
        }
    }
}
