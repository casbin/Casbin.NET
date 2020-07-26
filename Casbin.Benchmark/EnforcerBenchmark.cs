using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using NetCasbin;
using static Casbin.Benchmark.TestHelper;

namespace Casbin.Benchmark
{
    [MemoryDiagnoser]
    [BenchmarkCategory("Enforcer")]
    [SimpleJob(RunStrategy.Throughput, targetCount: 10, runtimeMoniker: RuntimeMoniker.Net48)]
    [SimpleJob(RunStrategy.Throughput, targetCount: 10, runtimeMoniker: RuntimeMoniker.NetCoreApp31, baseline: true)]
    // Wait to remove other ci
    //[SimpleJob(RunStrategy.Throughput, targetCount: 10, runtimeMoniker: RuntimeMoniker.NetCoreApp50)]
    [MinColumn, MaxColumn, MedianColumn]
    public class EnforcerBenchmark
    {
        private Enforcer NowEnforcer { get; set; }
        private string NowTestUserName { get; set; }
        private string NowTestDataName { get; set; }
        private TestResource NowTestResource { get; set; }
        private int[][] RbacScale { get; } =
        {
            new[] {100, 1000},    // Small
            new[] {1000, 10000},  // Medium
            new[] {10000, 100000} // Large
        };

        #region GlobalSetup
        [GlobalSetup(Targets = new[] {nameof(BasicModel)})]
        public void GlobalSetupForBasicModel()
        {
            GlobalSetupFromFile("basic_model.conf", "basic_policy.csv");
            Console.WriteLine("// Set the Basic enforcer");
        }

        [GlobalSetup(Targets = new[] {nameof(RbacModel)})]
        public void GlobalSetupForRbacModel()
        {
            GlobalSetupFromFile("rbac_model.conf", "rbac_policy.csv");
            Console.WriteLine("// Set the Rbac Model enforcer");
        }

        [GlobalSetup(Targets = new[] {nameof(RbacModelWithSmallScale)})]
        public void GlobalSetupForRbacModelWithSmallScale()
        {
            int groupCount = RbacScale[0][0];
            int userCount = RbacScale[0][1];
            GlobalSetupForRbacModelWithScale(groupCount, userCount);
            Console.WriteLine($"// Set the Rbac Model with small scale ({groupCount} groups and {userCount} users) enforcer.");
        }

        [GlobalSetup(Targets = new[] {nameof(RbacModelWithMediumScale)})]
        public void GlobalSetupForRbacModelWithMediumScale()
        {
            int groupCount = RbacScale[1][0];
            int userCount = RbacScale[1][1];
            GlobalSetupForRbacModelWithScale(groupCount, userCount);
            Console.WriteLine($"// Set the Rbac Model with medium scale ({groupCount} groups and {userCount} users) enforcer.");
        }

        [GlobalSetup(Targets = new[] { nameof(RbacModelWithLargeScale) })]
        public void GlobalSetupForRbacModelWithLargeScale()
        {
            int groupCount = RbacScale[2][0];
            int userCount = RbacScale[2][1];
            GlobalSetupForRbacModelWithScale(groupCount, userCount);
            Console.WriteLine($"// Set the RBAC with large scale ({groupCount} groups and {userCount} users) enforcer.");
        }

        [GlobalSetup(Targets = new[] {nameof(RbacModelWithResourceRoles)})]
        public void GlobalSetupForRbacModelWithResourceRoles()
        {
            GlobalSetupFromFile("rbac_with_resource_roles_model.conf", "rbac_with_resource_roles_policy.csv");
            Console.WriteLine("// Set the RbacModel With Resource Roles enforcer");
        }

        [GlobalSetup(Targets = new[] {nameof(RbacModelWithDomains)})]
        public void GlobalSetupForRbacModelWithDomains()
        {
            GlobalSetupFromFile("rbac_with_domains_model.conf", "rbac_with_domains_policy.csv");
            Console.WriteLine("// Set the Rbac Model With Domains enforcer");
        }

        [GlobalSetup(Targets = new[] {nameof(RbacModelWithDeny)})]
        public void GlobalSetupForRbacModelWithDeny()
        {
            GlobalSetupFromFile("rbac_with_deny_model.conf", "rbac_with_deny_policy.csv");
            Console.WriteLine("// Set the Rbac Model With Deny enforcer");
        }

        [GlobalSetup(Targets = new[] {nameof(AbacModel)})]
        public void GlobalSetupForAbacModel()
        {
            GlobalSetupFromFile("abac_model.conf");
            NowTestResource = new TestResource("data1", "alice");
            Console.WriteLine("// Set the Abac Model enforcer");
        }

