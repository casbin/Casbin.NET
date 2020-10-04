using System.IO;
using System.Threading.Tasks;
using NetCasbin.Persist;
using NetCasbin.Persist.FileAdapter;
using Xunit;
using static NetCasbin.UnitTest.Util.TestUtil;

namespace NetCasbin.UnitTest
{
    public class EnforcerUnitTest
    {
        [Fact]
        public void TestKeyMatchModelInMemory()
        {
            var m = Model.Model.CreateDefault();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

            IAdapter a = new DefaultFileAdapter("examples/keymatch_policy.csv");

            var e = new Enforcer(m, a);

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
            a.LoadPolicy(e.GetModel());

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
            var m = Model.Model.CreateDefault();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

            IAdapter a = new DefaultFileAdapter("examples/keymatch_policy.csv");

            var e = new Enforcer(m, a);

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
            await a.LoadPolicyAsync(e.GetModel());

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
            var m = Model.Model.CreateDefault();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("e", "e", "!some(where (p.eft == deny))");
            m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

            IAdapter a = new DefaultFileAdapter("examples/keymatch_policy.csv");

            var e = new Enforcer(m, a);

            TestEnforce(e, "alice", "/alice_data/resource2", "POST", true);
        }

        [Fact]
        public void TestRbacModelInMemoryIndeterminate()
        {
            var m = Model.Model.CreateDefault();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("g", "g", "_, _");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

            var e = new Enforcer(m);

            e.AddPermissionForUser("alice", "data1", "invalid");

            TestEnforce(e, "alice", "data1", "read", false);
        }

        [Fact]
        public async Task TestRbacModelInMemoryIndeterminateAsync()
        {
            var m = Model.Model.CreateDefault();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("g", "g", "_, _");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

            var e = new Enforcer(m);

            await e.AddPermissionForUserAsync("alice", "data1", "invalid");

            await TestEnforceAsync(e, "alice", "data1", "read", false);
        }

        [Fact]
        public void TestRbacModelInMemory()
        {
            var m = Model.Model.CreateDefault();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("g", "g", "_, _");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

            var e = new Enforcer(m);

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
            var m = Model.Model.CreateDefault();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("g", "g", "_, _");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

            var e = new Enforcer(m);

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

            var m = Model.Model.CreateDefaultFromText(text);

            var e = new Enforcer(m);

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

            var m = Model.Model.CreateDefaultFromText(text);

            var e = new Enforcer(m);

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
            var m = Model.Model.CreateDefault();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("g", "g", "_, _");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

            var e = new Enforcer(m);

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
            var m = Model.Model.CreateDefault();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("g", "g", "_, _");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

            var e = new Enforcer(m);

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
            var e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            e.LoadPolicy();
            TestGetPolicy(e, AsList(AsList("alice", "data1", "read"), AsList("bob", "data2", "write"), AsList("data2_admin", "data2", "read"), AsList("data2_admin", "data2", "write")));
        }

        [Fact]
        public async Task TestReloadPolicyAsync()
        {
            var e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            await e.LoadPolicyAsync();
            TestGetPolicy(e, AsList(AsList("alice", "data1", "read"), AsList("bob", "data2", "write"), AsList("data2_admin", "data2", "read"), AsList("data2_admin", "data2", "write")));
        }

        [Fact]
        public void TestSavePolicy()
        {
            var e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            e.SavePolicy();
        }

        [Fact]
        public async Task TestSavePolicyAsync()
        {
            var e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            await e.SavePolicyAsync();
        }

        [Fact]
        public void TestSavePolicyWithoutBasicModel()
        {
            var e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");

            e.SavePolicy();
        }

        [Fact]
        public async Task TestSavePolicyWithoutBasicModelAsync()
        {
            var e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");

            await e.SavePolicyAsync();
        }

        [Fact]
        public void TestClearPolicy()
        {
            var e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            e.ClearPolicy();
        }

