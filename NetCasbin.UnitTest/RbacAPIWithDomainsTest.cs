using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NetCasbin.Test
{
    public class RbacAPIWithDomainsTest : TestUtil
    {
        [Fact]
        public void Test_RoleAPIWithDomains()
        {
            Enforcer e = new Enforcer("examples/rbac_with_domains_model.conf", "examples/rbac_with_domains_policy.csv");

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
    }
}
