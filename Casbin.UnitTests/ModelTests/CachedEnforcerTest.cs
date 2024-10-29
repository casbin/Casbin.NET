using Casbin.UnitTests.Fixtures;
using Casbin.UnitTests.Mock;
using Xunit;
using Xunit.Abstractions;

namespace Casbin.UnitTests.ModelTests;

[Collection("Model collection")]
public class CachedEnforcerTest
{
    private readonly TestModelFixture _testModelFixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public CachedEnforcerTest(ITestOutputHelper testOutputHelper, TestModelFixture testModelFixture)
    {
        _testOutputHelper = testOutputHelper;
        _testModelFixture = testModelFixture;
    }

    [Fact]
    public void TestEnforceWithCache()
    {
#if !NET452
        Enforcer e = new(TestModelFixture.GetBasicTestModel())
        {
            Logger = new MockLogger<Enforcer>(_testOutputHelper)
        };
#else
            var e = new Enforcer(TestModelFixture.GetBasicTestModel());
#endif
        e.EnableCache(true);
        e.EnableAutoCleanEnforceCache(false);

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));

        // The cache is enabled, so even if we remove a policy rule, the decision
        // for ("alice", "data1", "read") will still be true, as it uses the cached result.
        _ = e.RemovePolicy("alice", "data1", "read");

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));

        // Now we invalidate the cache, then all first-coming Enforce() has to be evaluated in real-time.
        // The decision for ("alice", "data1", "read") will be false now.
        e.EnforceCache.Clear();

        Assert.False(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
    }

    [Fact]
    public void TestAutoCleanCache()
    {
#if !NET452
        Enforcer e = new(TestModelFixture.GetBasicTestModel())
        {
            Logger = new MockLogger<Enforcer>(_testOutputHelper)
        };
#else
            var e = new Enforcer(TestModelFixture.GetBasicTestModel());
#endif
        e.EnableCache(true);

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));

        // The cache is enabled, so even if we remove a policy rule, the decision
        // for ("alice", "data1", "read") will still be true, as it uses the cached result.
        _ = e.RemovePolicy("alice", "data1", "read");

        Assert.False(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
    }
}
