using Casbin.Rbac;
using Xunit;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.RbacTests;

public class GroupRoleManagerTest
{
    [Fact]
    public void TestGroupRoleManager()
    {
        Enforcer e = new("Examples/group_with_domain_model.conf", "Examples/group_with_domain_policy.csv");
        GroupRoleManager roleManager = new(10);
        e.SetRoleManager("g", roleManager);
        e.SetRoleManager("g2", roleManager);
        TestDomainEnforce(e, "alice", "domain1", "data1", "read", true);
    }
}
