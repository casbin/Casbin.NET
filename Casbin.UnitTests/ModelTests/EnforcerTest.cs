using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Casbin.Adapter.File;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Rbac;
using Casbin.UnitTests.Fixtures;
using Casbin.UnitTests.Mock;
using Xunit;
using Xunit.Abstractions;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.ModelTests;

[Collection("Model collection")]
public class EnforcerTest
{
    private readonly TestModelFixture _testModelFixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public EnforcerTest(ITestOutputHelper testOutputHelper, TestModelFixture testModelFixture)
    {
        _testOutputHelper = testOutputHelper;
        _testModelFixture = testModelFixture;
    }

    [Fact]
    public void TestEnforceWithMultipleRoleManager()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            _testModelFixture._rbacMultipleModelText,
            _testModelFixture._rbacMultiplePolicyText));

        DefaultRoleManager roleManager = new(5);
        roleManager.AddMatchingFunc((arg1, arg2) => arg1.Equals(arg2));
        e.SetRoleManager(roleManager);
        bool result = e.Enforce("@adm-user", "org::customer1", "cust1", "manage");
        Assert.True(result);

        roleManager.AddMatchingFunc((arg1, arg2) => !arg1.Equals(arg2));
        e.SetRoleManager(roleManager);
        result = e.Enforce("@adm-user", "org::customer1", "cust1", "manage");
        Assert.False(result);
    }

    [Fact]
    public void TestEnforceWithMultipleEval()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            _testModelFixture._rbacMultipleEvalModelText,
            _testModelFixture._rbacMultipleEvalPolicyText));

        bool result = e.Enforce(
            "domain1",
            new { Role = "admin" },
            new { Name = "admin_panel" },
            "view");

        Assert.True(result);
    }

    [Fact]
    public void TestEnforceWithLazyLoadPolicy()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        FileAdapter a = new("examples/keymatch_policy.csv");

        IEnforcer e = DefaultEnforcer.Create(m, a, true);
        Assert.Empty(e.GetPolicy());

        e = DefaultEnforcer.Create(m, a);
        Assert.NotEmpty(e.GetPolicy());
    }

    [Fact]
    public void TestEnforceSubjectPriority()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            _testModelFixture._subjectPriorityModelText,
            _testModelFixture._subjectPriorityPolicyText));

        TestEnforce(e, "jane", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "read", true);
    }

    [Fact]
    public void TestEnforceSubjectPriorityWithDomain()
    {
        Enforcer e = new(
            Path.Combine("Examples", "subject_priority_model_with_domain.conf"),
            Path.Combine("Examples", "subject_priority_policy_with_domain.csv"));

        Assert.True(e.Enforce(
            "alice",
            "data1",
            "domain1",
            "write"));
        Assert.True(e.Enforce(
            "bob",
            "data2",
            "domain2",
            "write"));
    }

    #region In memory model

    [Fact]
    public void TestKeyMatchModelInMemory()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        FileAdapter a = new("examples/keymatch_policy.csv");

        Enforcer e = new(m, a);

        TestEnforce(e, "alice", "/alice_data/resource1", "GET", true);
        TestEnforce(e, "alice", "/alice_data/resource1", "POST", true);
        TestEnforce(e, "alice", "/alice_data/resource2", "GET", true);
        TestEnforce(e, "alice", "/alice_data/resource2", "POST", false);
        TestEnforce(e, "alice", "/bob_data/resource1", "GET", false);
        TestEnforce(e, "alice", "/bob_data/resource1", "POST", false);
        TestEnforce(e, "alice", "/bob_data/resource2", "GET", false);
        TestEnforce(e, "alice", "/bob_data/resource2", "POST", false);

        TestEnforce(e, "bob", "/alice_data/resource1", "GET", false);
        TestEnforce(e, "bob", "/alice_data/resource1", "POST", false);
        TestEnforce(e, "bob", "/alice_data/resource2", "GET", true);
        TestEnforce(e, "bob", "/alice_data/resource2", "POST", false);
        TestEnforce(e, "bob", "/bob_data/resource1", "GET", false);
        TestEnforce(e, "bob", "/bob_data/resource1", "POST", true);
        TestEnforce(e, "bob", "/bob_data/resource2", "GET", false);
        TestEnforce(e, "bob", "/bob_data/resource2", "POST", true);

        TestEnforce(e, "cathy", "/cathy_data", "GET", true);
        TestEnforce(e, "cathy", "/cathy_data", "POST", true);
        TestEnforce(e, "cathy", "/cathy_data", "DELETE", false);

        e = new Enforcer(m);
        a.LoadPolicy(e.Model);

        TestEnforce(e, "alice", "/alice_data/resource1", "GET", true);
        TestEnforce(e, "alice", "/alice_data/resource1", "POST", true);
        TestEnforce(e, "alice", "/alice_data/resource2", "GET", true);
        TestEnforce(e, "alice", "/alice_data/resource2", "POST", false);
        TestEnforce(e, "alice", "/bob_data/resource1", "GET", false);
        TestEnforce(e, "alice", "/bob_data/resource1", "POST", false);
        TestEnforce(e, "alice", "/bob_data/resource2", "GET", false);
        TestEnforce(e, "alice", "/bob_data/resource2", "POST", false);

        TestEnforce(e, "bob", "/alice_data/resource1", "GET", false);
        TestEnforce(e, "bob", "/alice_data/resource1", "POST", false);
        TestEnforce(e, "bob", "/alice_data/resource2", "GET", true);
        TestEnforce(e, "bob", "/alice_data/resource2", "POST", false);
        TestEnforce(e, "bob", "/bob_data/resource1", "GET", false);
        TestEnforce(e, "bob", "/bob_data/resource1", "POST", true);
        TestEnforce(e, "bob", "/bob_data/resource2", "GET", false);
        TestEnforce(e, "bob", "/bob_data/resource2", "POST", true);

        TestEnforce(e, "cathy", "/cathy_data", "GET", true);
        TestEnforce(e, "cathy", "/cathy_data", "POST", true);
        TestEnforce(e, "cathy", "/cathy_data", "DELETE", false);
    }

    [Fact]
    public async Task TestKeyMatchModelInMemoryAsync()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        FileAdapter a = new("examples/keymatch_policy.csv");

        Enforcer e = new(m, a);

        await TestEnforceAsync(e, "alice", "/alice_data/resource1", "GET", true);
        await TestEnforceAsync(e, "alice", "/alice_data/resource1", "POST", true);
        await TestEnforceAsync(e, "alice", "/alice_data/resource2", "GET", true);
        await TestEnforceAsync(e, "alice", "/alice_data/resource2", "POST", false);
        await TestEnforceAsync(e, "alice", "/bob_data/resource1", "GET", false);
        await TestEnforceAsync(e, "alice", "/bob_data/resource1", "POST", false);
        await TestEnforceAsync(e, "alice", "/bob_data/resource2", "GET", false);
        await TestEnforceAsync(e, "alice", "/bob_data/resource2", "POST", false);

        await TestEnforceAsync(e, "bob", "/alice_data/resource1", "GET", false);
        await TestEnforceAsync(e, "bob", "/alice_data/resource1", "POST", false);
        await TestEnforceAsync(e, "bob", "/alice_data/resource2", "GET", true);
        await TestEnforceAsync(e, "bob", "/alice_data/resource2", "POST", false);
        await TestEnforceAsync(e, "bob", "/bob_data/resource1", "GET", false);
        await TestEnforceAsync(e, "bob", "/bob_data/resource1", "POST", true);
        await TestEnforceAsync(e, "bob", "/bob_data/resource2", "GET", false);
        await TestEnforceAsync(e, "bob", "/bob_data/resource2", "POST", true);

        await TestEnforceAsync(e, "cathy", "/cathy_data", "GET", true);
        await TestEnforceAsync(e, "cathy", "/cathy_data", "POST", true);
        await TestEnforceAsync(e, "cathy", "/cathy_data", "DELETE", false);

        e = new Enforcer(m);
        await a.LoadPolicyAsync(e.Model);

        await TestEnforceAsync(e, "alice", "/alice_data/resource1", "GET", true);
        await TestEnforceAsync(e, "alice", "/alice_data/resource1", "POST", true);
        await TestEnforceAsync(e, "alice", "/alice_data/resource2", "GET", true);
        await TestEnforceAsync(e, "alice", "/alice_data/resource2", "POST", false);
        await TestEnforceAsync(e, "alice", "/bob_data/resource1", "GET", false);
        await TestEnforceAsync(e, "alice", "/bob_data/resource1", "POST", false);
        await TestEnforceAsync(e, "alice", "/bob_data/resource2", "GET", false);
        await TestEnforceAsync(e, "alice", "/bob_data/resource2", "POST", false);

        await TestEnforceAsync(e, "bob", "/alice_data/resource1", "GET", false);
        await TestEnforceAsync(e, "bob", "/alice_data/resource1", "POST", false);
        await TestEnforceAsync(e, "bob", "/alice_data/resource2", "GET", true);
        await TestEnforceAsync(e, "bob", "/alice_data/resource2", "POST", false);
        await TestEnforceAsync(e, "bob", "/bob_data/resource1", "GET", false);
        await TestEnforceAsync(e, "bob", "/bob_data/resource1", "POST", true);
        await TestEnforceAsync(e, "bob", "/bob_data/resource2", "GET", false);
        await TestEnforceAsync(e, "bob", "/bob_data/resource2", "POST", true);

        await TestEnforceAsync(e, "cathy", "/cathy_data", "GET", true);
        await TestEnforceAsync(e, "cathy", "/cathy_data", "POST", true);
        await TestEnforceAsync(e, "cathy", "/cathy_data", "DELETE", false);
    }

    [Fact]
    public void TestKeyMatchModelInMemoryDeny()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "!some(where (p.eft == deny))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        FileAdapter a = new("examples/keymatch_policy.csv");

        Enforcer e = new(m, a);

        TestEnforce(e, "alice", "/alice_data/resource2", "POST", true);
    }

    [Fact]
    public void TestRbacModelInMemoryIndeterminate()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);

        e.AddPermissionForUser("alice", "data1", "invalid");

        TestEnforce(e, "alice", "data1", "read", false);
    }

    [Fact]
    public async Task TestRbacModelInMemoryIndeterminateAsync()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);

        await e.AddPermissionForUserAsync("alice", "data1", "invalid");

        await TestEnforceAsync(e, "alice", "data1", "read", false);
    }

    [Fact]
    public void TestRbacModelInMemory()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);

        e.AddPermissionForUser("alice", "data1", "read");
        e.AddPermissionForUser("bob", "data2", "write");
        e.AddPermissionForUser("data2_admin", "data2", "read");
        e.AddPermissionForUser("data2_admin", "data2", "write");
        e.AddRoleForUser("alice", "data2_admin");

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", true);
        TestEnforce(e, "alice", "data2", "write", true);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
    }

    [Fact]
    public async Task TestRbacModelInMemoryAsync()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);

        await e.AddPermissionForUserAsync("alice", "data1", "read");
        await e.AddPermissionForUserAsync("bob", "data2", "write");
        await e.AddPermissionForUserAsync("data2_admin", "data2", "read");
        await e.AddPermissionForUserAsync("data2_admin", "data2", "write");
        await e.AddRoleForUserAsync("alice", "data2_admin");

        await TestEnforceAsync(e, "alice", "data1", "read", true);
        await TestEnforceAsync(e, "alice", "data1", "write", false);
        await TestEnforceAsync(e, "alice", "data2", "read", true);
        await TestEnforceAsync(e, "alice", "data2", "write", true);
        await TestEnforceAsync(e, "bob", "data1", "read", false);
        await TestEnforceAsync(e, "bob", "data1", "write", false);
        await TestEnforceAsync(e, "bob", "data2", "read", false);
        await TestEnforceAsync(e, "bob", "data2", "write", true);
    }

    [Fact]
    public void TestRbacModelInMemory2()
    {
        string text =
            "[request_definition]\n"
            + "r = sub, obj, act\n"
            + "\n"
            + "[policy_definition]\n"
            + "p = sub, obj, act\n"
            + "\n"
            + "[role_definition]\n"
            + "g = _, _\n"
            + "\n"
            + "[policy_effect]\n"
            + "e = some(where (p.eft == allow))\n"
            + "\n"
            + "[matchers]\n"
            + "m = g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act\n";

        IModel m = DefaultModel.CreateFromText(text);

        Enforcer e = new(m);

        e.AddPermissionForUser("alice", "data1", "read");
        e.AddPermissionForUser("bob", "data2", "write");
        e.AddPermissionForUser("data2_admin", "data2", "read");
        e.AddPermissionForUser("data2_admin", "data2", "write");
        e.AddRoleForUser("alice", "data2_admin");

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", true);
        TestEnforce(e, "alice", "data2", "write", true);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
    }

    [Fact]
    public async Task TestRbacModelInMemory2Async()
    {
        string text =
            "[request_definition]\n"
            + "r = sub, obj, act\n"
            + "\n"
            + "[policy_definition]\n"
            + "p = sub, obj, act\n"
            + "\n"
            + "[role_definition]\n"
            + "g = _, _\n"
            + "\n"
            + "[policy_effect]\n"
            + "e = some(where (p.eft == allow))\n"
            + "\n"
            + "[matchers]\n"
            + "m = g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act\n";

        IModel m = DefaultModel.CreateFromText(text);

        Enforcer e = new(m);

        await e.AddPermissionForUserAsync("alice", "data1", "read");
        await e.AddPermissionForUserAsync("bob", "data2", "write");
        await e.AddPermissionForUserAsync("data2_admin", "data2", "read");
        await e.AddPermissionForUserAsync("data2_admin", "data2", "write");
        await e.AddRoleForUserAsync("alice", "data2_admin");

        await TestEnforceAsync(e, "alice", "data1", "read", true);
        await TestEnforceAsync(e, "alice", "data1", "write", false);
        await TestEnforceAsync(e, "alice", "data2", "read", true);
        await TestEnforceAsync(e, "alice", "data2", "write", true);
        await TestEnforceAsync(e, "bob", "data1", "read", false);
        await TestEnforceAsync(e, "bob", "data1", "write", false);
        await TestEnforceAsync(e, "bob", "data2", "read", false);
        await TestEnforceAsync(e, "bob", "data2", "write", true);
    }

    [Fact]
    public void TestNotUsedRbacModelInMemory()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);

        e.AddPermissionForUser("alice", "data1", "read");
        e.AddPermissionForUser("bob", "data2", "write");

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
    }

    [Fact]
    public async Task TestNotUsedRbacModelInMemoryAsync()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);

        await e.AddPermissionForUserAsync("alice", "data1", "read");
        await e.AddPermissionForUserAsync("bob", "data2", "write");

        await TestEnforceAsync(e, "alice", "data1", "read", true);
        await TestEnforceAsync(e, "alice", "data1", "write", false);
        await TestEnforceAsync(e, "alice", "data2", "read", false);
        await TestEnforceAsync(e, "alice", "data2", "write", false);
        await TestEnforceAsync(e, "bob", "data1", "read", false);
        await TestEnforceAsync(e, "bob", "data1", "write", false);
        await TestEnforceAsync(e, "bob", "data2", "read", false);
        await TestEnforceAsync(e, "bob", "data2", "write", true);
    }

    [Fact]
    public void TestMultipleGroupTypeModelInMemory()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("g", "g2", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && g2(r.obj, p.obj) && r.act == p.act");

        Enforcer e = new(m);
        e.AddPolicy("alice", "data1", "read");
        e.AddPolicy("bob", "data2", "write");
        e.AddPolicy("data_group_admin", "data_group", "write");
        e.AddNamedGroupingPolicy("g", "alice", "data_group_admin");
        e.AddNamedGroupingPolicy("g2", "data1", "data_group");
        e.AddNamedGroupingPolicy("g2", "data2", "data_group");

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.True(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));
    }

    [Fact]
    public async Task TestMultipleGroupTypeModelInMemoryAsync()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("g", "g2", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && g2(r.obj, p.obj) && r.act == p.act");

        Enforcer e = new(m);
        await e.AddPolicyAsync("alice", "data1", "read");
        await e.AddPolicyAsync("bob", "data2", "write");
        await e.AddPolicyAsync("data_group_admin", "data_group", "write");
        await e.AddNamedGroupingPolicyAsync("g", "alice", "data_group_admin");
        await e.AddNamedGroupingPolicyAsync("g2", "data1", "data_group");
        await e.AddNamedGroupingPolicyAsync("g2", "data2", "data_group");

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.True(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.True(await e.EnforceAsync("alice", "data2", "write"));
    }

    #endregion

    #region Init enmpty

    [Fact]
    public void TestInitEmpty()
    {
        Enforcer e = new();

        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        FileAdapter a = new("examples/keymatch_policy.csv");

        e.SetModel(m);
        e.SetAdapter(a);
        e.LoadPolicy();

        TestEnforce(e, "alice", "/alice_data/resource1", "GET", true);
    }

    [Fact]
    public async Task TestInitEmptyAsync()
    {
        Enforcer e = new();

        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        FileAdapter a = new("examples/keymatch_policy.csv");

        e.SetModel(m);
        e.SetAdapter(a);
        await e.LoadPolicyAsync();

        await TestEnforceAsync(e, "alice", "/alice_data/resource1", "GET", true);
    }

    [Fact]
    public void TestInitEmptyByInputStream()
    {
        Enforcer e = new();

        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        using (FileStream fs = new("examples/keymatch_policy.csv", FileMode.Open, FileAccess.Read,
                   FileShare.ReadWrite))
        {
            FileAdapter a = new(fs);
            e.SetModel(m);
            e.SetAdapter(a);
            e.LoadPolicy();

            TestEnforce(e, "alice", "/alice_data/resource1", "GET", true);
        }
    }

    [Fact]
    public async Task TestInitEmptyByInputStreamAsync()
    {
        Enforcer e = new();

        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        using (FileStream fs = new("examples/keymatch_policy.csv", FileMode.Open, FileAccess.Read,
                   FileShare.ReadWrite))
        {
            FileAdapter a = new(fs);
            e.SetModel(m);
            e.SetAdapter(a);
            await e.LoadPolicyAsync();

            await TestEnforceAsync(e, "alice", "/alice_data/resource1", "GET", true);
        }
    }

    #endregion

    #region Store management

    [Fact]
    public void TestReloadPolicy()
    {
        Enforcer e = new("examples/rbac_model.conf", "examples/rbac_policy.csv");

        e.LoadPolicy();
        TestGetPolicy(e,
            AsList(AsList("alice", "data1", "read"), AsList("bob", "data2", "write"),
                AsList("data2_admin", "data2", "read"), AsList("data2_admin", "data2", "write")));
    }

    [Fact]
    public async Task TestReloadPolicyAsync()
    {
        Enforcer e = new("examples/rbac_model.conf", "examples/rbac_policy.csv");

        await e.LoadPolicyAsync();
        TestGetPolicy(e,
            AsList(AsList("alice", "data1", "read"), AsList("bob", "data2", "write"),
                AsList("data2_admin", "data2", "read"), AsList("data2_admin", "data2", "write")));
    }

    [Fact]
    public void TestSavePolicy()
    {
        Enforcer e = new("examples/rbac_model.conf", "examples/rbac_policy.csv");

        e.SavePolicy();
    }

    [Fact]
    public async Task TestSavePolicyAsync()
    {
        Enforcer e = new("examples/rbac_model.conf", "examples/rbac_policy.csv");

        await e.SavePolicyAsync();
    }

    [Fact]
    public void TestSavePolicyWithoutBasicModel()
    {
        Enforcer e = new("examples/basic_model.conf", "examples/basic_policy.csv");

        e.SavePolicy();
    }

    [Fact]
    public async Task TestSavePolicyWithoutBasicModelAsync()
    {
        Enforcer e = new("examples/basic_model.conf", "examples/basic_policy.csv");

        await e.SavePolicyAsync();
    }

    [Fact]
    public void TestClearPolicy()
    {
        Enforcer e = new("examples/rbac_model.conf", "examples/rbac_policy.csv");

        e.ClearPolicy();
    }

    #endregion

    #region Extension features

    [Fact]
    public void TestEnableEnforce()
    {
        Enforcer e = new("examples/basic_model.conf", "examples/basic_policy.csv");

        e.EnableEnforce(false);
        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", true);
        TestEnforce(e, "alice", "data2", "read", true);
        TestEnforce(e, "alice", "data2", "write", true);
        TestEnforce(e, "bob", "data1", "read", true);
        TestEnforce(e, "bob", "data1", "write", true);
        TestEnforce(e, "bob", "data2", "read", true);
        TestEnforce(e, "bob", "data2", "write", true);

        e.EnableEnforce(true);
        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
    }

#if !NET452
    [Fact]
    public void TestEnableLog()
    {
        Enforcer e = new("examples/basic_model.conf", "examples/basic_policy.csv")
        {
            Logger = new MockLogger<Enforcer>(_testOutputHelper)
        };

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);

        e.Logger = null;
        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
    }
