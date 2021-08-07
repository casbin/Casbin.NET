using System.Collections.Generic;
using NetCasbin.Extensions;
using NetCasbin.Rbac;
using NetCasbin.Util;
using Xunit;
using static NetCasbin.UnitTest.Util.TestUtil;

namespace NetCasbin.UnitTest
{
    public class RoleManagerTest
    {
        [Fact]
        public void TestRoleApi()
        {
            DefaultRoleManager roleManager = new(3);
            roleManager.AddLink("u1", "g1");
            roleManager.AddLink("u2", "g1");
            roleManager.AddLink("u3", "g2");
            roleManager.AddLink("u4", "g2");
            roleManager.AddLink("u4", "g3");
            roleManager.AddLink("g1", "g3");

            // Current role inheritance tree:
            //             g3    g2
            //            /  \  /  \
            //          g1    u4    u3
            //         /  \
            //       u1    u2

            TestRole(roleManager, "u1", "g1", true);
            TestRole(roleManager, "u1", "g2", false);
            TestRole(roleManager, "u1", "g3", true);
            TestRole(roleManager, "u2", "g1", true);
            TestRole(roleManager, "u2", "g2", false);
            TestRole(roleManager, "u2", "g3", true);
            TestRole(roleManager, "u3", "g1", false);
            TestRole(roleManager, "u3", "g2", true);
            TestRole(roleManager, "u3", "g3", false);
            TestRole(roleManager, "u4", "g1", false);
            TestRole(roleManager, "u4", "g2", true);
            TestRole(roleManager, "u4", "g3", true);

            TestGetRoles(roleManager, "u1", new List<string> { "g1" });
            TestGetRoles(roleManager, "u2", new List<string> { "g1" });
            TestGetRoles(roleManager, "u3", new List<string> { "g2" });
            TestGetRoles(roleManager, "u4", new List<string> { "g2", "g3" });
            TestGetRoles(roleManager, "g1", new List<string> { "g3" });
            TestGetRoles(roleManager, "g2", new List<string> { });
            TestGetRoles(roleManager, "g3", new List<string> { });

            roleManager.DeleteLink("g1", "g3");
            roleManager.DeleteLink("u4", "g2");

            // Current role inheritance tree after deleting the links:
            //             g3    g2
            //               \     \
            //          g1    u4    u3
            //         /  \
            //       u1    u2

            TestRole(roleManager, "u1", "g1", true);
            TestRole(roleManager, "u1", "g2", false);
            TestRole(roleManager, "u1", "g3", false);
            TestRole(roleManager, "u2", "g1", true);
            TestRole(roleManager, "u2", "g2", false);
            TestRole(roleManager, "u2", "g3", false);
            TestRole(roleManager, "u3", "g1", false);
            TestRole(roleManager, "u3", "g2", true);
            TestRole(roleManager, "u3", "g3", false);
            TestRole(roleManager, "u4", "g1", false);
            TestRole(roleManager, "u4", "g2", false);
            TestRole(roleManager, "u4", "g3", true);

            TestGetRoles(roleManager, "u1", new List<string> { "g1" });
            TestGetRoles(roleManager, "u2", new List<string> { "g1" });
            TestGetRoles(roleManager, "u3", new List<string> { "g2" });
            TestGetRoles(roleManager, "u4", new List<string> { "g3" });
            TestGetRoles(roleManager, "g1", new List<string>());
            TestGetRoles(roleManager, "g2", new List<string>());
            TestGetRoles(roleManager, "g3", new List<string>());
        }

        [Fact]
        public void TestDomainRoleApi()
        {
            DefaultRoleManager roleManager = new(3);
            roleManager.AddLink("u1", "g1", "domain1");
            roleManager.AddLink("u2", "g1", "domain1");
            roleManager.AddLink("u3", "admin", "domain2");
            roleManager.AddLink("u4", "admin", "domain2");
            roleManager.AddLink("u4", "admin", "domain1");
            roleManager.AddLink("g1", "admin", "domain1");

            // Current role inheritance tree:
            //       domain1:admin    domain2:admin
            //            /       \  /       \
            //      domain1:g1     u4         u3
            //         /  \
            //       u1    u2

            TestDomainRole(roleManager, "u1", "g1", "domain1", true);
            TestDomainRole(roleManager, "u1", "g1", "domain2", false);
            TestDomainRole(roleManager, "u1", "admin", "domain1", true);
            TestDomainRole(roleManager, "u1", "admin", "domain2", false);

            TestDomainRole(roleManager, "u2", "g1", "domain1", true);
            TestDomainRole(roleManager, "u2", "g1", "domain2", false);
            TestDomainRole(roleManager, "u2", "admin", "domain1", true);
            TestDomainRole(roleManager, "u2", "admin", "domain2", false);

            TestDomainRole(roleManager, "u3", "g1", "domain1", false);
            TestDomainRole(roleManager, "u3", "g1", "domain2", false);
            TestDomainRole(roleManager, "u3", "admin", "domain1", false);
            TestDomainRole(roleManager, "u3", "admin", "domain2", true);

            TestDomainRole(roleManager, "u4", "g1", "domain1", false);
            TestDomainRole(roleManager, "u4", "g1", "domain2", false);
            TestDomainRole(roleManager, "u4", "admin", "domain1", true);
            TestDomainRole(roleManager, "u4", "admin", "domain2", true);

            roleManager.DeleteLink("g1", "admin", "domain1");
            roleManager.DeleteLink("u4", "admin", "domain2");

            // Current role inheritance tree after deleting the links:
            //       domain1:admin    domain2:admin
            //                    \          \
            //      domain1:g1     u4         u3
            //         /  \
            //       u1    u2

            TestDomainRole(roleManager, "u1", "g1", "domain1", true);
            TestDomainRole(roleManager, "u1", "g1", "domain2", false);
            TestDomainRole(roleManager, "u1", "admin", "domain1", false);
            TestDomainRole(roleManager, "u1", "admin", "domain2", false);

            TestDomainRole(roleManager, "u2", "g1", "domain1", true);
            TestDomainRole(roleManager, "u2", "g1", "domain2", false);
            TestDomainRole(roleManager, "u2", "admin", "domain1", false);
            TestDomainRole(roleManager, "u2", "admin", "domain2", false);

            TestDomainRole(roleManager, "u3", "g1", "domain1", false);
            TestDomainRole(roleManager, "u3", "g1", "domain2", false);
            TestDomainRole(roleManager, "u3", "admin", "domain1", false);
            TestDomainRole(roleManager, "u3", "admin", "domain2", true);

            TestDomainRole(roleManager, "u4", "g1", "domain1", false);
            TestDomainRole(roleManager, "u4", "g1", "domain2", false);
            TestDomainRole(roleManager, "u4", "admin", "domain1", true);
            TestDomainRole(roleManager, "u4", "admin", "domain2", false);
        }

