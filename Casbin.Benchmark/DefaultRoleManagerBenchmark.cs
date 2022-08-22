using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using static Casbin.Benchmark.TestHelper;

namespace Casbin.Benchmark
{
    [MemoryDiagnoser]
    [BenchmarkCategory("Enforcer")]
    [SimpleJob(RunStrategy.Throughput, targetCount: 10, runtimeMoniker: RuntimeMoniker.Net48)]
    [SimpleJob(RunStrategy.Throughput, targetCount: 10, runtimeMoniker: RuntimeMoniker.NetCoreApp31, baseline: true)]
    [SimpleJob(RunStrategy.Throughput, targetCount: 10, runtimeMoniker: RuntimeMoniker.Net50)]
    [SimpleJob(RunStrategy.Throughput, targetCount: 10, runtimeMoniker: RuntimeMoniker.Net60)]
    public class DefaultRoleManagerBenchmark
    {
        private Enforcer NowEnforcer { get; set; }
        private string NowTestUserName { get; set; }
        private string NowTestRole1Name { get; set; }
        private string NowTestRole2Name { get; set; }
        private TestResource NowTestResource { get; set; }
        private int[][] RbacScale { get; } =
        {
            new[] {100, 1000},    // Small
            new[] {1000, 10000},  // Medium
            new[] {10000, 100000} // Large
        };

        #region GlobalSetup

        [GlobalSetup(Targets = new[] {
            nameof(RbacModelGetRoles), 
            nameof(RbacModelGetUsers), 
            nameof(RbacModelGetDomains), 
            nameof(RbacModelHasLink), 
            nameof(RbacModelAddLink), 
            nameof(RbacModelDeleteLink)
        })]
        public void GlobalSetupForRbacModel()
        {
            GlobalSetupFromFile("rbac_model.conf", "rbac_policy.csv");
            Console.WriteLine("// Set the Rbac Model enforcer");
        }

        [GlobalSetup(Targets = new[] {nameof(RbacModelWithSmallScaleRoleTest)})]
        public void GlobalSetupForRbacModelWithSmallScale()
        {
            int groupCount = RbacScale[0][0];
            int userCount = RbacScale[0][1];
            GlobalSetupForRbacModelWithScale(groupCount, userCount);
            Console.WriteLine($"// Set the Rbac Model with small scale ({groupCount} groups and {userCount} users) enforcer.");
        }

        [GlobalSetup(Targets = new[] {nameof(RbacModelWithMiddleScaleRoleTest)})]
        public void GlobalSetupForRbacModelWithMediumScale()
        {
            int groupCount = RbacScale[1][0];
            int userCount = RbacScale[1][1];
            GlobalSetupForRbacModelWithScale(groupCount, userCount);
            Console.WriteLine($"// Set the Rbac Model with medium scale ({groupCount} groups and {userCount} users) enforcer.");
        }

        [GlobalSetup(Targets = new[] { nameof(RbacModelWithLargeScaleRoleTest) })]
        public void GlobalSetupForRbacModelWithLargeScale()
        {
            int groupCount = RbacScale[2][0];
            int userCount = RbacScale[2][1];
            GlobalSetupForRbacModelWithScale(groupCount, userCount);
            Console.WriteLine($"// Set the RBAC with large scale ({groupCount} groups and {userCount} users) enforcer.");
        }

        [GlobalSetup(Targets = new[] {
            nameof(RbacModelWithDomainGetRoles), 
            nameof(RbacModelWithDomainGetUsers), 
            nameof(RbacModelWithDomainGetDomains), 
            nameof(RbacModelWithDomainHasLink), 
            nameof(RbacModelWithDomainAddLink), 
            nameof(RbacModelWithDomainDeleteLink)
        })]
        public void GlobalSetupForRbacModelWithDomains()
        {
            GlobalSetupFromFile("rbac_with_domains_model.conf", "rbac_with_domains_policy.csv");
            Console.WriteLine("// Set the Rbac Model With Domains enforcer");
        }

        #endregion

