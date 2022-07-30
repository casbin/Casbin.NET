using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Casbin.Adapter.File;
using Casbin.Model;
using Casbin.Persist;
using Casbin.UnitTests.Fixtures;
using Casbin.UnitTests.Mock;
using Xunit;
using Xunit.Abstractions;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.SyncedModelTests;

[Collection("Model collection")]
public class SyncedEnforcerTest
{
    private readonly TestModelFixture _testModelFixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public SyncedEnforcerTest(ITestOutputHelper testOutputHelper, TestModelFixture testModelFixture)
    {
        _testOutputHelper = testOutputHelper;
        _testModelFixture = testModelFixture;
    }

    [Fact]
    public void TestKeyMatchModelInMemory()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        FileAdapter a = new("examples/keymatch_policy.csv");

        IEnforcer e = SyncedEnforcer.Create(m, a);

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

        e = SyncedEnforcer.Create(m);
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

        IEnforcer e = SyncedEnforcer.Create(m, a);

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

        e = SyncedEnforcer.Create(m);
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

        IEnforcer e = SyncedEnforcer.Create(m, a);

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

        IEnforcer e = SyncedEnforcer.Create(m);

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

        IEnforcer e = SyncedEnforcer.Create(m);

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

        IEnforcer e = SyncedEnforcer.Create(m);

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

        IEnforcer e = SyncedEnforcer.Create(m);

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

        IEnforcer e = SyncedEnforcer.Create(m);

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

        IEnforcer e = SyncedEnforcer.Create(m);

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

        IEnforcer e = SyncedEnforcer.Create(m);

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

        IEnforcer e = SyncedEnforcer.Create(m);

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
    public void TestReloadPolicy()
    {
        IEnforcer e = SyncedEnforcer.Create("examples/rbac_model.conf", "examples/rbac_policy.csv");

        e.LoadPolicy();
        TestGetPolicy(e,
            AsList(AsList("alice", "data1", "read"), AsList("bob", "data2", "write"),
                AsList("data2_admin", "data2", "read"), AsList("data2_admin", "data2", "write")));
    }

    [Fact]
    public async Task TestReloadPolicyAsync()
    {
        IEnforcer e = SyncedEnforcer.Create("examples/rbac_model.conf", "examples/rbac_policy.csv");

        await e.LoadPolicyAsync();
        TestGetPolicy(e,
            AsList(AsList("alice", "data1", "read"), AsList("bob", "data2", "write"),
                AsList("data2_admin", "data2", "read"), AsList("data2_admin", "data2", "write")));
    }

    [Fact]
    public void TestSavePolicy()
    {
        IEnforcer e = SyncedEnforcer.Create("examples/rbac_model.conf", "examples/rbac_policy.csv");

        e.SavePolicy();
    }

    [Fact]
    public async Task TestSavePolicyAsync()
    {
        IEnforcer e = SyncedEnforcer.Create("examples/rbac_model.conf", "examples/rbac_policy.csv");

        await e.SavePolicyAsync();
    }

    [Fact]
    public void TestSavePolicyWithoutBasicModel()
    {
        IEnforcer e = SyncedEnforcer.Create("examples/basic_model.conf", "examples/basic_policy.csv");

        e.SavePolicy();
    }

    [Fact]
    public async Task TestSavePolicyWithoutBasicModelAsync()
    {
        IEnforcer e = SyncedEnforcer.Create("examples/basic_model.conf", "examples/basic_policy.csv");

        await e.SavePolicyAsync();
    }

    [Fact]
    public void TestClearPolicy()
    {
        IEnforcer e = SyncedEnforcer.Create("examples/rbac_model.conf", "examples/rbac_policy.csv");

        e.ClearPolicy();
    }

    [Fact]
    public void TestEnableEnforce()
    {
        IEnforcer e = SyncedEnforcer.Create("examples/basic_model.conf", "examples/basic_policy.csv");

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
        IEnforcer e = SyncedEnforcer.Create("examples/basic_model.conf", "examples/basic_policy.csv");
        e.Logger = new MockLogger<Enforcer>(_testOutputHelper);

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
        IEnforcer e = SyncedEnforcer.Create("examples/basic_model.conf", "examples/basic_policy.csv");

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
        IEnforcer e = SyncedEnforcer.Create("examples/basic_model.conf",
            "examples/basic_policy_for_async_adapter_test.csv");

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
        IEnforcer e = SyncedEnforcer.Create("examples/basic_model.conf", adapter);

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
        IEnforcer e = SyncedEnforcer.Create("examples/rbac_model.conf");
        e.EnableAutoBuildRoleLinks(false);
        e.BuildRoleLinks();
        e.Enforce("user501", "data9", "read");
    }

    [Fact]
    public void TestGetAndSetModel()
    {
        IEnforcer e = SyncedEnforcer.Create("examples/basic_model.conf", "examples/basic_policy.csv");
        IEnforcer e2 = SyncedEnforcer.Create("examples/basic_with_root_model.conf", "examples/basic_policy.csv");

        TestEnforce(e, "root", "data1", "read", false);

        e.SetModel(e2.Model);

        TestEnforce(e, "root", "data1", "read", true);
    }