        [Fact]
        public void TestClearApi()
        {
            DefaultRoleManager roleManager = new(3);
            roleManager.AddLink("u1", "g1");
            roleManager.AddLink("u2", "g1");
            roleManager.AddLink("u3", "g2");
            roleManager.AddLink("u4", "g2");
            roleManager.AddLink("u4", "g3");
            roleManager.AddLink("g1", "g3");

            // Current role inheritance tree:
            //             g3    g2
            //            /  \  /  \
            //          g1    u4    u3
            //         /  \
            //       u1    u2

            roleManager.Clear();

            // All data is cleared.
            // No role inheritance now.

            TestRole(roleManager, "u1", "g1", false);
            TestRole(roleManager, "u1", "g2", false);
            TestRole(roleManager, "u1", "g3", false);
            TestRole(roleManager, "u2", "g1", false);
            TestRole(roleManager, "u2", "g2", false);
            TestRole(roleManager, "u2", "g3", false);
            TestRole(roleManager, "u3", "g1", false);
            TestRole(roleManager, "u3", "g2", false);
            TestRole(roleManager, "u3", "g3", false);
            TestRole(roleManager, "u4", "g1", false);
            TestRole(roleManager, "u4", "g2", false);
            TestRole(roleManager, "u4", "g3", false);
        }

        [Fact]
        public void TestDomainPatternRoleApi()
        {
            DefaultRoleManager roleManager = new(10);

            roleManager.AddDomainMatchingFunc(BuiltInFunctions.KeyMatch2);

            roleManager.AddLink("u1", "g1", "domain1");
            roleManager.AddLink("u2", "g1", "domain2");
            roleManager.AddLink("u3", "g1", "*");
            roleManager.AddLink("u4", "g2", "domain3");

            // Current role inheritance tree after deleting the links:
            //       domain1:g1    domain2:g1			domain3:g2
            //		   /      \    /      \					|
            //	 domain1:u1    *:g1     domain2:u2		domain3:u4
            // 					|
            // 				   *:u3

            TestDomainRole(roleManager, "u1", "g1", "domain1", true);
            TestDomainRole(roleManager, "u2", "g1", "domain1", false);
            TestDomainRole(roleManager, "u2", "g1", "domain2", true);
            TestDomainRole(roleManager, "u3", "g1", "domain1", true);
            TestDomainRole(roleManager, "u3", "g1", "domain2", true);
            TestDomainRole(roleManager, "u1", "g2", "domain1", false);
            TestDomainRole(roleManager, "u4", "g2", "domain3", true);
            TestDomainRole(roleManager, "u3", "g2", "domain3", false);

            TestGetRolesWithDomain(roleManager, "u3", "domain1", new List<string>{ "g1"});
            TestGetRolesWithDomain(roleManager, "u1", "domain1", new List<string>{ "g1"});
            TestGetRolesWithDomain(roleManager, "u3", "domain2", new List<string>{ "g1"});
            TestGetRolesWithDomain(roleManager, "u1", "domain2", new List<string>());
            TestGetRolesWithDomain(roleManager, "u4", "domain3", new List<string>{ "g2"});
        }

        [Fact]
        public void TestAllMatchingFuncApi()
        {
            DefaultRoleManager roleManager = new(10);

            roleManager.AddMatchingFunc(BuiltInFunctions.KeyMatch2);
            roleManager.AddDomainMatchingFunc(BuiltInFunctions.KeyMatch2);
            roleManager.AddLink("/book/:id", "book_group", "*");

            // Current role inheritance tree after deleting the links:
            //  		*:book_group
            //				|
            // 			*:/book/:id

            TestDomainRole(roleManager, "/book/1", "book_group", "domain1", true);
            TestDomainRole(roleManager, "/book/2", "book_group", "domain1", true);
        }


    }
}
