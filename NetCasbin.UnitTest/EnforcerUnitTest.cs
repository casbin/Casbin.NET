using NetCasbin.Persist;
using NetCasbin.Persist.FileAdapter;
using System;
using System.IO;
using Xunit;

namespace NetCasbin.Test
{
    public class EnforcerUnitTest : TestUtil
    {
        [Fact]
        public void TestKeyMatchModelInMemory()
        {
            Model.Model m = CoreEnforcer.NewModel();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

            IAdapter a = new DefaultFileAdapter("examples/keymatch_policy.csv");

            Enforcer e = new Enforcer(m, a);

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
        public void TestKeyMatchModelInMemoryDeny()
        {
            Model.Model m = CoreEnforcer.NewModel();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("e", "e", "!some(where (p.eft == deny))");
            m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

            IAdapter a = new DefaultFileAdapter("examples/keymatch_policy.csv");

            Enforcer e = new Enforcer(m, a);

            TestEnforce(e, "alice", "/alice_data/resource2", "POST", true);
        }

        [Fact]
        public void TestRBACModelInMemoryIndeterminate()
        {
            Model.Model m = CoreEnforcer.NewModel();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("g", "g", "_, _");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

            Enforcer e = new Enforcer(m);

            e.AddPermissionForUser("alice", "data1", "invalid");

            TestEnforce(e, "alice", "data1", "read", false);
        }

        [Fact]
        public void TestRBACModelInMemory()
        {
            Model.Model m = CoreEnforcer.NewModel();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("g", "g", "_, _");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

            Enforcer e = new Enforcer(m);

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
        public void TestRBACModelInMemory2()
        {
            String text =
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

            Model.Model m = CoreEnforcer.NewModel(text);

            Enforcer e = new Enforcer(m);

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
        public void TestNotUsedRBACModelInMemory()
        {
            Model.Model m = CoreEnforcer.NewModel();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("g", "g", "_, _");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

            Enforcer e = new Enforcer(m);

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
        public void TestReloadPolicy()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            e.LoadPolicy();
            TestGetPolicy(e, AsList(AsList("alice", "data1", "read"), AsList("bob", "data2", "write"), AsList("data2_admin", "data2", "read"), AsList("data2_admin", "data2", "write")));
        }

        [Fact]
        public void TestSavePolicy()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            e.SavePolicy();
        }


        [Fact]
        public void TestSavePolicyWithoutBasicModel()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");

            e.SavePolicy();
        }

        [Fact]
        public void TestClearPolicy()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            e.ClearPolicy();
        }

        [Fact]
        public void TestEnableEnforce()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");

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
            Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv", true);
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
            Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");

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
        public void TestInitWithAdapter()
        {
            IAdapter adapter = new DefaultFileAdapter("examples/basic_policy.csv");
            Enforcer e = new Enforcer("examples/basic_model.conf", adapter);

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
            Enforcer e = new Enforcer("examples/rbac_model.conf");
            e.EnableAutoBuildRoleLinks(false);
            e.BuildRoleLinks();
            e.Enforce("user501", "data9", "read");
        }

        [Fact]
        public void TestGetAndSetModel()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");
            Enforcer e2 = new Enforcer("examples/basic_with_root_model.conf", "examples/basic_policy.csv");

            TestEnforce(e, "root", "data1", "read", false);

            e.SetModel(e2.GetModel());

            TestEnforce(e, "root", "data1", "read", true);
        }

        [Fact]
        public void TestGetAndSetAdapterInMem()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");
            Enforcer e2 = new Enforcer("examples/basic_model.conf", "examples/basic_inverse_policy.csv");

            TestEnforce(e, "alice", "data1", "read", true);
            TestEnforce(e, "alice", "data1", "write", false);

            IAdapter a2 = e2.GetAdapter();
            e.SetAdapter(a2);
            e.LoadPolicy();

            TestEnforce(e, "alice", "data1", "read", false);
            TestEnforce(e, "alice", "data1", "write", true);
        }

        [Fact]
        public void TestSetAdapterFromFile()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf");

            TestEnforce(e, "alice", "data1", "read", false);

            IAdapter a = new DefaultFileAdapter("examples/basic_policy.csv");
            e.SetAdapter(a);
            e.LoadPolicy();

            TestEnforce(e, "alice", "data1", "read", true);
        }

        [Fact]
        public void TestInitEmpty()
        {
            Enforcer e = new Enforcer();

            Model.Model m = CoreEnforcer.NewModel();
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
        public void TestInitEmptyByInputStream()
        {
            Enforcer e = new Enforcer();

            Model.Model m = CoreEnforcer.NewModel();
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

    }
}