        [Fact]
        public void TestEnableEnforce()
        {
            var e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");

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

        [Fact]
        public void TestEnableLog()
        {
            var e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv", true);
            // The log is enabled by default, so the above is the same with:
            // Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");

            TestEnforce(e, "alice", "data1", "read", true);
            TestEnforce(e, "alice", "data1", "write", false);
            TestEnforce(e, "alice", "data2", "read", false);
            TestEnforce(e, "alice", "data2", "write", false);
            TestEnforce(e, "bob", "data1", "read", false);
            TestEnforce(e, "bob", "data1", "write", false);
            TestEnforce(e, "bob", "data2", "read", false);
            TestEnforce(e, "bob", "data2", "write", true);

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
        public void TestEnableAutoSave()
        {
            var e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");

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
            var e = new Enforcer("examples/basic_model.conf", "examples/basic_policy_for_async_adapter_test.csv");

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
            IAdapter adapter = new DefaultFileAdapter("examples/basic_policy.csv");
            var e = new Enforcer("examples/basic_model.conf", adapter);

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
            var e = new Enforcer("examples/rbac_model.conf");
            e.EnableAutoBuildRoleLinks(false);
            e.BuildRoleLinks();
            e.Enforce("user501", "data9", "read");
        }

        [Fact]
        public void TestGetAndSetModel()
        {
            var e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");
            var e2 = new Enforcer("examples/basic_with_root_model.conf", "examples/basic_policy.csv");

            TestEnforce(e, "root", "data1", "read", false);

            e.SetModel(e2.GetModel());

            TestEnforce(e, "root", "data1", "read", true);
        }

        [Fact]
        public void TestGetAndSetAdapterInMem()
        {
            var e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");
            var e2 = new Enforcer("examples/basic_model.conf", "examples/basic_inverse_policy.csv");

            TestEnforce(e, "alice", "data1", "read", true);
            TestEnforce(e, "alice", "data1", "write", false);

            var a2 = e2.GetAdapter();
            e.SetAdapter(a2);
            e.LoadPolicy();

            TestEnforce(e, "alice", "data1", "read", false);
            TestEnforce(e, "alice", "data1", "write", true);
        }

        [Fact]
        public async Task TestGetAndSetAdapterInMemAsync()
        {
            var e = new Enforcer("examples/basic_model.conf", "examples/basic_policy_for_async_adapter_test.csv");
            var e2 = new Enforcer("examples/basic_model.conf", "examples/basic_inverse_policy.csv");

            await TestEnforceAsync(e, "alice", "data1", "read", true);
            await TestEnforceAsync(e, "alice", "data1", "write", false);

            var a2 = e2.GetAdapter();
            e.SetAdapter(a2);
            await e.LoadPolicyAsync();

            await TestEnforceAsync(e, "alice", "data1", "read", false);
            await TestEnforceAsync(e, "alice", "data1", "write", true);
        }

        [Fact]
        public void TestSetAdapterFromFile()
        {
            var e = new Enforcer("examples/basic_model.conf");

            TestEnforce(e, "alice", "data1", "read", false);

            IAdapter a = new DefaultFileAdapter("examples/basic_policy.csv");
            e.SetAdapter(a);
            e.LoadPolicy();

            TestEnforce(e, "alice", "data1", "read", true);
        }

        [Fact]
        public async Task TestSetAdapterFromFileAsync()
        {
            var e = new Enforcer("examples/basic_model.conf");

            await TestEnforceAsync(e, "alice", "data1", "read", false);

            IAdapter a = new DefaultFileAdapter("examples/basic_policy_for_async_adapter_test.csv");
            e.SetAdapter(a);
            await e.LoadPolicyAsync();

            await TestEnforceAsync(e, "alice", "data1", "read", true);
        }

        [Fact]
        public void TestInitEmpty()
        {
            var e = new Enforcer();

            var m = Model.Model.CreateDefault();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

            IAdapter a = new DefaultFileAdapter("examples/keymatch_policy.csv");

            e.SetModel(m);
            e.SetAdapter(a);
            e.LoadPolicy();

            TestEnforce(e, "alice", "/alice_data/resource1", "GET", true);
        }

        [Fact]
        public async Task TestInitEmptyAsync()
        {
            var e = new Enforcer();

            var m = Model.Model.CreateDefault();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

            IAdapter a = new DefaultFileAdapter("examples/keymatch_policy.csv");

            e.SetModel(m);
            e.SetAdapter(a);
            await e.LoadPolicyAsync();

            await TestEnforceAsync(e, "alice", "/alice_data/resource1", "GET", true);
        }

        [Fact]
        public void TestInitEmptyByInputStream()
        {
            var e = new Enforcer();

            var m = Model.Model.CreateDefault();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

            using (var fs = new FileStream("examples/keymatch_policy.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IAdapter a = new DefaultFileAdapter(fs);
                e.SetModel(m);
                e.SetAdapter(a);
                e.LoadPolicy();

                TestEnforce(e, "alice", "/alice_data/resource1", "GET", true);
            }
        }

        [Fact]
        public async Task TestInitEmptyByInputStreamAsync()
        {
            var e = new Enforcer();

            var m = Model.Model.CreateDefault();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

            using (var fs = new FileStream("examples/keymatch_policy.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IAdapter a = new DefaultFileAdapter(fs);
                e.SetModel(m);
                e.SetAdapter(a);
                await e.LoadPolicyAsync();

                await TestEnforceAsync(e, "alice", "/alice_data/resource1", "GET", true);
            }
        }
    }
}
