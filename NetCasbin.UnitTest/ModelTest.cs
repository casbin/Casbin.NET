using NetCasbin.Rabc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NetCasbin.Test
{
    public class ModelTest : TestUtil
    {
        [Fact]
        public void Test_BasicModel()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");

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
        public void Test_BasicModelNoPolicy()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf");

            testEnforce(e, "alice", "data1", "read", false);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", false);
            testEnforce(e, "alice", "data2", "write", false);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", false);
        }

        [Fact]
        public void Test_BasicModelWithRoot()
        {
            Enforcer e = new Enforcer("examples/basic_with_root_model.conf", "examples/basic_policy.csv");

            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", false);
            testEnforce(e, "alice", "data2", "write", false);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", true);
            testEnforce(e, "root", "data1", "read", true);
            testEnforce(e, "root", "data1", "write", true);
            testEnforce(e, "root", "data2", "read", true);
            testEnforce(e, "root", "data2", "write", true);
        }

        [Fact]
        public void Test_BasicModelWithRootNoPolicy()
        {
            Enforcer e = new Enforcer("examples/basic_with_root_model.conf");

            testEnforce(e, "alice", "data1", "read", false);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", false);
            testEnforce(e, "alice", "data2", "write", false);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", false);
            testEnforce(e, "root", "data1", "read", true);
            testEnforce(e, "root", "data1", "write", true);
            testEnforce(e, "root", "data2", "read", true);
            testEnforce(e, "root", "data2", "write", true);
        }

        [Fact]
        public void Test_BasicModelWithoutUsers()
        {
            Enforcer e = new Enforcer("examples/basic_without_users_model.conf", "examples/basic_without_users_policy.csv");

            testEnforceWithoutUsers(e, "data1", "read", true);
            testEnforceWithoutUsers(e, "data1", "write", false);
            testEnforceWithoutUsers(e, "data2", "read", false);
            testEnforceWithoutUsers(e, "data2", "write", true);
        }

        [Fact]
        public void Test_BasicModelWithoutResources()
        {
            Enforcer e = new Enforcer("examples/basic_without_resources_model.conf", "examples/basic_without_resources_policy.csv");

            testEnforceWithoutUsers(e, "alice", "read", true);
            testEnforceWithoutUsers(e, "alice", "write", false);
            testEnforceWithoutUsers(e, "bob", "read", false);
            testEnforceWithoutUsers(e, "bob", "write", true);
        }

        [Fact]
        public void Test_RBACModel()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

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
        public void Test_RBACModelWithResourceRoles()
        {
            Enforcer e = new Enforcer("examples/rbac_with_resource_roles_model.conf", "examples/rbac_with_resource_roles_policy.csv");

            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", true);
            testEnforce(e, "alice", "data2", "read", false);
            testEnforce(e, "alice", "data2", "write", true);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", true);
        }

        [Fact]
        public void Test_RBACModelWithDomains()
        {
            Enforcer e = new Enforcer("examples/rbac_with_domains_model.conf", "examples/rbac_with_domains_policy.csv");

            testDomainEnforce(e, "alice", "domain1", "data1", "read", true);
            testDomainEnforce(e, "alice", "domain1", "data1", "write", true);
            testDomainEnforce(e, "alice", "domain1", "data2", "read", false);
            testDomainEnforce(e, "alice", "domain1", "data2", "write", false);
            testDomainEnforce(e, "bob", "domain2", "data1", "read", false);
            testDomainEnforce(e, "bob", "domain2", "data1", "write", false);
            testDomainEnforce(e, "bob", "domain2", "data2", "read", true);
            testDomainEnforce(e, "bob", "domain2", "data2", "write", true);
        }

        [Fact]
        public void Test_RBACModelWithDomainsAtRuntime()
        {
            Enforcer e = new Enforcer("examples/rbac_with_domains_model.conf");

            e.AddPolicy("admin", "domain1", "data1", "read");
            e.AddPolicy("admin", "domain1", "data1", "write");
            e.AddPolicy("admin", "domain2", "data2", "read");
            e.AddPolicy("admin", "domain2", "data2", "write");

            e.AddGroupingPolicy("alice", "admin", "domain1");
            e.AddGroupingPolicy("bob", "admin", "domain2");

            testDomainEnforce(e, "alice", "domain1", "data1", "read", true);
            testDomainEnforce(e, "alice", "domain1", "data1", "write", true);
            testDomainEnforce(e, "alice", "domain1", "data2", "read", false);
            testDomainEnforce(e, "alice", "domain1", "data2", "write", false);
            testDomainEnforce(e, "bob", "domain2", "data1", "read", false);
            testDomainEnforce(e, "bob", "domain2", "data1", "write", false);
            testDomainEnforce(e, "bob", "domain2", "data2", "read", true);
            testDomainEnforce(e, "bob", "domain2", "data2", "write", true);

            // Remove all policy rules related to domain1 and data1.
            e.RemoveFilteredPolicy(1, "domain1", "data1");

            testDomainEnforce(e, "alice", "domain1", "data1", "read", false);
            testDomainEnforce(e, "alice", "domain1", "data1", "write", false);
            testDomainEnforce(e, "alice", "domain1", "data2", "read", false);
            testDomainEnforce(e, "alice", "domain1", "data2", "write", false);
            testDomainEnforce(e, "bob", "domain2", "data1", "read", false);
            testDomainEnforce(e, "bob", "domain2", "data1", "write", false);
            testDomainEnforce(e, "bob", "domain2", "data2", "read", true);
            testDomainEnforce(e, "bob", "domain2", "data2", "write", true);

            // Remove the specified policy rule.
            e.RemovePolicy("admin", "domain2", "data2", "read");

            testDomainEnforce(e, "alice", "domain1", "data1", "read", false);
            testDomainEnforce(e, "alice", "domain1", "data1", "write", false);
            testDomainEnforce(e, "alice", "domain1", "data2", "read", false);
            testDomainEnforce(e, "alice", "domain1", "data2", "write", false);
            testDomainEnforce(e, "bob", "domain2", "data1", "read", false);
            testDomainEnforce(e, "bob", "domain2", "data1", "write", false);
            testDomainEnforce(e, "bob", "domain2", "data2", "read", false);
            testDomainEnforce(e, "bob", "domain2", "data2", "write", true);
        }

        [Fact]
        public void Test_RBACModelWithDeny()
        {
            Enforcer e = new Enforcer("examples/rbac_with_deny_model.conf", "examples/rbac_with_deny_policy.csv");

            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", true);
            testEnforce(e, "alice", "data2", "write", false);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", true);
        }

        [Fact]
        public void Test_RBACModelWithOnlyDeny()
        {
            Enforcer e = new Enforcer("examples/rbac_with_not_deny_model.conf", "examples/rbac_with_deny_policy.csv");

            testEnforce(e, "alice", "data2", "write", false);
        }

        [Fact]
        public void Test_RBACModelWithCustomData()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            // You can add custom data to a grouping policy, Casbin will ignore it. It is only meaningful to the caller.
            // This feature can be used to store information like whether "bob" is an end user (so no subject will inherit "bob")
            // For Casbin, it is equivalent to: e.addGroupingPolicy("bob", "data2_admin")
            e.AddGroupingPolicy("bob", "data2_admin", "custom_data");

            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", true);
            testEnforce(e, "alice", "data2", "write", true);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", true);
            testEnforce(e, "bob", "data2", "write", true);

            // You should also take the custom data as a parameter when deleting a grouping policy.
            // e.removeGroupingPolicy("bob", "data2_admin") won't work.
            // Or you can remove it by using removeFilteredGroupingPolicy().
            e.RemoveGroupingPolicy("bob", "data2_admin", "custom_data");

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
        public void Test_RBACModelWithCustomRoleManager()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");
            e.SetRoleManager(new CustomRoleManager());
            e.LoadModel();
            e.LoadPolicy();

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
        public void Test_ABACModel()
        {
            Enforcer e = new Enforcer("examples/abac_model.conf");

            TestResource data1 = new TestResource("data1", "alice");
            TestResource data2 = new TestResource("data2", "bob");

            testEnforce(e, "alice", data1, "read", true);
            testEnforce(e, "alice", data1, "write", true);
            testEnforce(e, "alice", data2, "read", false);
            testEnforce(e, "alice", data2, "write", false);
            testEnforce(e, "bob", data1, "read", false);
            testEnforce(e, "bob", data1, "write", false);
            testEnforce(e, "bob", data2, "read", true);
            testEnforce(e, "bob", data2, "write", true);
        }

        [Fact]
        public void Test_KeyMatchModel()
        {
            Enforcer e = new Enforcer("examples/keymatch_model.conf", "examples/keymatch_policy.csv");

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
        public void Test_PriorityModelIndeterminate()
        {
            Enforcer e = new Enforcer("examples/priority_model.conf", "examples/priority_indeterminate_policy.csv");

            testEnforce(e, "alice", "data1", "read", false);
        }

        [Fact]
        public void Test_PriorityModel()
        {
            Enforcer e = new Enforcer("examples/priority_model.conf", "examples/priority_policy.csv");

            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", false);
            testEnforce(e, "alice", "data2", "write", false);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", true);
            testEnforce(e, "bob", "data2", "write", false);
        }

        [Fact]
        public void Test_KeyMatch2Model()
        {
            Enforcer e = new Enforcer("examples/keymatch2_model.conf", "examples/keymatch2_policy.csv");

            testEnforce(e, "alice", "/alice_data", "GET", false);
            testEnforce(e, "alice", "/alice_data/resource1", "GET", true);
            testEnforce(e, "alice", "/alice_data2/myid", "GET", false);
            testEnforce(e, "alice", "/alice_data2/myid/using/res_id", "GET", true);
        }


        public class CustomRoleManager : IRoleManager {

            public void Clear() { }
            public void AddLink(String name1, String name2, params string[] domain) { }
            public void DeleteLink(String name1, String name2, params string[] domain) { }

            public Boolean HasLink(String name1, String name2, params string[] domain)
            {
                if (name1.Equals("alice") && name2.Equals("alice"))
                {
                    return true;
                }
                else if (name1.Equals("alice") && name2.Equals("data2_admin"))
                {
                    return true;
                }
                else if (name1.Equals("bob") && name2.Equals("bob"))
                {
                    return true;
                }
                return false;
            }

            public List<String> GetRoles(String name, params string[] domain) { return null; }
            public List<String> GetUsers(String name, params string[] domain) { return null; }
        }

        public class TestResource
        {
            public String name { set; get; }
            public String owner { set; get; }

            public TestResource(String name, String owner)
            {
                this.name = name;
                this.owner = owner;
            }

        }

    }
}
