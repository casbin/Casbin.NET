using System.Collections.Generic;
using System.Threading.Tasks;
using NetCasbin.Rbac;
using NetCasbin.UnitTest.Fixtures;
using Xunit;
using static NetCasbin.UnitTest.Util.TestUtil;

namespace NetCasbin.UnitTest
{
    [Collection("Model collection")]
    public class ModelTest
    {
        private readonly TestModelFixture _testModelFixture;

        public ModelTest(TestModelFixture testModelFixture)
        {
            _testModelFixture = testModelFixture;
        }

        [Fact]
        public void TestBasicModel()
        {
            var e = new Enforcer(_testModelFixture.GetBasicTestModel());

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
        public void TestBasicModelNoPolicy()
        {
            var e = new Enforcer(TestModelFixture.GetNewTestModel(_testModelFixture._basicModelText));

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
        public void TestBasicModelWithRoot()
        {
            var e = new Enforcer(TestModelFixture.GetNewTestModel(
                _testModelFixture._basicWithRootModelText,
                _testModelFixture._basicPolicyText));

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
        public void TestBasicModelWithRootNoPolicy()
        {
            var e = new Enforcer(TestModelFixture.GetNewTestModel(_testModelFixture._basicWithRootModelText));

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
        public void TestBasicModelWithoutUsers()
        {
            var e = new Enforcer(_testModelFixture.GetBasicWithoutUserTestModel());

            TestEnforceWithoutUsers(e, "data1", "read", true);
            TestEnforceWithoutUsers(e, "data1", "write", false);
            TestEnforceWithoutUsers(e, "data2", "read", false);
            TestEnforceWithoutUsers(e, "data2", "write", true);
        }

        [Fact]
        public void TestBasicModelWithoutResources()
        {
            var e = new Enforcer(_testModelFixture.GetBasicWithoutResourceTestModel());

            TestEnforceWithoutUsers(e, "alice", "read", true);
            TestEnforceWithoutUsers(e, "alice", "write", false);
            TestEnforceWithoutUsers(e, "bob", "read", false);
            TestEnforceWithoutUsers(e, "bob", "write", true);
        }

        [Fact]
        public void TestRbacModel()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.BuildRoleLinks();

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
        public void TestRbacModelWithResourceRoles()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacWithResourceRoleTestModel());
            e.BuildRoleLinks();

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
        public void TestRbacModelWithDomains()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacWithDomainsTestModel());
            e.BuildRoleLinks();

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
        public void TestRbacModelWithDomainsAtRuntime()
        {
            var e = new Enforcer(TestModelFixture.GetNewTestModel(_testModelFixture._rbacWithDomainsModelText));
            e.BuildRoleLinks();

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
        public async Task TestRbacModelWithDomainsAtRuntimeAsync()
        {
            var e = new Enforcer(TestModelFixture.GetNewTestModel(_testModelFixture._rbacWithDomainsModelText));
            e.BuildRoleLinks();

            await e.AddPolicyAsync("admin", "domain1", "data1", "read");
            await e.AddPolicyAsync("admin", "domain1", "data1", "write");
            await e.AddPolicyAsync("admin", "domain2", "data2", "read");
            await e.AddPolicyAsync("admin", "domain2", "data2", "write");

            await e.AddGroupingPolicyAsync("alice", "admin", "domain1");
            await e.AddGroupingPolicyAsync("bob", "admin", "domain2");

            TestDomainEnforce(e, "alice", "domain1", "data1", "read", true);
            TestDomainEnforce(e, "alice", "domain1", "data1", "write", true);
            TestDomainEnforce(e, "alice", "domain1", "data2", "read", false);
            TestDomainEnforce(e, "alice", "domain1", "data2", "write", false);
            TestDomainEnforce(e, "bob", "domain2", "data1", "read", false);
            TestDomainEnforce(e, "bob", "domain2", "data1", "write", false);
            TestDomainEnforce(e, "bob", "domain2", "data2", "read", true);
            TestDomainEnforce(e, "bob", "domain2", "data2", "write", true);

            // Remove all policy rules related to domain1 and data1.
            await e.RemoveFilteredPolicyAsync(1, "domain1", "data1");

            TestDomainEnforce(e, "alice", "domain1", "data1", "read", false);
            TestDomainEnforce(e, "alice", "domain1", "data1", "write", false);
            TestDomainEnforce(e, "alice", "domain1", "data2", "read", false);
            TestDomainEnforce(e, "alice", "domain1", "data2", "write", false);
            TestDomainEnforce(e, "bob", "domain2", "data1", "read", false);
            TestDomainEnforce(e, "bob", "domain2", "data1", "write", false);
            TestDomainEnforce(e, "bob", "domain2", "data2", "read", true);
            TestDomainEnforce(e, "bob", "domain2", "data2", "write", true);

            // Remove the specified policy rule.
            await e.RemovePolicyAsync("admin", "domain2", "data2", "read");

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
        public void TestRbacModelWithDeny()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacWithDenyTestModel());
            e.BuildRoleLinks();

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
        public void TestRbacModelWithOnlyDeny()
        {
            var e = new Enforcer(TestModelFixture.GetNewTestModel(
                _testModelFixture._rbacWithNotDenyModelText,
                _testModelFixture._rbacWithDenyPolicyText));
            e.BuildRoleLinks();

            TestEnforce(e, "alice", "data2", "write", false);
        }

        [Fact]
        public void TestRbacModelWithCustomData()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.BuildRoleLinks();

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
        public async Task TestRbacModelWithCustomDataAsync()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.BuildRoleLinks();

            // You can add custom data to a grouping policy, Casbin will ignore it. It is only meaningful to the caller.
            // This feature can be used to store information like whether "bob" is an end user (so no subject will inherit "bob")
            // For Casbin, it is equivalent to: e.addGroupingPolicy("bob", "data2_admin")
            await e.AddGroupingPolicyAsync("bob", "data2_admin", "custom_data");

            await TestEnforceAsync(e, "alice", "data1", "read", true);
            await TestEnforceAsync(e, "alice", "data1", "write", false);
            await TestEnforceAsync(e, "alice", "data2", "read", true);
            await TestEnforceAsync(e, "alice", "data2", "write", true);
            await TestEnforceAsync(e, "bob", "data1", "read", false);
            await TestEnforceAsync(e, "bob", "data1", "write", false);
            await TestEnforceAsync(e, "bob", "data2", "read", true);
            await TestEnforceAsync(e, "bob", "data2", "write", true);

            // You should also take the custom data as a parameter when deleting a grouping policy.
            // e.removeGroupingPolicy("bob", "data2_admin") won't work.
            // Or you can remove it by using removeFilteredGroupingPolicy().
            await e.RemoveGroupingPolicyAsync("bob", "data2_admin", "custom_data");

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
        public void TestRbacModelWithCustomRoleManager()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.SetRoleManager(new CustomRoleManager());
            e.BuildRoleLinks();

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
        public void TestAbacModel()
        {
            var e = new Enforcer(_testModelFixture.GetNewAbacModel());

            var data1 = new TestResource("data1", "alice");
            var data2 = new TestResource("data2", "bob");

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
        public void TestAbacWithEvalModel()
        {
            var e = new Enforcer(_testModelFixture.GetNewAbacWithEvalModel());
            var subject1 = new TestSubject("alice", 16);
            var subject2 = new TestSubject("alice", 20);
            var subject3 = new TestSubject("alice", 65);

            TestEnforce(e, subject1, "/data1", "read", false);
            TestEnforce(e, subject1, "/data2", "read", false);
            TestEnforce(e, subject1, "/data1", "write", false);
            TestEnforce(e, subject1, "/data2", "write", true);

            TestEnforce(e, subject2, "/data1", "read", true);
            TestEnforce(e, subject2, "/data2", "read", false);
            TestEnforce(e, subject2, "/data1", "write", false);
            TestEnforce(e, subject2, "/data2", "write", true);

            TestEnforce(e, subject3, "/data1", "read", true);
            TestEnforce(e, subject3, "/data2", "read", false);
            TestEnforce(e, subject3, "/data1", "write", false);
            TestEnforce(e, subject3, "/data2", "write", false);
        }

        [Fact]
        public void TestKeyMatchModel()
        {
            var e = new Enforcer(_testModelFixture.GetNewKeyMatchTestModel());

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
        public void TestPriorityModelIndeterminate()
        {
            var e = new Enforcer(TestModelFixture.GetNewTestModel(
                _testModelFixture._priorityModelText,
                _testModelFixture._priorityIndeterminatePolicyText));
            e.BuildRoleLinks();

            TestEnforce(e, "alice", "data1", "read", false);
        }

        [Fact]
        public void TestPriorityModel()
        {
            var e = new Enforcer(_testModelFixture.GetNewPriorityTestModel());
            e.BuildRoleLinks();

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
        public void TestKeyMatch2Model()
        {
            var e = new Enforcer(_testModelFixture.GetNewKeyMatch2TestModel());

            TestEnforce(e, "alice", "/alice_data", "GET", false);
            TestEnforce(e, "alice", "/alice_data/resource1", "GET", true);
            TestEnforce(e, "alice", "/alice_data2/myid", "GET", false);
            TestEnforce(e, "alice", "/alice_data2/myid/using/res_id", "GET", true);
        }


        public class CustomRoleManager : IRoleManager
        {
            public void Clear()
            {
            }

            public void AddLink(string name1, string name2, params string[] domain)
            {
            }

            public void DeleteLink(string name1, string name2, params string[] domain)
            {
            }

            public bool HasLink(string name1, string name2, params string[] domain)
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

            public List<string> GetRoles(string name, params string[] domain) => null;
            public List<string> GetUsers(string name, params string[] domain) => null;
        }

        public class TestResource
        {
            public TestResource(string name, string owner)
            {
                Name = name;
                Owner = owner;
            }

            public string Name { get; }

            public string Owner { get; }
        }

        public class TestSubject
        {
            public TestSubject(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; }

            public int Age { get; }
        }
    }
}
