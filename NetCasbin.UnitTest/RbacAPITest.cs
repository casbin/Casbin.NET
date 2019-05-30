using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace NetCasbin.Test
{
    public class RbacAPITest : TestUtil
    {

        [Fact]
        public void Test_RoleAPI()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");
            testGetRoles(e, "alice", asList("data2_admin"));
            testGetRoles(e, "bob", asList());
            testGetRoles(e, "data2_admin", asList());
            testGetRoles(e, "non_exist", asList());

            testHasRole(e, "alice", "data1_admin", false);
            testHasRole(e, "alice", "data2_admin", true);

            e.AddRoleForUser("alice", "data1_admin");

            testGetRoles(e, "alice", asList("data1_admin", "data2_admin"));
            testGetRoles(e, "bob", asList());
            testGetRoles(e, "data2_admin", asList());

            e.DeleteRoleForUser("alice", "data1_admin");

            testGetRoles(e, "alice", asList("data2_admin"));
            testGetRoles(e, "bob", asList());
            testGetRoles(e, "data2_admin", asList());

            e.DeleteRolesForUser("alice");

            testGetRoles(e, "alice", asList());
            testGetRoles(e, "bob", asList());
            testGetRoles(e, "data2_admin", asList());

            e.AddRoleForUser("alice", "data1_admin");
            e.DeleteUser("alice");

            testGetRoles(e, "alice", asList());
            testGetRoles(e, "bob", asList());
            testGetRoles(e, "data2_admin", asList());

            e.AddRoleForUser("alice", "data2_admin");

            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);

            testEnforce(e, "alice", "data2", "read", true);
            testEnforce(e, "alice", "data2", "write", true);

            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", true);

            e.DeleteRole("data2_admin");

            testEnforce(e, "alice", "data1", "read", true);
            testEnforce(e, "alice", "data1", "write", false);
            testEnforce(e, "alice", "data2", "read", false);
            testEnforce(e, "alice", "data2", "write", false);
            testEnforce(e, "bob", "data1", "read", false);
            testEnforce(e, "bob", "data1", "write", false);
            testEnforce(e, "bob", "data2", "read", false);
            testEnforce(e, "bob", "data2", "write", true);

        }
    }
}
