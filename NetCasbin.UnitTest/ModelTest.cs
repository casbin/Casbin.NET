using NetCasbin.Rbac;
using System;
using System.Collections.Generic;
using Xunit;

namespace NetCasbin.Test
{
    public class ModelTest : TestUtil
    {
        [Fact]
        public void Test_BasicModel()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf", "examples/basic_policy.csv");

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
        public void Test_BasicModelNoPolicy()
        {
            Enforcer e = new Enforcer("examples/basic_model.conf");

            TestEnforce(e, "alice", "data1", "read", false);
            TestEnforce(e, "alice", "data1", "write", false);
            TestEnforce(e, "alice", "data2", "read", false);
            TestEnforce(e, "alice", "data2", "write", false);
            TestEnforce(e, "bob", "data1", "read", false);
            TestEnforce(e, "bob", "data1", "write", false);
            TestEnforce(e, "bob", "data2", "read", false);
            TestEnforce(e, "bob", "data2", "write", false);
        }

        [Fact]
        public void Test_BasicModelWithRoot()
        {
            Enforcer e = new Enforcer("examples/basic_with_root_model.conf", "examples/basic_policy.csv");

            TestEnforce(e, "alice", "data1", "read", true);
            TestEnforce(e, "alice", "data1", "write", false);
            TestEnforce(e, "alice", "data2", "read", false);
            TestEnforce(e, "alice", "data2", "write", false);
            TestEnforce(e, "bob", "data1", "read", false);
            TestEnforce(e, "bob", "data1", "write", false);
            TestEnforce(e, "bob", "data2", "read", false);
            TestEnforce(e, "bob", "data2", "write", true);
            TestEnforce(e, "root", "data1", "read", true);
            TestEnforce(e, "root", "data1", "write", true);
            TestEnforce(e, "root", "data2", "read", true);
            TestEnforce(e, "root", "data2", "write", true);
        }

        [Fact]
        public void Test_BasicModelWithRootNoPolicy()
        {
            Enforcer e = new Enforcer("examples/basic_with_root_model.conf");

            TestEnforce(e, "alice", "data1", "read", false);
            TestEnforce(e, "alice", "data1", "write", false);
            TestEnforce(e, "alice", "data2", "read", false);
            TestEnforce(e, "alice", "data2", "write", false);
            TestEnforce(e, "bob", "data1", "read", false);
            TestEnforce(e, "bob", "data1", "write", false);
            TestEnforce(e, "bob", "data2", "read", false);
            TestEnforce(e, "bob", "data2", "write", false);
            TestEnforce(e, "root", "data1", "read", true);
            TestEnforce(e, "root", "data1", "write", true);
            TestEnforce(e, "root", "data2", "read", true);
            TestEnforce(e, "root", "data2", "write", true);
        }

        [Fact]
        public void Test_BasicModelWithoutUsers()
        {
            Enforcer e = new Enforcer("examples/basic_without_users_model.conf", "examples/basic_without_users_policy.csv");

            TestEnforceWithoutUsers(e, "data1", "read", true);
            TestEnforceWithoutUsers(e, "data1", "write", false);
            TestEnforceWithoutUsers(e, "data2", "read", false);
            TestEnforceWithoutUsers(e, "data2", "write", true);
        }

        [Fact]
        public void Test_BasicModelWithoutResources()
        {
            Enforcer e = new Enforcer("examples/basic_without_resources_model.conf", "examples/basic_without_resources_policy.csv");

            TestEnforceWithoutUsers(e, "alice", "read", true);
            TestEnforceWithoutUsers(e, "alice", "write", false);
            TestEnforceWithoutUsers(e, "bob", "read", false);
            TestEnforceWithoutUsers(e, "bob", "write", true);
        }

        [Fact]
        public void Test_RBACModel()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

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
        public void Test_RBACModelWithResourceRoles()
        {
            Enforcer e = new Enforcer("examples/rbac_with_resource_roles_model.conf", "examples/rbac_with_resource_roles_policy.csv");

            TestEnforce(e, "alice", "data1", "read", true);
            TestEnforce(e, "alice", "data1", "write", true);
            TestEnforce(e, "alice", "data2", "read", false);
            TestEnforce(e, "alice", "data2", "write", true);
            TestEnforce(e, "bob", "data1", "read", false);
            TestEnforce(e, "bob", "data1", "write", false);
            TestEnforce(e, "bob", "data2", "read", false);
            TestEnforce(e, "bob", "data2", "write", true);
        }