#endif

    [Fact]
    public void TestEnableAutoSave()
    {
        Enforcer e = new("examples/basic_model.conf", "examples/basic_policy.csv");

        e.EnableAutoSave(false);
        // Because AutoSave is disabled, the policy change only affects the policy in Casbin enforcer,
        // it doesn't affect the policy in the storage.
        e.RemovePolicy("alice", "data1", "read");
        // Reload the policy from the storage to see the effect.
        e.LoadPolicy();
        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);

        e.EnableAutoSave(true);
        // Because AutoSave is enabled, the policy change not only affects the policy in Casbin enforcer,
        // but also affects the policy in the storage.
        e.RemovePolicy("alice", "data1", "read");

        // However, the file adapter doesn't implement the AutoSave feature, so enabling it has no effect at all here.

        // Reload the policy from the storage to see the effect.
        e.LoadPolicy();
        TestEnforce(e, "alice", "data1", "read", true); // Will not be false here.
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
    }

    [Fact]
    public async Task TestEnableAutoSaveAsync()
    {
        Enforcer e = new("examples/basic_model.conf", "examples/basic_policy_for_async_adapter_test.csv");

        e.EnableAutoSave(false);
        // Because AutoSave is disabled, the policy change only affects the policy in Casbin enforcer,
        // it doesn't affect the policy in the storage.
        await e.RemovePolicyAsync("alice", "data1", "read");
        // Reload the policy from the storage to see the effect.
        await e.LoadPolicyAsync();
        await TestEnforceAsync(e, "alice", "data1", "read", true);
        await TestEnforceAsync(e, "alice", "data1", "write", false);
        await TestEnforceAsync(e, "alice", "data2", "read", false);
        await TestEnforceAsync(e, "alice", "data2", "write", false);
        await TestEnforceAsync(e, "bob", "data1", "read", false);
        await TestEnforceAsync(e, "bob", "data1", "write", false);
        await TestEnforceAsync(e, "bob", "data2", "read", false);
        await TestEnforceAsync(e, "bob", "data2", "write", true);

        e.EnableAutoSave(true);
        // Because AutoSave is enabled, the policy change not only affects the policy in Casbin enforcer,
        // but also affects the policy in the storage.
        await e.RemovePolicyAsync("alice", "data1", "read");

        // However, the file adapter doesn't implement the AutoSave feature, so enabling it has no effect at all here.

        // Reload the policy from the storage to see the effect.
        await e.LoadPolicyAsync();
        await TestEnforceAsync(e, "alice", "data1", "read", true); // Will not be false here.
        await TestEnforceAsync(e, "alice", "data1", "write", false);
        await TestEnforceAsync(e, "alice", "data2", "read", false);
        await TestEnforceAsync(e, "alice", "data2", "write", false);
        await TestEnforceAsync(e, "bob", "data1", "read", false);
        await TestEnforceAsync(e, "bob", "data1", "write", false);
        await TestEnforceAsync(e, "bob", "data2", "read", false);
        await TestEnforceAsync(e, "bob", "data2", "write", true);
    }

    [Fact]
    public void TestInitWithAdapter()
    {
        FileAdapter adapter = new("examples/basic_policy.csv");
        Enforcer e = new("examples/basic_model.conf", adapter);

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);
        TestEnforce(e, "alice", "data2", "read", false);
        TestEnforce(e, "alice", "data2", "write", false);
        TestEnforce(e, "bob", "data1", "read", false);
        TestEnforce(e, "bob", "data1", "write", false);
        TestEnforce(e, "bob", "data2", "read", false);
        TestEnforce(e, "bob", "data2", "write", true);
    }

    [Fact]
    public void TestRoleLinks()
    {
        Enforcer e = new("examples/rbac_model.conf");
        e.EnableAutoBuildRoleLinks(false);
        e.BuildRoleLinks();
        e.Enforce("user501", "data9", "read");
    }

    [Fact]
    public void TestGetAndSetModel()
    {
        Enforcer e = new("examples/basic_model.conf", "examples/basic_policy.csv");
        Enforcer e2 = new("examples/basic_with_root_model.conf", "examples/basic_policy.csv");

        TestEnforce(e, "root", "data1", "read", false);

        e.SetModel(e2.Model);

        TestEnforce(e, "root", "data1", "read", true);
    }

    [Fact]
    public void TestGetAndSetAdapterInMem()
    {
        Enforcer e = new("examples/basic_model.conf", "examples/basic_policy.csv");
        Enforcer e2 = new("examples/basic_model.conf", "examples/basic_inverse_policy.csv");

        TestEnforce(e, "alice", "data1", "read", true);
        TestEnforce(e, "alice", "data1", "write", false);

        IReadOnlyAdapter a2 = e2.Adapter;
        e.SetAdapter(a2);
        e.LoadPolicy();

        TestEnforce(e, "alice", "data1", "read", false);
        TestEnforce(e, "alice", "data1", "write", true);
    }

    [Fact]
    public async Task TestGetAndSetAdapterInMemAsync()
    {
        Enforcer e = new("examples/basic_model.conf", "examples/basic_policy_for_async_adapter_test.csv");
        Enforcer e2 = new("examples/basic_model.conf", "examples/basic_inverse_policy.csv");

        await TestEnforceAsync(e, "alice", "data1", "read", true);
        await TestEnforceAsync(e, "alice", "data1", "write", false);

        IReadOnlyAdapter a2 = e2.Adapter;
        e.SetAdapter(a2);
        await e.LoadPolicyAsync();

        await TestEnforceAsync(e, "alice", "data1", "read", false);
        await TestEnforceAsync(e, "alice", "data1", "write", true);
    }

    [Fact]
    public void TestSetAdapterFromFile()
    {
        Enforcer e = new("examples/basic_model.conf");

        TestEnforce(e, "alice", "data1", "read", false);

        FileAdapter a = new("examples/basic_policy.csv");
        e.SetAdapter(a);
        e.LoadPolicy();

        TestEnforce(e, "alice", "data1", "read", true);
    }

    [Fact]
    public async Task TestSetAdapterFromFileAsync()
    {
        Enforcer e = new("examples/basic_model.conf");

        await TestEnforceAsync(e, "alice", "data1", "read", false);

        FileAdapter a = new("examples/basic_policy_for_async_adapter_test.csv");
        e.SetAdapter(a);
        await e.LoadPolicyAsync();

        await TestEnforceAsync(e, "alice", "data1", "read", true);
    }

    [Fact]
    public void TestEnforceExApi()
    {
        Enforcer e = new(_testModelFixture.GetBasicTestModel());

        TestEnforceEx(e, "alice", "data1", "read", new List<string> { "alice", "data1", "read" });
        TestEnforceEx(e, "alice", "data1", "write", new List<string>());
        TestEnforceEx(e, "alice", "data2", "read", new List<string>());
        TestEnforceEx(e, "alice", "data2", "write", new List<string>());
        TestEnforceEx(e, "bob", "data1", "read", new List<string>());
        TestEnforceEx(e, "bob", "data1", "write", new List<string>());
        TestEnforceEx(e, "bob", "data2", "read", new List<string>());
        TestEnforceEx(e, "bob", "data2", "write", new List<string> { "bob", "data2", "write" });

        e = new Enforcer(_testModelFixture.GetNewRbacTestModel());

        TestEnforceEx(e, "alice", "data1", "read", new List<string> { "alice", "data1", "read" });
        TestEnforceEx(e, "alice", "data1", "write", new List<string>());
        TestEnforceEx(e, "alice", "data2", "read", new List<string> { "data2_admin", "data2", "read" });
        TestEnforceEx(e, "alice", "data2", "write", new List<string> { "data2_admin", "data2", "write" });
        TestEnforceEx(e, "bob", "data1", "read", new List<string>());
        TestEnforceEx(e, "bob", "data1", "write", new List<string>());
        TestEnforceEx(e, "bob", "data2", "read", new List<string>());
        TestEnforceEx(e, "bob", "data2", "write", new List<string> { "bob", "data2", "write" });

        e = new Enforcer(_testModelFixture.GetNewPriorityTestModel());
        e.BuildRoleLinks();

        TestEnforceEx(e, "alice", "data1", "read", new List<string> { "alice", "data1", "read", "allow" });
        TestEnforceEx(e, "alice", "data1", "write",
            new List<string> { "data1_deny_group", "data1", "write", "deny" });
        TestEnforceEx(e, "alice", "data2", "read", new List<string>());
        TestEnforceEx(e, "alice", "data2", "write", new List<string>());
        TestEnforceEx(e, "bob", "data1", "write", new List<string>());
        TestEnforceEx(e, "bob", "data2", "read",
            new List<string> { "data2_allow_group", "data2", "read", "allow" });
        TestEnforceEx(e, "bob", "data2", "write", new List<string> { "bob", "data2", "write", "deny" });
    }

    [Fact]
    public async Task TestEnforceExApiAsync()
    {
        Enforcer e = new(_testModelFixture.GetBasicTestModel());

        await TestEnforceExAsync(e, "alice", "data1", "read", new List<string> { "alice", "data1", "read" });
        await TestEnforceExAsync(e, "alice", "data1", "write", new List<string>());
        await TestEnforceExAsync(e, "alice", "data2", "read", new List<string>());
        await TestEnforceExAsync(e, "alice", "data2", "write", new List<string>());
        await TestEnforceExAsync(e, "bob", "data1", "read", new List<string>());
        await TestEnforceExAsync(e, "bob", "data1", "write", new List<string>());
        await TestEnforceExAsync(e, "bob", "data2", "read", new List<string>());
        await TestEnforceExAsync(e, "bob", "data2", "write", new List<string> { "bob", "data2", "write" });

        e = new Enforcer(_testModelFixture.GetNewRbacTestModel());

        await TestEnforceExAsync(e, "alice", "data1", "read", new List<string> { "alice", "data1", "read" });
        await TestEnforceExAsync(e, "alice", "data1", "write", new List<string>());
        await TestEnforceExAsync(e, "alice", "data2", "read", new List<string> { "data2_admin", "data2", "read" });
        await TestEnforceExAsync(e, "alice", "data2", "write", new List<string> { "data2_admin", "data2", "write" });
        await TestEnforceExAsync(e, "bob", "data1", "read", new List<string>());
        await TestEnforceExAsync(e, "bob", "data1", "write", new List<string>());
        await TestEnforceExAsync(e, "bob", "data2", "read", new List<string>());
        await TestEnforceExAsync(e, "bob", "data2", "write", new List<string> { "bob", "data2", "write" });

        e = new Enforcer(_testModelFixture.GetNewPriorityTestModel());
        e.BuildRoleLinks();

        await TestEnforceExAsync(e, "alice", "data1", "read", new List<string> { "alice", "data1", "read", "allow" });
        await TestEnforceExAsync(e, "alice", "data1", "write",
            new List<string> { "data1_deny_group", "data1", "write", "deny" });
        await TestEnforceExAsync(e, "alice", "data2", "read", new List<string>());
        await TestEnforceExAsync(e, "alice", "data2", "write", new List<string>());
        await TestEnforceExAsync(e, "bob", "data1", "write", new List<string>());
        await TestEnforceExAsync(e, "bob", "data2", "read",
            new List<string> { "data2_allow_group", "data2", "read", "allow" });
        await TestEnforceExAsync(e, "bob", "data2", "write", new List<string> { "bob", "data2", "write", "deny" });
    }

