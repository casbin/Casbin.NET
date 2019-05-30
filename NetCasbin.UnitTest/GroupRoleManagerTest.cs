using NetCasbin.Rabc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NetCasbin.Test
{
    public class GroupRoleManagerTest:TestUtil
    {
        [Fact]
        public void testGroupRoleManager()
        {
            Enforcer e = new Enforcer("examples/group_with_domain_model.conf", "examples/group_with_domain_policy.csv");
            e.SetRoleManager(new GroupRoleManager(10));
            e.BuildRoleLinks();

            testDomainEnforce(e, "alice", "domain1", "data1", "read", true);
        }
    }
}
