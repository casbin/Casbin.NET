using Casbin.Extensions;
using Casbin.Rbac;
using Xunit;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.RbacTests
{
    public class GroupRoleManagerTest
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