        [Fact]
        public void Test_RBACModelWithDomains()
        {
            Enforcer e = new Enforcer("examples/rbac_with_domains_model.conf", "examples/rbac_with_domains_policy.csv");

            TestDomainEnforce(e, "alice", "domain1", "data1", "read", true);
            TestDomainEnforce(e, "alice", "domain1", "data1", "write", true);
            TestDomainEnforce(e, "alice", "domain1", "data2", "read", false);
            TestDomainEnforce(e, "alice", "domain1", "data2", "write", false);
            TestDomainEnforce(e, "bob", "domain2", "data1", "read", false);
            TestDomainEnforce(e, "bob", "domain2", "data1", "write", false);
            TestDomainEnforce(e, "bob", "domain2", "data2", "read", true);
            TestDomainEnforce(e, "bob", "domain2", "data2", "write", true);
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

            TestDomainEnforce(e, "alice", "domain1", "data1", "read", true);
            TestDomainEnforce(e, "alice", "domain1", "data1", "write", true);
            TestDomainEnforce(e, "alice", "domain1", "data2", "read", false);
            TestDomainEnforce(e, "alice", "domain1", "data2", "write", false);
            TestDomainEnforce(e, "bob", "domain2", "data1", "read", false);
            TestDomainEnforce(e, "bob", "domain2", "data1", "write", false);
            TestDomainEnforce(e, "bob", "domain2", "data2", "read", true);
            TestDomainEnforce(e, "bob", "domain2", "data2", "write", true);

            // Remove all policy rules related to domain1 and data1.
            e.RemoveFilteredPolicy(1, "domain1", "data1");

            TestDomainEnforce(e, "alice", "domain1", "data1", "read", false);
            TestDomainEnforce(e, "alice", "domain1", "data1", "write", false);
            TestDomainEnforce(e, "alice", "domain1", "data2", "read", false);
            TestDomainEnforce(e, "alice", "domain1", "data2", "write", false);
            TestDomainEnforce(e, "bob", "domain2", "data1", "read", false);
            TestDomainEnforce(e, "bob", "domain2", "data1", "write", false);
            TestDomainEnforce(e, "bob", "domain2", "data2", "read", true);
            TestDomainEnforce(e, "bob", "domain2", "data2", "write", true);

            // Remove the specified policy rule.
            e.RemovePolicy("admin", "domain2", "data2", "read");

            TestDomainEnforce(e, "alice", "domain1", "data1", "read", false);
            TestDomainEnforce(e, "alice", "domain1", "data1", "write", false);
            TestDomainEnforce(e, "alice", "domain1", "data2", "read", false);
            TestDomainEnforce(e, "alice", "domain1", "data2", "write", false);
            TestDomainEnforce(e, "bob", "domain2", "data1", "read", false);
            TestDomainEnforce(e, "bob", "domain2", "data1", "write", false);
            TestDomainEnforce(e, "bob", "domain2", "data2", "read", false);
            TestDomainEnforce(e, "bob", "domain2", "data2", "write", true);
        }

        [Fact]
        public void Test_RBACModelWithDeny()
        {
            Enforcer e = new Enforcer("examples/rbac_with_deny_model.conf", "examples/rbac_with_deny_policy.csv");

            TestEnforce(e, "alice", "data1", "read", true);
            TestEnforce(e, "alice", "data1", "write", false);
            TestEnforce(e, "alice", "data2", "read", true);
            TestEnforce(e, "alice", "data2", "write", false);
            TestEnforce(e, "bob", "data1", "read", false);
            TestEnforce(e, "bob", "data1", "write", false);
            TestEnforce(e, "bob", "data2", "read", false);
            TestEnforce(e, "bob", "data2", "write", true);
        }

