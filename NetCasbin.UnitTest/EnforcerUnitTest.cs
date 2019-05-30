using NetCasbin.Persist;
using NetCasbin.Persist.FileAdapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace NetCasbin.Test
{
    public class EnforcerUnitTest : TestUtil
    {
        [Fact]
        public void testKeyMatchModelInMemory()
        {
            Model m = CoreEnforcer.NewModel();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

            IAdapter a = new DefaultFileAdapter("examples/keymatch_policy.csv");

            Enforcer e = new Enforcer(m, a);

            testEnforce(e, "alice", "/alice_data/resource1", "GET", true);
            testEnforce(e, "alice", "/alice_data/resource1", "POST", true);
            testEnforce(e, "alice", "/alice_data/resource2", "GET", true);
            testEnforce(e, "alice", "/alice_data/resource2", "POST", false);
            testEnforce(e, "alice", "/bob_data/resource1", "GET", false);
            testEnforce(e, "alice", "/bob_data/resource1", "POST", false);
            testEnforce(e, "alice", "/bob_data/resource2", "GET", false);
            testEnforce(e, "alice", "/bob_data/resource2", "POST", false);

            testEnforce(e, "bob", "/alice_data/resource1", "GET", false);
            testEnforce(e, "bob", "/alice_data/resource1", "POST", false);
            testEnforce(e, "bob", "/alice_data/resource2", "GET", true);
            testEnforce(e, "bob", "/alice_data/resource2", "POST", false);
            testEnforce(e, "bob", "/bob_data/resource1", "GET", false);
            testEnforce(e, "bob", "/bob_data/resource1", "POST", true);
            testEnforce(e, "bob", "/bob_data/resource2", "GET", false);
            testEnforce(e, "bob", "/bob_data/resource2", "POST", true);

            testEnforce(e, "cathy", "/cathy_data", "GET", true);
            testEnforce(e, "cathy", "/cathy_data", "POST", true);
            testEnforce(e, "cathy", "/cathy_data", "DELETE", false);

            e = new Enforcer(m);
            a.LoadPolicy(e.GetModel());

            testEnforce(e, "alice", "/alice_data/resource1", "GET", true);
            testEnforce(e, "alice", "/alice_data/resource1", "POST", true);
            testEnforce(e, "alice", "/alice_data/resource2", "GET", true);
            testEnforce(e, "alice", "/alice_data/resource2", "POST", false);
            testEnforce(e, "alice", "/bob_data/resource1", "GET", false);
            testEnforce(e, "alice", "/bob_data/resource1", "POST", false);
            testEnforce(e, "alice", "/bob_data/resource2", "GET", false);
            testEnforce(e, "alice", "/bob_data/resource2", "POST", false);

            testEnforce(e, "bob", "/alice_data/resource1", "GET", false);
            testEnforce(e, "bob", "/alice_data/resource1", "POST", false);
            testEnforce(e, "bob", "/alice_data/resource2", "GET", true);
            testEnforce(e, "bob", "/alice_data/resource2", "POST", false);
            testEnforce(e, "bob", "/bob_data/resource1", "GET", false);
            testEnforce(e, "bob", "/bob_data/resource1", "POST", true);
            testEnforce(e, "bob", "/bob_data/resource2", "GET", false);
            testEnforce(e, "bob", "/bob_data/resource2", "POST", true);

            testEnforce(e, "cathy", "/cathy_data", "GET", true);
            testEnforce(e, "cathy", "/cathy_data", "POST", true);
            testEnforce(e, "cathy", "/cathy_data", "DELETE", false);
        }

        [Fact]
        public void testKeyMatchModelInMemoryDeny()
        {
            Model m = CoreEnforcer.NewModel();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("e", "e", "!some(where (p.eft == deny))");
            m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

            IAdapter a = new DefaultFileAdapter("examples/keymatch_policy.csv");

            Enforcer e = new Enforcer(m, a);

            testEnforce(e, "alice", "/alice_data/resource2", "POST", true);
        }

        [Fact]
        public void testRBACModelInMemoryIndeterminate()
        {
            Model m = CoreEnforcer.NewModel();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("g", "g", "_, _");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

            Enforcer e = new Enforcer(m);

            e.AddPermissionForUser("alice", "data1", "invalid");

            testEnforce(e, "alice", "data1", "read", false);
        }

        [Fact]
        public void testRBACModelInMemory()
        {
            Model m = CoreEnforcer.NewModel();
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

            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", true);
            testEnforce(e, "alice", "data2", "write", true);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", true);
        }

        [Fact]
        public void testRBACModelInMemory2()
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

            Model m = CoreEnforcer.NewModel(text);

            Enforcer e = new Enforcer(m);

            e.AddPermissionForUser("alice", "data1", "read");
            e.AddPermissionForUser("bob", "data2", "write");
            e.AddPermissionForUser("data2_admin", "data2", "read");
            e.AddPermissionForUser("data2_admin", "data2", "write");
            e.AddRoleForUser("alice", "data2_admin");

            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", true);
            testEnforce(e, "alice", "data2", "write", true);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", true);
        }

        [Fact]
        public void testNotUsedRBACModelInMemory()
        {
            Model m = CoreEnforcer.NewModel();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("g", "g", "_, _");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

            Enforcer e = new Enforcer(m);

            e.AddPermissionForUser("alice", "data1", "read");
            e.AddPermissionForUser("bob", "data2", "write");

            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", false);
            testEnforce(e, "alice", "data2", "write", false);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", true);
        }

        [Fact]
        public void testReloadPolicy()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            e.LoadPolicy();
            testGetPolicy(e, asList(asList("alice", "data1", "read"), asList("bob", "data2", "write"), asList("data2_admin", "data2", "read"), asList("data2_admin", "data2", "write")));
        }

        [Fact]
        public void testSavePolicy()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            e.SavePolicy();
        }

        [Fact]
        public void testClearPolicy()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            e.ClearPolicy();
        }

        [Fact]
        public void testEnableEnforce()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");

            e.EnableEnforce(false);
            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", true);
            testEnforce(e, "alice", "data2", "read", true);
            testEnforce(e, "alice", "data2", "write", true);
            testEnforce(e, "bob", "data1", "read", true);
            testEnforce(e, "bob", "data1", "write", true);
            testEnforce(e, "bob", "data2", "read", true);
            testEnforce(e, "bob", "data2", "write", true);

            e.EnableEnforce(true);
            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", false);
            testEnforce(e, "alice", "data2", "write", false);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", true);
        }

        [Fact]
        public void testEnableLog()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv", true);
            // The log is enabled by default, so the above is the same with:
            // Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");

            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", false);
            testEnforce(e, "alice", "data2", "write", false);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", true);


            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", false);
            testEnforce(e, "alice", "data2", "write", false);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", true);
        }

        [Fact]
        public void testEnableAutoSave()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");

            e.EnableAutoSave(false);
            // Because AutoSave is disabled, the policy change only affects the policy in Casbin enforcer,
            // it doesn't affect the policy in the storage.
            e.RemovePolicy("alice", "data1", "read");
            // Reload the policy from the storage to see the effect.
            e.LoadPolicy();
            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", false);
            testEnforce(e, "alice", "data2", "write", false);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", true);

            e.EnableAutoSave(true);
            // Because AutoSave is enabled, the policy change not only affects the policy in Casbin enforcer,
            // but also affects the policy in the storage.
            e.RemovePolicy("alice", "data1", "read");

            // However, the file adapter doesn't implement the AutoSave feature, so enabling it has no effect at all here.

            // Reload the policy from the storage to see the effect.
            e.LoadPolicy();
            testEnforce(e, "alice", "data1", "read", true); // Will not be false here.
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", false);
            testEnforce(e, "alice", "data2", "write", false);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", true);
        }

        [Fact]
        public void testInitWithAdapter()
        {
            IAdapter adapter = new DefaultFileAdapter("examples/basic_policy.csv");
            Enforcer e = new Enforcer("examples/basic_model.conf", adapter);

            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", false);
            testEnforce(e, "alice", "data2", "write", false);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", true);
        }

        [Fact]
        public void testRoleLinks()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf");
            e.EnableAutoBuildRoleLinks(false);
            e.BuildRoleLinks();
            e.Enforce("user501", "data9", "read");
        }

        [Fact]
        public void testGetAndSetModel()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");
            Enforcer e2 = new Enforcer("examples/basic_with_root_model.conf", "examples/basic_policy.csv");

            testEnforce(e, "root", "data1", "read", false);

            e.SetModel(e2.GetModel());

            testEnforce(e, "root", "data1", "read", true);
        }

        [Fact]
        public void testGetAndSetAdapterInMem()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");
            Enforcer e2 = new Enforcer("examples/basic_model.conf", "examples/basic_inverse_policy.csv");

            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);

            IAdapter a2 = e2.GetAdapter();
            e.SetAdapter(a2);
            e.LoadPolicy();

            testEnforce(e, "alice", "data1", "read", false);
            testEnforce(e, "alice", "data1", "write", true);
        }

        [Fact]
        public void testSetAdapterFromFile()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf");

            testEnforce(e, "alice", "data1", "read", false);

            IAdapter a = new DefaultFileAdapter("examples/basic_policy.csv");
            e.SetAdapter(a);
            e.LoadPolicy();

            testEnforce(e, "alice", "data1", "read", true);
        }

        [Fact]
        public void testInitEmpty()
        {
            Enforcer e = new Enforcer();

            Model m = CoreEnforcer.NewModel();
            m.AddDef("r", "r", "sub, obj, act");
            m.AddDef("p", "p", "sub, obj, act");
            m.AddDef("e", "e", "some(where (p.eft == allow))");
            m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

            IAdapter a = new DefaultFileAdapter("examples/keymatch_policy.csv");

            e.SetModel(m);
            e.SetAdapter(a);
            e.LoadPolicy();

            testEnforce(e, "alice", "/alice_data/resource1", "GET", true);
        }

        [Fact]
        public void testInitEmptyByInputStream()
        {
            Enforcer e = new Enforcer();

            Model m = CoreEnforcer.NewModel();
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

                testEnforce(e, "alice", "/alice_data/resource1", "GET", true);
            }

        }

    }
}
