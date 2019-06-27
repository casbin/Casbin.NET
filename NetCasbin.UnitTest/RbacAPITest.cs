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
            TestGetRoles(e, "alice", AsList("data2_admin"));
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());
            TestGetRoles(e, "non_exist", AsList());

            TestHasRole(e, "alice", "data1_admin", false);
            TestHasRole(e, "alice", "data2_admin", true);

            e.AddRoleForUser("alice", "data1_admin");

            TestGetRoles(e, "alice", AsList("data1_admin", "data2_admin"));
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());

            e.DeleteRoleForUser("alice", "data1_admin");

            TestGetRoles(e, "alice", AsList("data2_admin"));
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());

            e.DeleteRolesForUser("alice");

            TestGetRoles(e, "alice", AsList());
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());

            e.AddRoleForUser("alice", "data1_admin");
            e.DeleteUser("alice");

            TestGetRoles(e, "alice", AsList());
            TestGetRoles(e, "bob", AsList());
            TestGetRoles(e, "data2_admin", AsList());

            e.AddRoleForUser("alice", "data2_admin");

            TestEnforce(e, "alice", "data1", "read", true);
            TestEnforce(e, "alice", "data1", "write", false);

            TestEnforce(e, "alice", "data2", "read", true);
            TestEnforce(e, "alice", "data2", "write", true);

            TestEnforce(e, "bob", "data1", "read", false);
            TestEnforce(e, "bob", "data1", "write", false);
            TestEnforce(e, "bob", "data2", "read", false);
            TestEnforce(e, "bob", "data2", "write", true);

            e.DeleteRole("data2_admin");

            TestEnforce(e, "alice", "data1", "read", true);
            TestEnforce(e, "alice", "data1", "write", false);
            TestEnforce(e, "alice", "data2", "read", false);
            TestEnforce(e, "alice", "data2", "write", false);
            TestEnforce(e, "bob", "data1", "read", false);
            TestEnforce(e, "bob", "data1", "write", false);
            TestEnforce(e, "bob", "data2", "read", false);
            TestEnforce(e, "bob", "data2", "write", true);

        }
    }
}
