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

            testGetRolesInDomain(e, "alice", "domain1", asList("admin"));
            testGetRolesInDomain(e, "bob", "domain1", asList());
            testGetRolesInDomain(e, "admin", "domain1", asList());
            testGetRolesInDomain(e, "non_exist", "domain1", asList());

            testGetRolesInDomain(e, "alice", "domain2", asList());
            testGetRolesInDomain(e, "bob", "domain2", asList("admin"));
            testGetRolesInDomain(e, "admin", "domain2", asList());
            testGetRolesInDomain(e, "non_exist", "domain2", asList());

            e.DeleteRoleForUserInDomain("alice", "admin", "domain1");
            e.AddRoleForUserInDomain("bob", "admin", "domain1");

            testGetRolesInDomain(e, "alice", "domain1", asList());
            testGetRolesInDomain(e, "bob", "domain1", asList("admin"));
            testGetRolesInDomain(e, "admin", "domain1", asList());
            testGetRolesInDomain(e, "non_exist", "domain1", asList());

            testGetRolesInDomain(e, "alice", "domain2", asList());
            testGetRolesInDomain(e, "bob", "domain2", asList("admin"));
            testGetRolesInDomain(e, "admin", "domain2", asList());
            testGetRolesInDomain(e, "non_exist", "domain2", asList());
        }
    }
}
