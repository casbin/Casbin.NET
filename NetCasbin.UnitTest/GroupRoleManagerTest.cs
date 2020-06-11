using NetCasbin.Rbac;
using Xunit;

namespace NetCasbin.Test
{
    public class GroupRoleManagerTest:TestUtil
    {
        [Fact]
        public void TestGroupRoleManager()
        {
            var e = new Enforcer("examples/group_with_domain_model.conf", "examples/group_with_domain_policy.csv");
            e.SetRoleManager(new GroupRoleManager(10));
            e.BuildRoleLinks();

            TestDomainEnforce(e, "alice", "domain1", "data1", "read", true);
        }
    }
}