    [Fact]
    public void TestGetAndSetAdapterInMem()
    {
        IEnforcer e = SyncedEnforcer.Create("examples/basic_model.conf", "examples/basic_policy.csv");
        IEnforcer e2 = SyncedEnforcer.Create("examples/basic_model.conf", "examples/basic_inverse_policy.csv");

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
        IEnforcer e = SyncedEnforcer.Create("examples/basic_model.conf",
            "examples/basic_policy_for_async_adapter_test.csv");
        IEnforcer e2 = SyncedEnforcer.Create("examples/basic_model.conf", "examples/basic_inverse_policy.csv");

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
        IEnforcer e = SyncedEnforcer.Create("examples/basic_model.conf");

        TestEnforce(e, "alice", "data1", "read", false);

        FileAdapter a = new("examples/basic_policy.csv");
        e.SetAdapter(a);
        e.LoadPolicy();

        TestEnforce(e, "alice", "data1", "read", true);
    }

    [Fact]
    public async Task TestSetAdapterFromFileAsync()
    {
        IEnforcer e = SyncedEnforcer.Create("examples/basic_model.conf");

        await TestEnforceAsync(e, "alice", "data1", "read", false);

        FileAdapter a = new("examples/basic_policy_for_async_adapter_test.csv");
        e.SetAdapter(a);
        await e.LoadPolicyAsync();

        await TestEnforceAsync(e, "alice", "data1", "read", true);
    }

    [Fact]
    public void TestInitEmpty()
    {
        IEnforcer e = DefaultEnforcer.Create();

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
        IEnforcer e = SyncedEnforcer.Create();

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
        IEnforcer e = SyncedEnforcer.Create();

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
        IEnforcer e = SyncedEnforcer.Create();

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

    [Fact]
    public void TestEnforceExApi()
    {
        IEnforcer e = SyncedEnforcer.Create(_testModelFixture.GetBasicTestModel());

        TestEnforceEx(e, "alice", "data1", "read", new List<string> { "alice", "data1", "read" });
        TestEnforceEx(e, "alice", "data1", "write", new List<string>());
        TestEnforceEx(e, "alice", "data2", "read", new List<string>());
        TestEnforceEx(e, "alice", "data2", "write", new List<string>());
        TestEnforceEx(e, "bob", "data1", "read", new List<string>());
        TestEnforceEx(e, "bob", "data1", "write", new List<string>());
        TestEnforceEx(e, "bob", "data2", "read", new List<string>());
        TestEnforceEx(e, "bob", "data2", "write", new List<string> { "bob", "data2", "write" });

        e = SyncedEnforcer.Create(_testModelFixture.GetNewRbacTestModel());

        TestEnforceEx(e, "alice", "data1", "read", new List<string> { "alice", "data1", "read" });
        TestEnforceEx(e, "alice", "data1", "write", new List<string>());
        TestEnforceEx(e, "alice", "data2", "read", new List<string> { "data2_admin", "data2", "read" });
        TestEnforceEx(e, "alice", "data2", "write", new List<string> { "data2_admin", "data2", "write" });
        TestEnforceEx(e, "bob", "data1", "read", new List<string>());
        TestEnforceEx(e, "bob", "data1", "write", new List<string>());
        TestEnforceEx(e, "bob", "data2", "read", new List<string>());
        TestEnforceEx(e, "bob", "data2", "write", new List<string> { "bob", "data2", "write" });

        e = SyncedEnforcer.Create(_testModelFixture.GetNewPriorityTestModel());
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
        IEnforcer e = SyncedEnforcer.Create(_testModelFixture.GetBasicTestModel());

        await TestEnforceExAsync(e, "alice", "data1", "read", new List<string> { "alice", "data1", "read" });
        await TestEnforceExAsync(e, "alice", "data1", "write", new List<string>());
        await TestEnforceExAsync(e, "alice", "data2", "read", new List<string>());
        await TestEnforceExAsync(e, "alice", "data2", "write", new List<string>());
        await TestEnforceExAsync(e, "bob", "data1", "read", new List<string>());
        await TestEnforceExAsync(e, "bob", "data1", "write", new List<string>());
        await TestEnforceExAsync(e, "bob", "data2", "read", new List<string>());
        await TestEnforceExAsync(e, "bob", "data2", "write", new List<string> { "bob", "data2", "write" });

        e = SyncedEnforcer.Create(_testModelFixture.GetNewRbacTestModel());

        await TestEnforceExAsync(e, "alice", "data1", "read", new List<string> { "alice", "data1", "read" });
        await TestEnforceExAsync(e, "alice", "data1", "write", new List<string>());
        await TestEnforceExAsync(e, "alice", "data2", "read", new List<string> { "data2_admin", "data2", "read" });
        await TestEnforceExAsync(e, "alice", "data2", "write", new List<string> { "data2_admin", "data2", "write" });
        await TestEnforceExAsync(e, "bob", "data1", "read", new List<string>());
        await TestEnforceExAsync(e, "bob", "data1", "write", new List<string>());
        await TestEnforceExAsync(e, "bob", "data2", "read", new List<string>());
        await TestEnforceExAsync(e, "bob", "data2", "write", new List<string> { "bob", "data2", "write" });

        e = SyncedEnforcer.Create(_testModelFixture.GetNewPriorityTestModel());
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
        IEnforcer e = SyncedEnforcer.Create(_testModelFixture.GetBasicTestModel());
        e.Logger = new MockLogger<Enforcer>(_testOutputHelper);

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
}
