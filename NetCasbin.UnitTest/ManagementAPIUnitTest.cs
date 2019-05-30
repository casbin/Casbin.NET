using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NetCasbin.Test
{
    public class ManagementAPIUnitTest : TestUtil
    {
        [Fact]
        public void testGetPolicyAPI()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            testGetPolicy(e, asList(
                    asList("alice", "data1", "read"),
                    asList("bob", "data2", "write"),
                    asList("data2_admin", "data2", "read"),
                    asList("data2_admin", "data2", "write")));

            testGetFilteredPolicy(e, 0, asList(asList("alice", "data1", "read")), "alice");
            testGetFilteredPolicy(e, 0, asList(asList("bob", "data2", "write")), "bob");
            testGetFilteredPolicy(e, 0, asList(asList("data2_admin", "data2", "read"), asList("data2_admin", "data2", "write")), "data2_admin");
            testGetFilteredPolicy(e, 1, asList(asList("alice", "data1", "read")), "data1");
            testGetFilteredPolicy(e, 1, asList(asList("bob", "data2", "write"), asList("data2_admin", "data2", "read"), asList("data2_admin", "data2", "write")), "data2");
            testGetFilteredPolicy(e, 2, asList(asList("alice", "data1", "read"), asList("data2_admin", "data2", "read")), "read");
            testGetFilteredPolicy(e, 2, asList(asList("bob", "data2", "write"), asList("data2_admin", "data2", "write")), "write");

            testGetFilteredPolicy(e, 0, asList(asList("data2_admin", "data2", "read"), asList("data2_admin", "data2", "write")), "data2_admin", "data2");
            // Note: "" (empty string) in fieldValues means matching all values.
            testGetFilteredPolicy(e, 0, asList(asList("data2_admin", "data2", "read")), "data2_admin", "", "read");
            testGetFilteredPolicy(e, 1, asList(asList("bob", "data2", "write"), asList("data2_admin", "data2", "write")), "data2", "write");

            testHasPolicy(e, asList("alice", "data1", "read"), true);
            testHasPolicy(e, asList("bob", "data2", "write"), true);
            testHasPolicy(e, asList("alice", "data2", "read"), false);
            testHasPolicy(e, asList("bob", "data3", "write"), false);

            testGetGroupingPolicy(e, asList(asList("alice", "data2_admin")));

            testGetFilteredGroupingPolicy(e, 0, asList(asList("alice", "data2_admin")), "alice");
            testGetFilteredGroupingPolicy(e, 0, new List<List<string>>(), "bob");
            testGetFilteredGroupingPolicy(e, 1, new List<List<string>>(), "data1_admin");
            testGetFilteredGroupingPolicy(e, 1, asList(asList("alice", "data2_admin")), "data2_admin");
            // Note: "" (empty string) in fieldValues means matching all values.
            testGetFilteredGroupingPolicy(e, 0, asList(asList("alice", "data2_admin")), "", "data2_admin");

            testHasGroupingPolicy(e, asList("alice", "data2_admin"), true);
            testHasGroupingPolicy(e, asList("bob", "data2_admin"), false);
        }

        [Fact]
        public void testModifyPolicyAPI()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            testGetPolicy(e, asList(
                    asList("alice", "data1", "read"),
                    asList("bob", "data2", "write"),
                    asList("data2_admin", "data2", "read"),
                    asList("data2_admin", "data2", "write")));

            e.RemovePolicy("alice", "data1", "read");
            e.RemovePolicy("bob", "data2", "write");
            e.RemovePolicy("alice", "data1", "read");
            e.AddPolicy("eve", "data3", "read");
            e.AddPolicy("eve", "data3", "read");

            List<String> namedPolicy = asList("eve", "data3", "read");
            e.RemoveNamedPolicy("p", namedPolicy);
            e.AddNamedPolicy("p", namedPolicy);

            testGetPolicy(e, asList(
                    asList("data2_admin", "data2", "read"),
                    asList("data2_admin", "data2", "write"),
                    asList("eve", "data3", "read")));

            e.RemoveFilteredPolicy(1, "data2");

            testGetPolicy(e, asList(asList("eve", "data3", "read")));
        }

        [Fact]
        public void testModifyGroupingPolicyAPI()
        {
            Enforcer e = new Enforcer("examples/rbac_model.conf", "examples/rbac_policy.csv");

            testGetRoles(e, "alice", asList("data2_admin"));
            testGetRoles(e, "bob", asList());
            testGetRoles(e, "eve", asList());
            testGetRoles(e, "non_exist", asList());

            e.RemoveGroupingPolicy("alice", "data2_admin");
            e.AddGroupingPolicy("bob", "data1_admin");
            e.AddGroupingPolicy("eve", "data3_admin");

            List<String> namedGroupingPolicy = asList("alice", "data2_admin");
            testGetRoles(e, "alice", asList());
            e.AddNamedGroupingPolicy("g", namedGroupingPolicy);
            testGetRoles(e, "alice", asList("data2_admin"));
            e.RemoveNamedGroupingPolicy("g", namedGroupingPolicy);

            testGetRoles(e, "alice", asList());
            testGetRoles(e, "bob", asList("data1_admin"));
            testGetRoles(e, "eve", asList("data3_admin"));
            testGetRoles(e, "non_exist", asList());

            testGetUsers(e, "data1_admin", asList("bob"));
            testGetUsers(e, "data2_admin", asList());
            testGetUsers(e, "data3_admin", asList("eve"));

            e.RemoveFilteredGroupingPolicy(0, "bob");

            testGetRoles(e, "alice", asList());
            testGetRoles(e, "bob", asList());
            testGetRoles(e, "eve", asList("data3_admin"));
            testGetRoles(e, "non_exist", asList());

            testGetUsers(e, "data1_admin", asList());
            testGetUsers(e, "data2_admin", asList());
            testGetUsers(e, "data3_admin", asList("eve"));
        }
    }
}
