using System.Threading.Tasks;
using Xunit;

namespace NetCasbin.UnitTest
{
    public class RbacApiWithDomainsTest : TestUtil
    {
        [Fact]
        public void TestRoleApiWithDomains()
        {
            var e = new Enforcer("examples/rbac_with_domains_model.conf", "examples/rbac_with_domains_policy.csv");

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
            var e = new Enforcer("examples/rbac_with_domains_model.conf", "examples/rbac_with_domains_policy.csv");

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