        [Fact]
        public void Test_RBACModelWithOnlyDeny()
        {
            Enforcer e = new Enforcer("examples/rbac_with_not_deny_model.conf", "examples/rbac_with_deny_policy.csv");

            TestEnforce(e, "alice", "data2", "write", false);
        }

        [Fact]
        public void Test_RBACModelWithCustomData()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            // You can add custom data to a grouping policy, Casbin will ignore it. It is only meaningful to the caller.
            // This feature can be used to store information like whether "bob" is an end user (so no subject will inherit "bob")
            // For Casbin, it is equivalent to: e.addGroupingPolicy("bob", "data2_admin")
            e.AddGroupingPolicy("bob", "data2_admin", "custom_data");

            TestEnforce(e, "alice", "data1", "read", true);
            TestEnforce(e, "alice", "data1", "write", false);
            TestEnforce(e, "alice", "data2", "read", true);
            TestEnforce(e, "alice", "data2", "write", true);
            TestEnforce(e, "bob", "data1", "read", false);
            TestEnforce(e, "bob", "data1", "write", false);
            TestEnforce(e, "bob", "data2", "read", true);
            TestEnforce(e, "bob", "data2", "write", true);

            // You should also take the custom data as a parameter when deleting a grouping policy.
            // e.removeGroupingPolicy("bob", "data2_admin") won't work.
            // Or you can remove it by using removeFilteredGroupingPolicy().
            e.RemoveGroupingPolicy("bob", "data2_admin", "custom_data");

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
        public void Test_RBACModelWithCustomRoleManager()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");
            e.SetRoleManager(new CustomRoleManager());
            e.LoadModel();
            e.LoadPolicy();

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
        public void Test_ABACModel()
        {
            Enforcer e = new Enforcer("examples/abac_model.conf");

            TestResource data1 = new TestResource("data1", "alice");
            TestResource data2 = new TestResource("data2", "bob");

            TestEnforce(e, "alice", data1, "read", true);
            TestEnforce(e, "alice", data1, "write", true);
            TestEnforce(e, "alice", data2, "read", false);
            TestEnforce(e, "alice", data2, "write", false);
            TestEnforce(e, "bob", data1, "read", false);
            TestEnforce(e, "bob", data1, "write", false);
            TestEnforce(e, "bob", data2, "read", true);
            TestEnforce(e, "bob", data2, "write", true);
        }

        [Fact]
        public void Test_KeyMatchModel()
        {
            Enforcer e = new Enforcer("examples/keymatch_model.conf", "examples/keymatch_policy.csv");

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
        public void Test_PriorityModelIndeterminate()
        {
            Enforcer e = new Enforcer("examples/priority_model.conf", "examples/priority_indeterminate_policy.csv");

            TestEnforce(e, "alice", "data1", "read", false);
        }

        [Fact]
        public void Test_PriorityModel()
        {
            Enforcer e = new Enforcer("examples/priority_model.conf", "examples/priority_policy.csv");

            TestEnforce(e, "alice", "data1", "read", true);
            TestEnforce(e, "alice", "data1", "write", false);
            TestEnforce(e, "alice", "data2", "read", false);
            TestEnforce(e, "alice", "data2", "write", false);
            TestEnforce(e, "bob", "data1", "read", false);
            TestEnforce(e, "bob", "data1", "write", false);
            TestEnforce(e, "bob", "data2", "read", true);
            TestEnforce(e, "bob", "data2", "write", false);
        }

        [Fact]
        public void Test_KeyMatch2Model()
        {
            Enforcer e = new Enforcer("examples/keymatch2_model.conf", "examples/keymatch2_policy.csv");

            TestEnforce(e, "alice", "/alice_data", "GET", false);
            TestEnforce(e, "alice", "/alice_data/resource1", "GET", true);
            TestEnforce(e, "alice", "/alice_data2/myid", "GET", false);
            TestEnforce(e, "alice", "/alice_data2/myid/using/res_id", "GET", true);
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
            public String Name { set; get; }
            public String Owner { set; get; }

            public TestResource(String name, String owner)
            {
                this.Name = name;
                this.Owner = owner;
            }

        }

    }
}
