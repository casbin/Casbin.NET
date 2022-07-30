using System;
using System.Threading.Tasks;
using Casbin.Adapter.File;
using Casbin.Persist;
using Casbin.UnitTests.Fixtures;
using Xunit;

namespace Casbin.UnitTests.PersistTests;

[Collection("Model collection")]
public class WatcherTest
{
    private readonly TestModelFixture _testModelFixture;

    public WatcherTest(TestModelFixture testModelFixture) => _testModelFixture = testModelFixture;

    [Fact]
    public void ShouldUpdate()
    {
        SampleWatcher sampleWatcher = new();
        Assert.False(sampleWatcher.Called);

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);
        enforcer.SavePolicy();
        Assert.True(sampleWatcher.Called);
    }

    [Fact]
    public async Task ShouldUpdateAsync()
    {
        SampleWatcher sampleWatcher = new();
        Assert.False(sampleWatcher.AsyncCalled);

        Enforcer enforcer = new(_testModelFixture.GetBasicTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_async_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);
        await enforcer.SavePolicyAsync();
        Assert.True(sampleWatcher.AsyncCalled);
    }

    public class SampleWatcher : IWatcher
    {
        private Func<Task> _asyncCallback;
        private Action _callback;

        public bool Called { get; private set; }

        public bool AsyncCalled { get; private set; }

        public void SetUpdateCallback(Action callback) => _callback = callback;

        public void SetUpdateCallback(Func<Task> callback) => _asyncCallback = callback;

        public void Update()
        {
            _callback?.Invoke();
            Called = true;
        }

        public async Task UpdateAsync()
        {
            if (!(_asyncCallback is null))
            {
                await _asyncCallback.Invoke();
            }

            AsyncCalled = true;
        }
    }
}
