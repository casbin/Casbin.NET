using System.Threading.Tasks;
using Casbin.UnitTests.Fixtures;
using Xunit;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.ModelTests;

[Collection("Model collection")]
public class RbacApiTest
{
    private readonly TestModelFixture TestModelFixture;

    public RbacApiTest(TestModelFixture testModelFixture) => TestModelFixture = testModelFixture;

    [Fact]
    public void TestRoleApi()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        TestGetRoles(e, "alice", AsList("data2_admin"));
        TestGetRoles(e, "bob", AsList());
        TestGetRoles(e, "data2_admin", AsList());
        TestGetRoles(e, "non_exist", AsList());

        Assert.False(e.HasRoleForUser("alice", "data1_admin"));
        Assert.True(e.HasRoleForUser("alice", "data2_admin"));

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

        Assert.False(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));

        Assert.True(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));

        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));

        e.DeleteRole("data2_admin");

        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestRoleApiAsync()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        TestGetRoles(e, "alice", AsList("data2_admin"));
        TestGetRoles(e, "bob", AsList());
        TestGetRoles(e, "data2_admin", AsList());
        TestGetRoles(e, "non_exist", AsList());

        Assert.False(e.HasRoleForUser("alice", "data1_admin"));
        Assert.True(e.HasRoleForUser("alice", "data2_admin"));

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

        Assert.False(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));

        Assert.True(await e.EnforceAsync("alice", "data2", "read"));
        Assert.True(await e.EnforceAsync("alice", "data2", "write"));

        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));

        await e.DeleteRoleAsync("data2_admin");

        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestRoleApiWithDomains()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacWithDomainsTestModel());
        e.BuildRoleLinks();

        Assert.True(e.HasRoleForUser("alice", "admin", "domain1"));
        Assert.False(e.HasRoleForUser("alice", "admin", "domain2"));

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
        Enforcer e = new(TestModelFixture.GetNewRbacWithDomainsTestModel());
        e.BuildRoleLinks();

        Assert.True(e.HasRoleForUser("alice", "admin", "domain1"));
        Assert.False(e.HasRoleForUser("alice", "admin", "domain2"));

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
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        _ = e.AddRolesForUser("alice", AsList("data1_admin", "data2_admin", "data3_admin"));
        // The "alice" already has "data2_admin" , it will be return false. So "alice" just has "data2_admin".
        TestGetRoles(e, "alice", AsList("data2_admin"));
        // delete role
        _ = e.DeleteRoleForUser("alice", "data2_admin");

        _ = e.AddRolesForUser("alice", AsList("data1_admin", "data2_admin", "data3_admin"));
        TestGetRoles(e, "alice", AsList("data1_admin", "data2_admin", "data3_admin"));
        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.True(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));
    }

    [Fact]
    public async Task TestAddRolesForUserAsync()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        _ = await e.AddRolesForUserAsync("alice", AsList("data1_admin", "data2_admin", "data3_admin"));
        // The "alice" already has "data2_admin" , it will be return false. So "alice" just has "data2_admin".
        TestGetRoles(e, "alice", AsList("data2_admin"));
        // delete role
        _ = await e.DeleteRoleForUserAsync("alice", "data2_admin");

        _ = await e.AddRolesForUserAsync("alice", AsList("data1_admin", "data2_admin", "data3_admin"));
        TestGetRoles(e, "alice", AsList("data1_admin", "data2_admin", "data3_admin"));
        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.True(await e.EnforceAsync("alice", "data2", "read"));
        Assert.True(await e.EnforceAsync("alice", "data2", "write"));
    }

    [Fact]
    public void TestPermissionApi()
    {
        Enforcer e = new(TestModelFixture.GetBasicWithoutResourceTestModel());
        e.BuildRoleLinks();

        Assert.True(e.Enforce("alice", "read"));
        Assert.False(e.Enforce("alice", "write"));
        Assert.False(e.Enforce("bob", "read"));
        Assert.True(e.Enforce("bob", "write"));

        TestGetPermissions(e, "alice", AsList(AsList("alice", "read")));
        TestGetPermissions(e, "bob", AsList(AsList("bob", "write")));

        Assert.True(e.HasPermissionForUser("alice", "read"));
        Assert.False(e.HasPermissionForUser("alice", "write"));
        Assert.False(e.HasPermissionForUser("bob", "read"));
        Assert.True(e.HasPermissionForUser("bob", "write"));

        _ = e.DeletePermission("read");

        Assert.False(e.Enforce("alice", "read"));
        Assert.False(e.Enforce("alice", "write"));
        Assert.False(e.Enforce("bob", "read"));
        Assert.True(e.Enforce("bob", "write"));

        _ = e.AddPermissionForUser("bob", "read");

        Assert.False(e.Enforce("alice", "read"));
        Assert.False(e.Enforce("alice", "write"));
        Assert.True(e.Enforce("bob", "read"));
        Assert.True(e.Enforce("bob", "write"));

        _ = e.DeletePermissionForUser("bob", "read");

        Assert.False(e.Enforce("alice", "read"));
        Assert.False(e.Enforce("alice", "write"));
        Assert.False(e.Enforce("bob", "read"));
        Assert.True(e.Enforce("bob", "write"));

        _ = e.DeletePermissionsForUser("bob");

        Assert.False(e.Enforce("alice", "read"));
        Assert.False(e.Enforce("alice", "write"));
        Assert.False(e.Enforce("bob", "read"));
        Assert.False(e.Enforce("bob", "write"));
    }

    [Fact]
    public async Task TestPermissionApiAsync()
    {
        Enforcer e = new(TestModelFixture.GetBasicWithoutResourceTestModel());
        e.BuildRoleLinks();

        Assert.True(await e.EnforceAsync("alice", "read"));
        Assert.False(await e.EnforceAsync("alice", "write"));
        Assert.False(await e.EnforceAsync("bob", "read"));
        Assert.True(await e.EnforceAsync("bob", "write"));

        TestGetPermissions(e, "alice", AsList(AsList("alice", "read")));
        TestGetPermissions(e, "bob", AsList(AsList("bob", "write")));

        Assert.True(e.HasPermissionForUser("alice", "read"));
        Assert.False(e.HasPermissionForUser("alice", "write"));
        Assert.False(e.HasPermissionForUser("bob", "read"));
        Assert.True(e.HasPermissionForUser("bob", "write"));

        _ = await e.DeletePermissionAsync("read");

        Assert.False(await e.EnforceAsync("alice", "read"));
        Assert.False(await e.EnforceAsync("alice", "write"));
        Assert.False(await e.EnforceAsync("bob", "read"));
        Assert.True(await e.EnforceAsync("bob", "write"));

        _ = await e.AddPermissionForUserAsync("bob", "read");

        Assert.False(await e.EnforceAsync("alice", "read"));
        Assert.False(await e.EnforceAsync("alice", "write"));
        Assert.True(await e.EnforceAsync("bob", "read"));
        Assert.True(await e.EnforceAsync("bob", "write"));

        _ = await e.DeletePermissionForUserAsync("bob", "read");

        Assert.False(await e.EnforceAsync("alice", "read"));
        Assert.False(await e.EnforceAsync("alice", "write"));
        Assert.False(await e.EnforceAsync("bob", "read"));
        Assert.True(await e.EnforceAsync("bob", "write"));

        _ = await e.DeletePermissionsForUserAsync("bob");

        Assert.False(await e.EnforceAsync("alice", "read"));
        Assert.False(await e.EnforceAsync("alice", "write"));
        Assert.False(await e.EnforceAsync("bob", "read"));
        Assert.False(await e.EnforceAsync("bob", "write"));
    }

    [Fact]
    public void TestGetImplicitPermissionsForUser()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacModelText,
            TestModelFixture.RbacWithHierarchyPolicyText));
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
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacWithDomainsModelText,
            TestModelFixture.RbacWithHierarchyWithDomainsPolicyText));
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
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacModelText,
            TestModelFixture.RbacWithHierarchyPolicyText));
        e.BuildRoleLinks();

        // Assert
        TestGetPermissions(e, "alice", AsList(
            AsList("alice", "data1", "read")));
        TestGetPermissions(e, "bob", AsList(
            AsList("bob", "data2", "write")));
        Assert.Equal(["admin", "data1_admin", "data2_admin"],
            e.GetImplicitRolesForUser("alice"));
        Assert.Equal(new string[0],
            e.GetImplicitRolesForUser("bob"));
    }

    [Fact]
    public void TestGetImplicitUsersForPermission()
    {
        // Arrange
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacModelText,
            TestModelFixture.RbacWithHierarchyPolicyText));
        e.BuildRoleLinks();

        Assert.Equal(["alice"], e.GetImplicitUsersForPermission("data1", "read"));
        Assert.Equal(["alice"], e.GetImplicitUsersForPermission("data1", "write"));
        Assert.Equal(["alice"], e.GetImplicitUsersForPermission("data2", "read"));
        Assert.Equal(["alice", "bob"], e.GetImplicitUsersForPermission("data2", "write"));

        // Act
        e.ClearPolicy();
        _ = e.AddPolicy("admin", "data1", "read");
        _ = e.AddPolicy("bob", "data1", "read");
        _ = e.AddGroupingPolicy("alice", "admin");

        // Assert
        Assert.Equal(["bob", "alice"], e.GetImplicitUsersForPermission("data1", "read"));
    }
}
