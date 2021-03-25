using NetCasbin.UnitTest.Fixtures;
using NetCasbin.UnitTest.Mock;
using Xunit;
using Xunit.Abstractions;
using static NetCasbin.UnitTest.Util.TestUtil;

namespace NetCasbin.UnitTest
{
    [Collection("Model collection")]
    public class EnforcerCacheTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly TestModelFixture _testModelFixture;

        public EnforcerCacheTest(ITestOutputHelper testOutputHelper, TestModelFixture testModelFixture)
        {
            _testOutputHelper = testOutputHelper;
            _testModelFixture = testModelFixture;
        }

        [Fact]
        public void TestEnforceWithCache()
        {
#if !NET452
            var e = new Enforcer(_testModelFixture.GetBasicTestModel())
            {
                Logger = new MockLogger<Enforcer>(_testOutputHelper)
            };
#else
            var e = new Enforcer(_testModelFixture.GetBasicTestModel());
#endif
            e.EnableCache(true);
            e.EnableAutoCleanEnforceCache(false);

            TestEnforce(e, "alice", "data1", "read", true);
            TestEnforce(e, "alice", "data1", "write", false);
            TestEnforce(e, "alice", "data2", "read", false);
            TestEnforce(e, "alice", "data2", "write", false);

            // The cache is enabled, so even if we remove a policy rule, the decision
            // for ("alice", "data1", "read") will still be true, as it uses the cached result.
            _ = e.RemovePolicy("alice", "data1", "read");

            TestEnforce(e, "alice", "data1", "read", true);
            TestEnforce(e, "alice", "data1", "write", false);
            TestEnforce(e, "alice", "data2", "read", false);
            TestEnforce(e, "alice", "data2", "write", false);

            // Now we invalidate the cache, then all first-coming Enforce() has to be evaluated in real-time.
            // The decision for ("alice", "data1", "read") will be false now.
            e.EnforceCache.Clear();

            TestEnforce(e, "alice", "data1", "read", false);
            TestEnforce(e, "alice", "data1", "write", false);
            TestEnforce(e, "alice", "data2", "read", false);
            TestEnforce(e, "alice", "data2", "write", false);
        }

        [Fact]
        public void TestAutoCleanCache()
        {
#if !NET452
            var e = new Enforcer(_testModelFixture.GetBasicTestModel())
            {
                Logger = new MockLogger<Enforcer>(_testOutputHelper)
            };
#else
            var e = new Enforcer(_testModelFixture.GetBasicTestModel());
#endif
            e.EnableCache(true);

            TestEnforce(e, "alice", "data1", "read", true);
            TestEnforce(e, "alice", "data1", "write", false);
            TestEnforce(e, "alice", "data2", "read", false);
            TestEnforce(e, "alice", "data2", "write", false);

            // The cache is enabled, so even if we remove a policy rule, the decision
            // for ("alice", "data1", "read") will still be true, as it uses the cached result.
            _ = e.RemovePolicy("alice", "data1", "read");

            TestEnforce(e, "alice", "data1", "read", false);
            TestEnforce(e, "alice", "data1", "write", false);
            TestEnforce(e, "alice", "data2", "read", false);
            TestEnforce(e, "alice", "data2", "write", false);
        }
    }
}