        [GlobalSetup(Targets = new[] {nameof(KeyMatchModel)})]
        public void GlobalSetupForKeyMatchModel()
        {
            GlobalSetupFromFile("keymatch_model.conf", "keymatch_policy.csv");
            Console.WriteLine("// Set the Key Match Model enforcer");
        }

        [GlobalSetup(Targets = new[] {nameof(PriorityModel)})]
        public void GlobalSetupForPriorityModel()
        {
            GlobalSetupFromFile("priority_model.conf", "priority_policy.csv");
            Console.WriteLine("// Set the Priority Model enforcer");
        }
        #endregion

        #region private help method
        private void GlobalSetupForRbacModelWithScale(int groupCount, int userCount)
        {
            GlobalSetupForRbacModel();
            for (int i = 0; i < groupCount; i++)
            {
                NowEnforcer.AddPolicy($"group{i}", $"data{i / 10}", "read");
            }

            // Because we still have the policies management api,
            // so it will run BuildRoleLink at every loop.
            NowEnforcer.EnableAutoBuildRoleLinks(false);
            for (int i = 0; i < userCount; i++)
            {
                NowEnforcer.AddGroupingPolicy($"group{i}", $"group{i / 10}");
            }
            NowEnforcer.BuildRoleLinks();

            NowTestUserName = $"user{userCount / 2 + 1}"; // if 1000 => 501...
            NowTestDataName = $"group{groupCount - 1}"; // if 100 => 99...
            Console.WriteLine($"// Already set user name to {NowTestUserName}.");
            Console.WriteLine($"// Already set data name to {NowTestDataName}.");
        }
        
        private void GlobalSetupFromFile(string modelFileName, string policyFileName = null)
        {
            if (policyFileName is null)
            {
                NowEnforcer = new Enforcer(
                    GetTestFilePath(modelFileName));
                return;
            }

            NowEnforcer = new Enforcer(
                GetTestFilePath(modelFileName),
                GetTestFilePath(policyFileName),
                false);
        }
        #endregion

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            NowEnforcer = null;
            Console.WriteLine("// Cleaned the enforcer");
        }

        [Benchmark(Description = "ACL, 2 rules (2 users)")]
        [BenchmarkCategory("BasicModel")]
        public void BasicModel()
        {
            _ = NowEnforcer.Enforce("alice", "data1", "read");
        }

        [Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModel()
        {
            _ = NowEnforcer.Enforce("alice", "data2", "read");
        }

        [Benchmark(Description = "RBAC (small), 1100 rules (1000 users, 100 roles)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelWithSmallScale()
        {
            _ = NowEnforcer.Enforce(NowTestUserName, NowTestDataName, "read");
        }

        [Benchmark(Description = "RBAC (medium), 11000 rules (10000 users, 1000 roles)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelWithMediumScale()
        {
            _ = NowEnforcer.Enforce(NowTestUserName, NowTestDataName, "read");
        }

        [Benchmark(Description = "RBAC (large), 110000 rules (100000 users, 10000 roles)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelWithLargeScale()
        {
            _ = NowEnforcer.Enforce(NowTestUserName, NowTestDataName, "read");
        }

        [Benchmark(Description = "RBAC with resource roles, 6 rules (2 users, 2 roles)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelWithResourceRoles()
        {
            _ = NowEnforcer.Enforce("alice", "data1", "read");
        }

        [Benchmark(Description = "RBAC with domains/tenants, 6 rules (2 users, 1 role, 2 domains)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelWithDomains()
        {
            _ = NowEnforcer.Enforce("alice", "domain1", "data1", "read");
        }

        [Benchmark(Description = "Deny-override, 6 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelWithDeny()
        {
            _ = NowEnforcer.Enforce("alice", "data1", "read");
        }

        [Benchmark(Description = "ABAC, 0 rule (0 user)")]
        [BenchmarkCategory("AbacModel")]
        public void AbacModel()
        {
            var data1 = NowTestResource;
            _ = NowEnforcer.Enforce("alice", data1, "read");
        }

        [Benchmark(Description = "RESTful, 5 rules (3 users)")]
        [BenchmarkCategory("KeyMatchModel")]
        public void KeyMatchModel()
        {
            _ = NowEnforcer.Enforce("alice", "/alice_data/resource1", "GET");
        }

        [Benchmark(Description = "Priority, 9 rules (2 users, 2 roles)")]
        [BenchmarkCategory("PriorityModel")]
        public void PriorityModel()
        {
            _ = NowEnforcer.Enforce("alice", "data1", "read");
        }
    }
}
