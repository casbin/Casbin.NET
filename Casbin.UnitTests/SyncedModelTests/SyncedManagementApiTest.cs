using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.UnitTests.Fixtures;
using Xunit;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.SyncedModelTests;

[Collection("Model collection")]
public class SyncedManagementApiTest
{
    private readonly TestModelFixture _testModelFixture;

    public SyncedManagementApiTest(TestModelFixture testModelFixture) => _testModelFixture = testModelFixture;

    [Fact]
    public void TestGetPolicyApi()
    {
        IEnforcer e = SyncedEnforcer.Create(_testModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        TestGetPolicy(e, AsList(
            AsList("alice", "data1", "read"),
            AsList("bob", "data2", "write"),
            AsList("data2_admin", "data2", "read"),
            AsList("data2_admin", "data2", "write")));

        TestGetFilteredPolicy(e, 0, AsList(AsList("alice", "data1", "read")), "alice");
        TestGetFilteredPolicy(e, 0, AsList(AsList("bob", "data2", "write")), "bob");
        TestGetFilteredPolicy(e, 0,
            AsList(AsList("data2_admin", "data2", "read"), AsList("data2_admin", "data2", "write")), "data2_admin");
        TestGetFilteredPolicy(e, 1, AsList(AsList("alice", "data1", "read")), "data1");
        TestGetFilteredPolicy(e, 1,
            AsList(AsList("bob", "data2", "write"), AsList("data2_admin", "data2", "read"),
                AsList("data2_admin", "data2", "write")), "data2");
        TestGetFilteredPolicy(e, 2,
            AsList(AsList("alice", "data1", "read"), AsList("data2_admin", "data2", "read")), "read");
        TestGetFilteredPolicy(e, 2,
            AsList(AsList("bob", "data2", "write"), AsList("data2_admin", "data2", "write")), "write");

        TestGetFilteredPolicy(e, 0,
            AsList(AsList("data2_admin", "data2", "read"), AsList("data2_admin", "data2", "write")), "data2_admin",
            "data2");
        // Note: "" (empty string) in fieldValues means matching all values.
        TestGetFilteredPolicy(e, 0, AsList(AsList("data2_admin", "data2", "read")), "data2_admin", "", "read");
        TestGetFilteredPolicy(e, 1,
            AsList(AsList("bob", "data2", "write"), AsList("data2_admin", "data2", "write")), "data2", "write");

        TestHasPolicy(e, AsList("alice", "data1", "read"), true);
        TestHasPolicy(e, AsList("bob", "data2", "write"), true);
        TestHasPolicy(e, AsList("alice", "data2", "read"), false);
        TestHasPolicy(e, AsList("bob", "data3", "write"), false);

        TestGetGroupingPolicy(e, AsList(AsList("alice", "data2_admin")));

        TestGetFilteredGroupingPolicy(e, 0, AsList(AsList("alice", "data2_admin")), "alice");
        TestGetFilteredGroupingPolicy(e, 0, new List<List<string>>(), "bob");
        TestGetFilteredGroupingPolicy(e, 1, new List<List<string>>(), "data1_admin");
        TestGetFilteredGroupingPolicy(e, 1, AsList(AsList("alice", "data2_admin")), "data2_admin");
        // Note: "" (empty string) in fieldValues means matching all values.
        TestGetFilteredGroupingPolicy(e, 0, AsList(AsList("alice", "data2_admin")), "", "data2_admin");

        TestHasGroupingPolicy(e, AsList("alice", "data2_admin"), true);
        TestHasGroupingPolicy(e, AsList("bob", "data2_admin"), false);
    }

    [Fact]
    public void TestModifyPolicy()
    {
        IEnforcer e = SyncedEnforcer.Create(_testModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        TestGetPolicy(e, AsList(
            AsList("alice", "data1", "read"),
            AsList("bob", "data2", "write"),
            AsList("data2_admin", "data2", "read"),
            AsList("data2_admin", "data2", "write")));

        e.RemovePolicy("alice", "data1", "read");
        e.RemovePolicy("bob", "data2", "write");
        e.RemovePolicy("alice", "data1", "read");
        e.AddPolicy("eve", "data3", "read");
        e.AddPolicy("eve", "data3", "read");

        List<List<string>> rules = AsList(
            AsList("jack", "data4", "read"),
            AsList("jack", "data4", "read"),
            AsList("jack", "data4", "read"),
            AsList("katy", "data4", "write"),
            AsList("leyo", "data4", "read"),
            AsList("katy", "data4", "write"),
            AsList("katy", "data4", "write"),
            AsList("ham", "data4", "write")
        );

        _ = e.AddPolicies(rules);
        _ = e.AddPolicies(rules);

        TestGetPolicy(e, AsList(
                AsList("data2_admin", "data2", "read"),
                AsList("data2_admin", "data2", "write"),
                AsList("eve", "data3", "read"),
                AsList("jack", "data4", "read"),
                AsList("katy", "data4", "write"),
                AsList("leyo", "data4", "read"),
                AsList("ham", "data4", "write")
            )
        );

        _ = e.RemovePolicies(rules);
        _ = e.RemovePolicies(rules);

        List<string> namedPolicy = AsList("eve", "data3", "read");
        e.RemoveNamedPolicy("p", namedPolicy);
        e.AddNamedPolicy("p", namedPolicy);

        TestGetPolicy(e, AsList(
            AsList("data2_admin", "data2", "read"),
            AsList("data2_admin", "data2", "write"),
            AsList("eve", "data3", "read")));

        e.RemoveFilteredPolicy(1, "data2");
        TestGetPolicy(e, AsList(AsList("eve", "data3", "read")));

        e.RemoveFilteredPolicy(1);
        TestGetPolicy(e, AsList(AsList("eve", "data3", "read")));

        e.RemoveFilteredPolicy(1, "");
        TestGetPolicy(e, AsList(AsList("eve", "data3", "read")));

        bool res = e.UpdatePolicy(AsList("eve", "data3", "read"), AsList("eve", "data3", "write"));
        TestGetPolicy(e, AsList(AsList("eve", "data3", "write")));
        Assert.True(res);

        // This test shows that a non-existent policy will not be updated.
        res = e.UpdatePolicy(AsList("non_exist", "data3", "write"), AsList("non_exist", "data3", "read"));
        TestGetPolicy(e, AsList(AsList("eve", "data3", "write")));
        Assert.False(res);

        e.AddPolicies(rules);
        TestGetPolicy(e, AsList(
            AsList("eve", "data3", "write"),
            AsList("jack", "data4", "read"),
            AsList("katy", "data4", "write"),
            AsList("leyo", "data4", "read"),
            AsList("ham", "data4", "write")));

        res = e.UpdatePolicies(
            AsList(
                AsList("eve", "data3", "write"),
                AsList("leyo", "data4", "read"),
                AsList("katy", "data4", "write")),
            AsList(
                AsList("eve", "data3", "read"),
                AsList("leyo", "data4", "write"),
                AsList("katy", "data1", "write")));
        TestGetPolicy(e, AsList(
            AsList("eve", "data3", "read"),
            AsList("jack", "data4", "read"),
            AsList("katy", "data1", "write"),
            AsList("leyo", "data4", "write"),
            AsList("ham", "data4", "write")));
        Assert.True(res);

        // This test shows that a non-existent policy in oldParameters will not be updated, so other existent ones
        // will be ignored and the return value will be False.
        res = e.UpdatePolicies(
            AsList(
                AsList("eve", "data3", "read"), AsList("non_exist", "data4", "read")),
            AsList(
                AsList("eve", "data3", "write"), AsList("non_exist", "data4", "write")));
        TestGetPolicy(e, AsList(
            AsList("eve", "data3", "read"),
            AsList("jack", "data4", "read"),
            AsList("katy", "data1", "write"),
            AsList("leyo", "data4", "write"),
            AsList("ham", "data4", "write")));
        Assert.False(res);

        // If oldRules' length is not the same as newRules', no rules will be updated.
        res = e.UpdatePolicies(
            AsList(
                AsList("eve", "data3", "read"), AsList("leyo", "data4", "write")),
            AsList(AsList("eve", "data3", "write")));
        TestGetPolicy(e, AsList(
            AsList("eve", "data3", "read"),
            AsList("jack", "data4", "read"),
            AsList("katy", "data1", "write"),
            AsList("leyo", "data4", "write"),
            AsList("ham", "data4", "write")));
        Assert.False(res);
    }

    [Fact]
    public async Task TestModifyPolicyAsync()
    {
        IEnforcer e = SyncedEnforcer.Create(_testModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        TestGetPolicy(e, AsList(
            AsList("alice", "data1", "read"),
            AsList("bob", "data2", "write"),
            AsList("data2_admin", "data2", "read"),
            AsList("data2_admin", "data2", "write")));

        await e.RemovePolicyAsync("alice", "data1", "read");
        await e.RemovePolicyAsync("bob", "data2", "write");
        await e.RemovePolicyAsync("alice", "data1", "read");
        await e.AddPolicyAsync("eve", "data3", "read");
        await e.AddPolicyAsync("eve", "data3", "read");

        List<List<string>> rules = AsList(
            AsList("jack", "data4", "read"),
            AsList("jack", "data4", "read"),
            AsList("jack", "data4", "read"),
            AsList("katy", "data4", "write"),
            AsList("leyo", "data4", "read"),
            AsList("katy", "data4", "write"),
            AsList("katy", "data4", "write"),
            AsList("ham", "data4", "write")
        );

        _ = await e.AddPoliciesAsync(rules);
        _ = await e.AddPoliciesAsync(rules);

        TestGetPolicy(e, AsList(
                AsList("data2_admin", "data2", "read"),
                AsList("data2_admin", "data2", "write"),
                AsList("eve", "data3", "read"),
                AsList("jack", "data4", "read"),
                AsList("katy", "data4", "write"),
                AsList("leyo", "data4", "read"),
                AsList("ham", "data4", "write")
            )
        );

        _ = await e.RemovePoliciesAsync(rules);
        _ = await e.RemovePoliciesAsync(rules);

        List<string> namedPolicy = AsList("eve", "data3", "read");
        await e.RemoveNamedPolicyAsync("p", namedPolicy);
        await e.AddNamedPolicyAsync("p", namedPolicy);

        TestGetPolicy(e, AsList(
            AsList("data2_admin", "data2", "read"),
            AsList("data2_admin", "data2", "write"),
            AsList("eve", "data3", "read")));

        await e.RemoveFilteredPolicyAsync(1, "data2");

        TestGetPolicy(e, AsList(AsList("eve", "data3", "read")));

        bool res = await e.UpdatePolicyAsync(AsList("eve", "data3", "read"), AsList("eve", "data3", "write"));
        TestGetPolicy(e, AsList(AsList("eve", "data3", "write")));
        Assert.True(res);

        // This test shows that a non-existent policy will not be updated.
        res = await e.UpdatePolicyAsync(AsList("non_exist", "data3", "write"),
            AsList("non_exist", "data3", "read"));
        TestGetPolicy(e, AsList(AsList("eve", "data3", "write")));
        Assert.False(res);

        await e.AddPoliciesAsync(rules);
        TestGetPolicy(e, AsList(
            AsList("eve", "data3", "write"),
            AsList("jack", "data4", "read"),
            AsList("katy", "data4", "write"),
            AsList("leyo", "data4", "read"),
            AsList("ham", "data4", "write")));

        res = await e.UpdatePoliciesAsync(
            AsList(
                AsList("eve", "data3", "write"),
                AsList("leyo", "data4", "read"),
                AsList("katy", "data4", "write")),
            AsList(
                AsList("eve", "data3", "read"),
                AsList("leyo", "data4", "write"),
                AsList("katy", "data1", "write")));
        TestGetPolicy(e, AsList(
            AsList("eve", "data3", "read"),
            AsList("jack", "data4", "read"),
            AsList("katy", "data1", "write"),
            AsList("leyo", "data4", "write"),
            AsList("ham", "data4", "write")));
        Assert.True(res);

        // This test shows that a non-existent policy in oldParameters will not be updated, so other existent ones
        // will be ignored and the return value will be False.
        res = await e.UpdatePoliciesAsync(
            AsList(
                AsList("eve", "data3", "read"), AsList("non_exist", "data4", "read")),
            AsList(
                AsList("eve", "data3", "write"), AsList("non_exist", "data4", "write")));
        TestGetPolicy(e, AsList(
            AsList("eve", "data3", "read"),
            AsList("jack", "data4", "read"),
            AsList("katy", "data1", "write"),
            AsList("leyo", "data4", "write"),
            AsList("ham", "data4", "write")));
        Assert.False(res);

        // If oldRules' length is not the same as newRules', no rules will be updated.
        res = await e.UpdatePoliciesAsync(
            AsList(
                AsList("eve", "data3", "read"), AsList("leyo", "data4", "write")),
            AsList(AsList("eve", "data3", "write")));
        TestGetPolicy(e, AsList(
            AsList("eve", "data3", "read"),
            AsList("jack", "data4", "read"),
            AsList("katy", "data1", "write"),
            AsList("leyo", "data4", "write"),
            AsList("ham", "data4", "write")));
        Assert.False(res);
    }

    [Fact]
    public void TestModifyGroupingPolicy()
    {
        IEnforcer e = SyncedEnforcer.Create(_testModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        TestGetRoles(e, "alice", AsList("data2_admin"));
        TestGetRoles(e, "bob", AsList());
        TestGetRoles(e, "eve", AsList());
        TestGetRoles(e, "non_exist", AsList());

        e.RemoveGroupingPolicy("alice", "data2_admin");
        e.AddGroupingPolicy("bob", "data1_admin");
        e.AddGroupingPolicy("eve", "data3_admin");

        List<List<string>> groupingRules = AsList(
            AsList("ham", "data4_admin"),
            AsList("jack", "data5_admin")
        );

        _ = e.AddGroupingPolicies(groupingRules);
        TestGetRoles(e, "ham", AsList("data4_admin"));
        TestGetRoles(e, "jack", AsList("data5_admin"));
        _ = e.RemoveGroupingPolicies(groupingRules);

        TestGetRoles(e, "alice", AsList());

        List<string> namedGroupingPolicy = AsList("alice", "data2_admin");
        TestGetRoles(e, "alice", AsList());
        e.AddNamedGroupingPolicy("g", namedGroupingPolicy);
        TestGetRoles(e, "alice", AsList("data2_admin"));
        e.RemoveNamedGroupingPolicy("g", namedGroupingPolicy);

        TestGetRoles(e, "alice", AsList());
        TestGetRoles(e, "bob", AsList("data1_admin"));
        TestGetRoles(e, "eve", AsList("data3_admin"));
        TestGetRoles(e, "non_exist", AsList());

        TestGetUsers(e, "data1_admin", AsList("bob"));
        TestGetUsers(e, "data2_admin", AsList());
        TestGetUsers(e, "data3_admin", AsList("eve"));

        e.RemoveFilteredGroupingPolicy(0, "bob");

        TestGetRoles(e, "alice", AsList());
        TestGetRoles(e, "bob", AsList());
        TestGetRoles(e, "eve", AsList("data3_admin"));
        TestGetRoles(e, "non_exist", AsList());

        TestGetUsers(e, "data1_admin", AsList());
        TestGetUsers(e, "data2_admin", AsList());
        TestGetUsers(e, "data3_admin", AsList("eve"));

        e.AddGroupingPolicy("data3_admin", "data4_admin");
        bool res = e.UpdateGroupingPolicy(AsList("eve", "data3_admin"), AsList("eve", "admin"));
        Assert.True(res);
        res = e.UpdateGroupingPolicy(AsList("data3_admin", "data4_admin"), AsList("admin", "data4_admin"));
        Assert.True(res);
        TestGetUsers(e, "data4_admin", AsList("admin"));
        TestGetUsers(e, "admin", AsList("eve"));
        TestGetRoles(e, "eve", AsList("admin"));
        TestGetRoles(e, "admin", AsList("data4_admin"));

        res = e.UpdateGroupingPolicy(AsList("non_exist", "data4_admin"), AsList("non_exist2", "data4_admin"));
        Assert.False(res);
        TestGetUsers(e, "data4_admin", AsList("admin"));

        res = e.UpdateGroupingPolicies(
            AsList(
                AsList("eve", "admin"),
                AsList("admin", "data4_admin")),
            AsList(
                AsList("eve", "admin_groups"),
                AsList("admin", "data5_admin")));

        Assert.True(res);
        TestGetUsers(e, "data5_admin", AsList("admin"));
        TestGetUsers(e, "admin_groups", AsList("eve"));
        TestGetRoles(e, "admin", AsList("data5_admin"));
        TestGetRoles(e, "eve", AsList("admin_groups"));

        res = e.UpdateGroupingPolicies(
            AsList(
                AsList("admin", "data5_admin"),
                AsList("non_exist", "admin_groups")
            ),
            AsList(
                AsList("admin", "data6_admin"),
                AsList("non_exist2", "admin_groups")
            ));
        Assert.False(res);
        TestGetRoles(e, "admin", AsList("data5_admin"));
        TestGetRoles(e, "eve", AsList("admin_groups"));

        // If oldRules' length is not the same as newRules', no rules will be updated.
        res = e.UpdateGroupingPolicies(
            AsList(
                AsList("admin", "data5_admin"),
                AsList("eve", "admin2_groups")),
            AsList(
                AsList("admin", "data6_admin")));
        Assert.False(res);
        TestGetRoles(e, "admin", AsList("data5_admin"));
        TestGetRoles(e, "eve", AsList("admin_groups"));
    }

    [Fact]
    public async Task TestModifyGroupingPolicyAsync()
    {
        IEnforcer e = SyncedEnforcer.Create(_testModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        TestGetRoles(e, "alice", AsList("data2_admin"));
        TestGetRoles(e, "bob", AsList());
        TestGetRoles(e, "eve", AsList());
        TestGetRoles(e, "non_exist", AsList());

        await e.RemoveGroupingPolicyAsync("alice", "data2_admin");
        await e.AddGroupingPolicyAsync("bob", "data1_admin");
        await e.AddGroupingPolicyAsync("eve", "data3_admin");

        List<List<string>> groupingRules = AsList(
            AsList("ham", "data4_admin"),
            AsList("jack", "data5_admin")
        );

        _ = await e.AddGroupingPoliciesAsync(groupingRules);
        TestGetRoles(e, "ham", AsList("data4_admin"));
        TestGetRoles(e, "jack", AsList("data5_admin"));
        _ = await e.RemoveGroupingPoliciesAsync(groupingRules);

        TestGetRoles(e, "alice", AsList());
        List<string> namedGroupingPolicy = AsList("alice", "data2_admin");
        TestGetRoles(e, "alice", AsList());
        await e.AddNamedGroupingPolicyAsync("g", namedGroupingPolicy);
        TestGetRoles(e, "alice", AsList("data2_admin"));
        await e.RemoveNamedGroupingPolicyAsync("g", namedGroupingPolicy);

        TestGetRoles(e, "alice", AsList());
        TestGetRoles(e, "bob", AsList("data1_admin"));
        TestGetRoles(e, "eve", AsList("data3_admin"));
        TestGetRoles(e, "non_exist", AsList());

        TestGetUsers(e, "data1_admin", AsList("bob"));
        TestGetUsers(e, "data2_admin", AsList());
        TestGetUsers(e, "data3_admin", AsList("eve"));

        await e.RemoveFilteredGroupingPolicyAsync(0, "bob");

        TestGetRoles(e, "alice", AsList());
        TestGetRoles(e, "bob", AsList());
        TestGetRoles(e, "eve", AsList("data3_admin"));
        TestGetRoles(e, "non_exist", AsList());

        TestGetUsers(e, "data1_admin", AsList());
        TestGetUsers(e, "data2_admin", AsList());
        TestGetUsers(e, "data3_admin", AsList("eve"));

        await e.AddGroupingPolicyAsync("data3_admin", "data4_admin");
        bool res = await e.UpdateGroupingPolicyAsync(AsList("eve", "data3_admin"), AsList("eve", "admin"));
        Assert.True(res);
        res = await e.UpdateGroupingPolicyAsync(AsList("data3_admin", "data4_admin"),
            AsList("admin", "data4_admin"));
        Assert.True(res);
        TestGetUsers(e, "data4_admin", AsList("admin"));
        TestGetUsers(e, "admin", AsList("eve"));
        TestGetRoles(e, "eve", AsList("admin"));
        TestGetRoles(e, "admin", AsList("data4_admin"));

        res = await e.UpdateGroupingPolicyAsync(AsList("non_exist", "data4_admin"),
            AsList("non_exist2", "data4_admin"));
        Assert.False(res);
        TestGetUsers(e, "data4_admin", AsList("admin"));

        res = await e.UpdateGroupingPoliciesAsync(
            AsList(
                AsList("eve", "admin"),
                AsList("admin", "data4_admin")),
            AsList(
                AsList("eve", "admin_groups"),
                AsList("admin", "data5_admin")));

        Assert.True(res);
        TestGetUsers(e, "data5_admin", AsList("admin"));
        TestGetUsers(e, "admin_groups", AsList("eve"));
        TestGetRoles(e, "admin", AsList("data5_admin"));
        TestGetRoles(e, "eve", AsList("admin_groups"));

        res = await e.UpdateGroupingPoliciesAsync(
            AsList(
                AsList("admin", "data5_admin"),
                AsList("non_exist", "admin_groups")
            ),
            AsList(
                AsList("admin", "data6_admin"),
                AsList("non_exist2", "admin_groups")
            ));
        Assert.False(res);
        TestGetRoles(e, "admin", AsList("data5_admin"));
        TestGetRoles(e, "eve", AsList("admin_groups"));

        // If oldRules' length is not the same as newRules', no rules will be updated.
        res = await e.UpdateGroupingPoliciesAsync(
            AsList(
                AsList("admin", "data5_admin"),
                AsList("eve", "admin2_groups")),
            AsList(
                AsList("admin", "data6_admin")));
        Assert.False(res);
        TestGetRoles(e, "admin", AsList("data5_admin"));
        TestGetRoles(e, "eve", AsList("admin_groups"));
    }
}
