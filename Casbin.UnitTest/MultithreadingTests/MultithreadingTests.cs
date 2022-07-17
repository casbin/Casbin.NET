using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.UnitTests.Fixtures;
using Xunit;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.MultithreadingTests
{
    internal class ReaderWriterPolicyManagerForTest : ReaderWriterPolicyManager
    {
        private readonly bool _waitWhenWrite;
        private readonly bool _waitWhenRead;
        private readonly List<string> _result;

        public ReaderWriterPolicyManagerForTest(IPolicyStore policyStore, List<string> result, bool waitWhenWrite, bool waitWhenRead) : base(policyStore)
        {
            _result = result;
            _waitWhenWrite = waitWhenWrite;
            _waitWhenRead = waitWhenRead;
        }

        public static IPolicyManager Create(List<string> result, bool waitWhenWrite, bool waitWhenRead)
        {
            return new ReaderWriterPolicyManagerForTest(DefaultPolicyStore.Create(), result, waitWhenWrite, waitWhenRead);
        }

        public override Task<bool> RemovePolicyAsync(string section, string policyType, IEnumerable<string> rule)
        {
            return Task.Run(() =>
            {
                if (TryStartWrite() is false)
                {
                    return Task.FromResult(false);
                }

                try
                {
                    string[] ruleArray = rule as string[] ?? rule.ToArray();
                    // In this test, we ignore any adapter.
                    if (_waitWhenWrite)
                    {
                        Thread.Sleep(5000);
                    }
                    return Task.FromResult(PolicyStore.RemovePolicy(section, policyType, ruleArray));
                }
                finally
                {
                    EndWrite();
                }
            });
        }

        public override Task<bool> SavePolicyAsync()
        {
            return Task.Run(() =>
            {
                if (TryStartRead() is false)
                {
                    return Task.FromResult(false);
                }

                try
                {
                    // In this test, we ignore any adapter.
                    if (_waitWhenRead)
                    {
                        Thread.Sleep(5000);
                    }
                    return Task.FromResult(false);
                }
                finally
                {
                    EndRead();
                }
            });
        }

        public override void EndRead()
        {
            _result.Add("EndRead");
            base.EndRead();
        }

        public override void EndWrite()
        {
            _result.Add("EndWrite");
            base.EndWrite();
        }

        public override bool TryStartRead()
        {
            bool result = base.TryStartRead();
            _result.Add("TryStartRead " + result);
            return result;
        }

        public override bool TryStartWrite()
        {
            bool result = base.TryStartWrite();
            _result.Add("TryStartWrite " + result);
            return result;
        }
    }

    [Collection("Model collection")]
    public class MultithreadingTests
    {
        private readonly TestModelFixture _testModelFixture;

        public MultithreadingTests(TestModelFixture testModelFixture)
        {
            _testModelFixture = testModelFixture;
        }

        [Fact]
        public void TestWriteAfterWriteAsync()
        {
            var result = new List<string>();
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.Model.ReplacePolicyManager(ReaderWriterPolicyManagerForTest.Create(result, true, false));

            e.BuildRoleLinks();

            TestGetPolicy(e, AsList(
                AsList("alice", "data1", "read"),
                AsList("bob", "data2", "write"),
                AsList("data2_admin", "data2", "read"),
                AsList("data2_admin", "data2", "write")));

            var manager = e.PolicyManager;
            result.Clear();

            var task1 = manager.RemovePolicyAsync("p", "p", new List<string>{"alice", "data1", "read"});
            Thread.Sleep(1000);
            var task2 = manager.AddPolicyAsync("p", "p", new List<string>{"eve", "data3", "read"});
            Task.WaitAll(task1, task2);

            Assert.Equal(3, result.Count);
            Assert.Equal("TryStartWrite True", result[0]);
            Assert.Equal("TryStartWrite False", result[1]);
            Assert.Equal("EndWrite", result[2]);

            TestGetPolicy(e, AsList(
                    AsList("bob", "data2", "write"),
                    AsList("data2_admin", "data2", "read"),
                    AsList("data2_admin", "data2", "write")
                )
            );
        }

        [Fact]
        public void TestReadAfterWriteAsync()
        {
            var result = new List<string>();
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.Model.ReplacePolicyManager(ReaderWriterPolicyManagerForTest.Create(result, true, false));

            e.BuildRoleLinks();

            TestGetPolicy(e, AsList(
                AsList("alice", "data1", "read"),
                AsList("bob", "data2", "write"),
                AsList("data2_admin", "data2", "read"),
                AsList("data2_admin", "data2", "write")));

            var manager = e.PolicyManager;
            result.Clear();

            var task1 = manager.RemovePolicyAsync("p", "p", new List<string>{"alice", "data1", "read"});
            Thread.Sleep(1000);
            var task2 = manager.SavePolicyAsync();
            Task.WaitAll(task1, task2);

            Assert.Equal(3, result.Count);
            Assert.Equal("TryStartWrite True", result[0]);
            Assert.Equal("TryStartRead False", result[1]);
            Assert.Equal("EndWrite", result[2]);

            TestGetPolicy(e, AsList(
                    AsList("bob", "data2", "write"),
                    AsList("data2_admin", "data2", "read"),
                    AsList("data2_admin", "data2", "write")
                )
            );
        }

        [Fact]
        public void TestReadAfterReadAsync()
        {
            var result = new List<string>();
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.Model.ReplacePolicyManager(ReaderWriterPolicyManagerForTest.Create(result, false, true));

            e.BuildRoleLinks();

            TestGetPolicy(e, AsList(
                AsList("alice", "data1", "read"),
                AsList("bob", "data2", "write"),
                AsList("data2_admin", "data2", "read"),
                AsList("data2_admin", "data2", "write")));

            var manager = e.PolicyManager;
            result.Clear();

            var task1 = manager.SavePolicyAsync();
            Thread.Sleep(1000);
            var task2 = manager.SavePolicyAsync();
            Task.WaitAll(task1, task2);

            Assert.Equal("TryStartRead True", result[0]);
            Assert.Equal("TryStartRead True", result[1]);
            Assert.Equal("EndRead", result[2]);

            TestGetPolicy(e, AsList(
                    AsList("alice", "data1", "read"),
                    AsList("bob", "data2", "write"),
                    AsList("data2_admin", "data2", "read"),
                    AsList("data2_admin", "data2", "write")
                )
            );
        }

        [Fact]
        public void TestWriteAfterReadAsync()
        {
            var result = new List<string>();
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.Model.ReplacePolicyManager(ReaderWriterPolicyManagerForTest.Create(result, false, true));

            e.BuildRoleLinks();

            TestGetPolicy(e, AsList(
                AsList("alice", "data1", "read"),
                AsList("bob", "data2", "write"),
                AsList("data2_admin", "data2", "read"),
                AsList("data2_admin", "data2", "write")));

            var manager = e.PolicyManager;
            result.Clear();

            var task1 = manager.SavePolicyAsync();
            Thread.Sleep(1000);
            var task2 = manager.RemovePolicyAsync("p", "p", new List<string>{"alice", "data1", "read"});
            Task.WaitAll(task1, task2);

            Assert.Equal(3, result.Count);
            Assert.Equal("TryStartRead True", result[0]);
            Assert.Equal("TryStartWrite False", result[1]);
            Assert.Equal("EndRead", result[2]);

            TestGetPolicy(e, AsList(
                    AsList("alice", "data1", "read"),
                    AsList("bob", "data2", "write"),
                    AsList("data2_admin", "data2", "read"),
                    AsList("data2_admin", "data2", "write")
                )
            );
        }

        [Fact]
        public void TestNoWaitingAsync()
        {
            var result = new List<string>();
            var e = new Enforcer(_testModelFixture.GetNewRbacTestModel());
            e.Model.ReplacePolicyManager(ReaderWriterPolicyManagerForTest.Create(result, false, false));

            e.BuildRoleLinks();

            TestGetPolicy(e, AsList(
                AsList("alice", "data1", "read"),
                AsList("bob", "data2", "write"),
                AsList("data2_admin", "data2", "read"),
                AsList("data2_admin", "data2", "write")));

            var manager = e.PolicyManager;
            result.Clear();

            var rules = AsList(
                AsList("jack", "data4", "read"),
                AsList("jack", "data4", "read"),
                AsList("jack", "data4", "read"),
                AsList("katy", "data4", "write"),
                AsList("leyo", "data4", "read"),
                AsList("katy", "data4", "write"),
                AsList("katy", "data4", "write"),
                AsList("ham", "data4", "write")
            );

            var task1 = manager.RemovePolicyAsync("p", "p", new List<string>{"alice", "data1", "read"});
            var task2 = manager.RemovePolicyAsync("p", "p", new List<string>{"bob", "data2", "write"});
            var task3 = manager.SavePolicyAsync();
            var task4 = manager.AddPoliciesAsync("p", "p", rules);
            var task5 = manager.AddPolicyAsync("p", "p", new List<string>{"eve", "data3", "read"});
            Task.WaitAll(task1, task2, task3, task4, task5);

            int tryStartReadTrueCount = 0, tryStartWriteTrueCount = 0, endReadCount = 0, endWriteCount = 0;
            foreach (string r in result)
            {
                switch (r)
                {
                    case "TryStartWrite True": tryStartWriteTrueCount++; break;
                    case "TryStartRead True": tryStartReadTrueCount++; break;
                    case "EndWrite": endWriteCount++; break;
                    case "EndRead": endReadCount++; break;
                }
            }
            Assert.Equal(1, tryStartReadTrueCount);
            Assert.Equal(4, tryStartWriteTrueCount);
            Assert.Equal(1, endReadCount);
            Assert.Equal(4, endWriteCount);
        }
    }
}
