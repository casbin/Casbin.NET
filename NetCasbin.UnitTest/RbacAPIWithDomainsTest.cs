using System.Threading.Tasks;
using NetCasbin.UnitTest.Fixtures;
using Xunit;
using static NetCasbin.UnitTest.Util.TestUtil;

namespace NetCasbin.UnitTest
{
    [Collection("Model collection")]
    public class RbacApiWithDomainsTest
    {
        private readonly TestModelFixture _testModelFixture;

        public RbacApiWithDomainsTest(TestModelFixture testModelFixture)
        {
            _testModelFixture = testModelFixture;
        }

        [Fact]
        public void TestRoleApiWithDomains()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacWithDomainsTestModel());
            e.BuildRoleLinks();

            TestGetRolesInDomain(e, "alice", "domain1", AsList("admin"));
            TestGetRolesInDomain(e, "bob", "domain1", AsList());
            TestGetRolesInDomain(e, "admin", "domain1", AsList());
            TestGetRolesInDomain(e, "non_exist", "domain1", AsList());

            TestGetRolesInDomain(e, "alice", "domain2", AsList());
            TestGetRolesInDomain(e, "bob", "domain2", AsList("admin"));
            TestGetRolesInDomain(e, "admin", "domain2", AsList());
            TestGetRolesInDomain(e, "non_exist", "domain2", AsList());

            e.DeleteRoleForUserInDomain("alice", "admin", "domain1");
            e.AddRoleForUserInDomain("bob", "admin", "domain1");

            TestGetRolesInDomain(e, "alice", "domain1", AsList());
            TestGetRolesInDomain(e, "bob", "domain1", AsList("admin"));
            TestGetRolesInDomain(e, "admin", "domain1", AsList());
            TestGetRolesInDomain(e, "non_exist", "domain1", AsList());

            TestGetRolesInDomain(e, "alice", "domain2", AsList());
            TestGetRolesInDomain(e, "bob", "domain2", AsList("admin"));
            TestGetRolesInDomain(e, "admin", "domain2", AsList());
            TestGetRolesInDomain(e, "non_exist", "domain2", AsList());
        }

        [Fact]
        public async Task TestRoleApiWithDomainsAsync()
        {
            var e = new Enforcer(_testModelFixture.GetNewRbacWithDomainsTestModel());
            e.BuildRoleLinks();

            TestGetRolesInDomain(e, "alice", "domain1", AsList("admin"));
            TestGetRolesInDomain(e, "bob", "domain1", AsList());
            TestGetRolesInDomain(e, "admin", "domain1", AsList());
            TestGetRolesInDomain(e, "non_exist", "domain1", AsList());

            TestGetRolesInDomain(e, "alice", "domain2", AsList());
            TestGetRolesInDomain(e, "bob", "domain2", AsList("admin"));
            TestGetRolesInDomain(e, "admin", "domain2", AsList());
            TestGetRolesInDomain(e, "non_exist", "domain2", AsList());

            await e.DeleteRoleForUserInDomainAsync("alice", "admin", "domain1");
            await e.AddRoleForUserInDomainAsync("bob", "admin", "domain1");

            TestGetRolesInDomain(e, "alice", "domain1", AsList());
            TestGetRolesInDomain(e, "bob", "domain1", AsList("admin"));
            TestGetRolesInDomain(e, "admin", "domain1", AsList());
            TestGetRolesInDomain(e, "non_exist", "domain1", AsList());

            TestGetRolesInDomain(e, "alice", "domain2", AsList());
            TestGetRolesInDomain(e, "bob", "domain2", AsList("admin"));
            TestGetRolesInDomain(e, "admin", "domain2", AsList());
            TestGetRolesInDomain(e, "non_exist", "domain2", AsList());
        }
    }
}
