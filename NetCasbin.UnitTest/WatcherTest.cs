using System;
using System.Threading.Tasks;
using NetCasbin.Persist;
using NetCasbin.Persist.FileAdapter;
using NetCasbin.UnitTest.Fixtures;
using Xunit;

namespace NetCasbin.UnitTest
{
    [Collection("Model collection")]
    public class WatcherTest
    {
        private readonly TestModelFixture _testModelFixture;

        public WatcherTest(TestModelFixture testModelFixture)
        {
            _testModelFixture = testModelFixture;
        }

        public class SampleWatcher : IWatcher
        {
            private Action _callback;

            private Func<Task> _asyncCallback;

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

        [Fact]
        public void ShouldUpdate()
        {
            var sampleWatcher = new SampleWatcher();
            Assert.False(sampleWatcher.Called);

            var enforcer = new Enforcer(_testModelFixture.GetNewRbacTestModel(),
                new DefaultFileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

            enforcer.SetWatcher(sampleWatcher, false);
            enforcer.SavePolicy();
            Assert.True(sampleWatcher.Called);
        }

        [Fact]
        public async Task ShouldUpdateAsync()
        {
            var sampleWatcher = new SampleWatcher();
            Assert.False(sampleWatcher.AsyncCalled);

            var enforcer = new Enforcer(_testModelFixture.GetBasicTestModel(),
                new DefaultFileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_async_watcher_test.csv")));

            enforcer.SetWatcher(sampleWatcher);
            await enforcer.SavePolicyAsync();
            Assert.True(sampleWatcher.AsyncCalled);
        }
    }
}
