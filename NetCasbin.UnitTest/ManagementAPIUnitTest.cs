using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using NetCasbin.UnitTest.Fixtures;
using static NetCasbin.UnitTest.Util.TestUtil;

namespace NetCasbin.UnitTest
{
    public class ManagementApiUnitTest : IClassFixture<TestModelFixture>
    {
        private readonly TestModelFixture _testModelFixture;

        public ManagementApiUnitTest(TestModelFixture testModelFixture)
        {
            _testModelFixture = testModelFixture;
        }

        [Fact]
        public void TestGetPolicyApi()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.BuildRoleLinks();

            TestGetPolicy(e, AsList(
                    AsList("alice", "data1", "read"),
                    AsList("bob", "data2", "write"),
                    AsList("data2_admin", "data2", "read"),
                    AsList("data2_admin", "data2", "write")));

            TestGetFilteredPolicy(e, 0, AsList(AsList("alice", "data1", "read")), "alice");
            TestGetFilteredPolicy(e, 0, AsList(AsList("bob", "data2", "write")), "bob");
            TestGetFilteredPolicy(e, 0, AsList(AsList("data2_admin", "data2", "read"), AsList("data2_admin", "data2", "write")), "data2_admin");
            TestGetFilteredPolicy(e, 1, AsList(AsList("alice", "data1", "read")), "data1");
            TestGetFilteredPolicy(e, 1, AsList(AsList("bob", "data2", "write"), AsList("data2_admin", "data2", "read"), AsList("data2_admin", "data2", "write")), "data2");
            TestGetFilteredPolicy(e, 2, AsList(AsList("alice", "data1", "read"), AsList("data2_admin", "data2", "read")), "read");
            TestGetFilteredPolicy(e, 2, AsList(AsList("bob", "data2", "write"), AsList("data2_admin", "data2", "write")), "write");

            TestGetFilteredPolicy(e, 0, AsList(AsList("data2_admin", "data2", "read"), AsList("data2_admin", "data2", "write")), "data2_admin", "data2");
            // Note: "" (empty string) in fieldValues means matching all values.
            TestGetFilteredPolicy(e, 0, AsList(AsList("data2_admin", "data2", "read")), "data2_admin", "", "read");
            TestGetFilteredPolicy(e, 1, AsList(AsList("bob", "data2", "write"), AsList("data2_admin", "data2", "write")), "data2", "write");

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
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
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

            var namedPolicy = AsList("eve", "data3", "read");
            e.RemoveNamedPolicy("p", namedPolicy);
            e.AddNamedPolicy("p", namedPolicy);

            TestGetPolicy(e, AsList(
                    AsList("data2_admin", "data2", "read"),
                    AsList("data2_admin", "data2", "write"),
                    AsList("eve", "data3", "read")));

            e.RemoveFilteredPolicy(1, "data2");

            TestGetPolicy(e, AsList(AsList("eve", "data3", "read")));
        }

        [Fact]
        public async Task TestModifyPolicyAsync()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
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

            var namedPolicy = AsList("eve", "data3", "read");
            await e.RemoveNamedPolicyAsync("p", namedPolicy);
            await e.AddNamedPolicyAsync("p", namedPolicy);

            TestGetPolicy(e, AsList(
                AsList("data2_admin", "data2", "read"),
                AsList("data2_admin", "data2", "write"),
                AsList("eve", "data3", "read")));

            await e.RemoveFilteredPolicyAsync(1, "data2");

            TestGetPolicy(e, AsList(AsList("eve", "data3", "read")));
        }

        [Fact]
        public void TestModifyGroupingPolicy()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.BuildRoleLinks();

            TestGetRoles(e, "alice", AsList("data2_admin"));
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "eve", AsList());
            TestGetRoles(e, "non_exist", AsList());

            e.RemoveGroupingPolicy("alice", "data2_admin");
            e.AddGroupingPolicy("bob", "data1_admin");
            e.AddGroupingPolicy("eve", "data3_admin");

            var namedGroupingPolicy = AsList("alice", "data2_admin");
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
        }

        [Fact]
        public async Task TestModifyGroupingPolicyAsync()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.BuildRoleLinks();

            TestGetRoles(e, "alice", AsList("data2_admin"));
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "eve", AsList());
            TestGetRoles(e, "non_exist", AsList());

            await e.RemoveGroupingPolicyAsync("alice", "data2_admin");
            await e.AddGroupingPolicyAsync("bob", "data1_admin");
            await e.AddGroupingPolicyAsync("eve", "data3_admin");

            var namedGroupingPolicy = AsList("alice", "data2_admin");
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
        }
    }
}