#if !NET452
    [Fact]
    public void TestEnforceExApiLog()
    {
        Enforcer e = new(_testModelFixture.GetBasicTestModel())
        {
            Logger = new MockLogger<Enforcer>(_testOutputHelper)
        };

        TestEnforceEx(e, "alice", "data1", "read", new List<string> { "alice", "data1", "read" });
        TestEnforceEx(e, "alice", "data1", "write", new List<string>());
        TestEnforceEx(e, "alice", "data2", "read", new List<string>());
        TestEnforceEx(e, "alice", "data2", "write", new List<string>());
        TestEnforceEx(e, "bob", "data1", "read", new List<string>());
        TestEnforceEx(e, "bob", "data1", "write", new List<string>());
        TestEnforceEx(e, "bob", "data2", "read", new List<string>());
        TestEnforceEx(e, "bob", "data2", "write", new List<string> { "bob", "data2", "write" });

        e.Logger = null;
    }
#endif

    #endregion

    #region EnforceWithMatcher and EnforceExWithMatcher API

    [Fact]
    public void TestEnforceWithMatcherApi()
    {
        Enforcer e = new(_testModelFixture.GetBasicTestModel());
        string matcher = "r.sub != p.sub && r.obj == p.obj && r.act == p.act";

        e.TestEnforceWithMatcher(matcher, "alice", "data1", "read", false);
        e.TestEnforceWithMatcher(matcher, "alice", "data1", "write", false);
        e.TestEnforceWithMatcher(matcher, "alice", "data2", "read", false);
        e.TestEnforceWithMatcher(matcher, "alice", "data2", "write", true);
        e.TestEnforceWithMatcher(matcher, "bob", "data1", "read", true);
        e.TestEnforceWithMatcher(matcher, "bob", "data1", "write", false);
        e.TestEnforceWithMatcher(matcher, "bob", "data2", "read", false);
        e.TestEnforceWithMatcher(matcher, "bob", "data2", "write", false);
    }

    [Fact]
    public async Task TestEnforceWithMatcherAsync()
    {
        Enforcer e = new(_testModelFixture.GetBasicTestModel());
        string matcher = "r.sub != p.sub && r.obj == p.obj && r.act == p.act";

        await e.TestEnforceWithMatcherAsync(matcher, "alice", "data1", "read", false);
        await e.TestEnforceWithMatcherAsync(matcher, "alice", "data1", "write", false);
        await e.TestEnforceWithMatcherAsync(matcher, "alice", "data2", "read", false);
        await e.TestEnforceWithMatcherAsync(matcher, "alice", "data2", "write", true);
        await e.TestEnforceWithMatcherAsync(matcher, "bob", "data1", "read", true);
        await e.TestEnforceWithMatcherAsync(matcher, "bob", "data1", "write", false);
        await e.TestEnforceWithMatcherAsync(matcher, "bob", "data2", "read", false);
        await e.TestEnforceWithMatcherAsync(matcher, "bob", "data2", "write", false);
    }

    [Fact]
    public void TestEnforceExWithMatcherApi()
    {
        Enforcer e = new(_testModelFixture.GetBasicTestModel());
        string matcher = "r.sub != p.sub && r.obj == p.obj && r.act == p.act";

        e.TestEnforceExWithMatcher(matcher, "alice", "data1", "read", new List<string>());
        e.TestEnforceExWithMatcher(matcher, "alice", "data1", "write", new List<string>());
        e.TestEnforceExWithMatcher(matcher, "alice", "data2", "read", new List<string>());
        e.TestEnforceExWithMatcher(matcher, "alice", "data2", "write", new List<string> { "bob", "data2", "write" });
        e.TestEnforceExWithMatcher(matcher, "bob", "data1", "read", new List<string> { "alice", "data1", "read" });
        e.TestEnforceExWithMatcher(matcher, "bob", "data1", "write", new List<string>());
        e.TestEnforceExWithMatcher(matcher, "bob", "data2", "read", new List<string>());
        e.TestEnforceExWithMatcher(matcher, "bob", "data2", "write", new List<string>());
    }

    [Fact]
    public async Task TestEnforceExWithMatcherAsync()
    {
        Enforcer e = new(_testModelFixture.GetBasicTestModel());
        string matcher = "r.sub != p.sub && r.obj == p.obj && r.act == p.act";

        await e.TestEnforceExWithMatcherAsync(matcher, "alice", "data1", "read", new List<string>());
        await e.TestEnforceExWithMatcherAsync(matcher, "alice", "data1", "write", new List<string>());
        await e.TestEnforceExWithMatcherAsync(matcher, "alice", "data2", "read", new List<string>());
        await e.TestEnforceExWithMatcherAsync(matcher, "alice", "data2", "write",
            new List<string> { "bob", "data2", "write" });
        await e.TestEnforceExWithMatcherAsync(matcher, "bob", "data1", "read",
            new List<string> { "alice", "data1", "read" });
        await e.TestEnforceExWithMatcherAsync(matcher, "bob", "data1", "write", new List<string>());
        await e.TestEnforceExWithMatcherAsync(matcher, "bob", "data2", "read", new List<string>());
        await e.TestEnforceExWithMatcherAsync(matcher, "bob", "data2", "write", new List<string>());
    }

    #endregion
}