        #region private help method
        /// <summary>
        /// Build the dependencies of groups and users without cycle.
        /// </summary>
        /// <param name="groupCount"></param>
        /// <returns></returns>
        private List<List<string>> BuildGroupDependencies(int groupCount)
        {
            List<List<string>> res = new List<List<string>>();
            int[] disjointSet = new int[groupCount];
            // Init the Disjoint-set
            for (int i = 0; i < groupCount; i++)
            {
                disjointSet[i] = i;
            }

            // Build dependencies between groups
            var rd = new Random();
            for (int i = 0; i < groupCount; i++)
            {
                int father = rd.Next(groupCount);
                if(findRootInDisjointSet(disjointSet, i) != findRootInDisjointSet(disjointSet, father))
                {
                    disjointSet[findRootInDisjointSet(disjointSet, i)] = findRootInDisjointSet(disjointSet, father);
                    res.Add(new[] { $"group{i}", $"group{father}" }.ToList());
                }
            }
            return res;
        }
        /// <summary>
        /// Find the root in Disjoint-set
        /// </summary>
        /// <param name="disjointSet"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private int findRootInDisjointSet(int[] disjointSet, int x)
        {
            if(disjointSet[x] != x)
            {
                disjointSet[x] = findRootInDisjointSet(disjointSet, disjointSet[x]);
            }
            return disjointSet[x];
        }
        private void GlobalSetupForRbacModelWithScale(int groupCount, int userCount)
        {
            GlobalSetupForRbacModel();
            var policyList = new List<List<string>>();
            for (int i = 0; i < groupCount; i++)
            {
                policyList.Add( new[] {$"group{i}", $"data{i / 10}", "read"}.ToList());
            }
            NowEnforcer.AddPolicies(policyList);

            policyList.Clear();
            for (int i = 0; i < userCount; i++)
            {
                policyList.Add( new[] {$"user{i}", $"group{i / 10}"}.ToList());
            }
            NowEnforcer.EnableAutoBuildRoleLinks(false);
            NowEnforcer.AddGroupingPolicies(policyList);
            NowEnforcer.AddGroupingPolicies(BuildGroupDependencies(groupCount));
            NowEnforcer.BuildRoleLinks();

            NowTestUserName = $"user{userCount / 2 + 1}"; // if 1000 => 501...
            NowTestRole1Name = $"group{groupCount / 2 + 1}"; // if 1000 => 501...
            NowTestRole2Name = $"group{groupCount / 10 - 1}"; // if 100 => 9...
            Console.WriteLine($"// Already set user name to {NowTestUserName}.");
            Console.WriteLine($"// Already set role1 name to {NowTestRole1Name}.");
            Console.WriteLine($"// Already set role1 name to {NowTestRole2Name}.");
        }

        private void GlobalSetupFromFile(string modelFileName, string policyFileName = null)
        {
            if (policyFileName is null)
            {
                NowEnforcer = new Enforcer(
                    GetTestFilePath(modelFileName));
                NowEnforcer.EnableCache(false);
                return;
            }

            NowEnforcer = new Enforcer(
                GetTestFilePath(modelFileName),
                GetTestFilePath(policyFileName));
            NowEnforcer.EnableCache(false);
        }
        
        #endregion

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            NowEnforcer = null;
            Console.WriteLine("// Cleaned the enforcer");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelGetRoles()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetRoles("alice");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelGetUsers()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetUsers("_admin");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelGetDomains()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetDomains("alice");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelHasLink()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.HasLink("alice", "bob");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelAddLink()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.AddLink("alice", "bob");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelDeleteLink()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.DeleteLink("alice", "data2_admin");
        }

       [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModelWithDomain")]
        public void RbacModelWithDomainGetRoles()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetRoles("alice", "domain1");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModelWithDomain")]
        public void RbacModelWithDomainGetUsers()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetUsers("admin", "domain1");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModelWithDomain")]
        public void RbacModelWithDomainGetDomains()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetDomains("alice");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModelWithDomain")]
        public void RbacModelWithDomainHasLink()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.HasLink("alice", "admin", "domain1");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModelWithDomain")]
        public void RbacModelWithDomainAddLink()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.AddLink("alice", "bob", "domain2");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModelWithDomain")]
        public void RbacModelWithDomainDeleteLink()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.DeleteLink("alice", "admin", "domain1");
        }
        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModelWithSmallScale")]
        public void RbacModelWithSmallScaleRoleTest()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetRoles(NowTestUserName);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetUsers(NowTestRole1Name);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetDomains(NowTestRole1Name);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.HasLink(NowTestRole1Name, NowTestRole2Name);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.AddLink(NowTestRole1Name, NowTestRole2Name);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.DeleteLink(NowTestRole1Name, NowTestRole2Name);
        }
        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModelWithMiddleScale")]
        public void RbacModelWithMiddleScaleRoleTest()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetRoles(NowTestUserName);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetUsers(NowTestRole1Name);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetDomains(NowTestRole1Name);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.HasLink(NowTestRole1Name, NowTestRole2Name);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.AddLink(NowTestRole1Name, NowTestRole2Name);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.DeleteLink(NowTestRole1Name, NowTestRole2Name);
        }
        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModelWithLargeScale")]
        public void RbacModelWithLargeScaleRoleTest()
        {
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetRoles(NowTestUserName);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetUsers(NowTestRole1Name);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.GetDomains(NowTestRole1Name);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.HasLink(NowTestRole1Name, NowTestRole2Name);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.AddLink(NowTestRole1Name, NowTestRole2Name);
            NowEnforcer.Model.Sections[PermConstants.Section.RoleSection][PermConstants.DefaultRoleType]
                    .RoleManager.DeleteLink(NowTestRole1Name, NowTestRole2Name);
        }
    }
}
