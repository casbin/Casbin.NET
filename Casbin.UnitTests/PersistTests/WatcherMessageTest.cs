using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Persist.Adapter.File;
using Casbin.UnitTests.Fixtures;
using Xunit;

namespace Casbin.UnitTests.PersistTests;

[Collection("Model collection")]
public class WatcherMessageTest
{
    private readonly TestModelFixture _testModelFixture;

    public WatcherMessageTest(TestModelFixture testModelFixture) => _testModelFixture = testModelFixture;

    private void MessageEquals(PolicyChangedMessage message, PolicyChangedMessage message2)
    {
        Assert.Equal(message.Operation, message2.Operation);
        Assert.Equal(message.Section, message2.Section);
        Assert.Equal(message.Values, message2.Values);
        Assert.Equal(message.NewValues, message2.NewValues);
        Assert.Equal(message.PolicyType, message2.PolicyType);
        Assert.Equal(message.FieldIndex, message2.FieldIndex);
        Assert.Equal(message.ValuesList, message2.ValuesList);
        Assert.Equal(message.NewValuesList, message2.NewValuesList);
    }

    [Fact]
    public void TestAddPolicy()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        enforcer.AddPolicy("alice", "data1", "read");
        Assert.True(sampleWatcher.WatcherMessage is null);

        enforcer.AddPolicy("alice", "book1", "write");
        MessageEquals(sampleWatcher.WatcherMessage,
            PolicyChangedMessage.CreateAddPolicy("p", "p", Policy.ValuesFrom(new[] { "alice", "book1", "write" })));
    }

    [Fact]
    public void TestRemovePolicy()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        enforcer.RemovePolicy("alice", "data1", "write");
        Assert.True(sampleWatcher.WatcherMessage is null);

        enforcer.RemovePolicy("alice", "data1", "read");
        MessageEquals(sampleWatcher.WatcherMessage,
            PolicyChangedMessage.CreateRemovePolicy("p", "p", Policy.ValuesFrom(new[] { "alice", "data1", "read" })));
    }

    [Fact]
    public void TestUpdatePolicy()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        enforcer.UpdatePolicy(new[] { "alice", "book1", "read" }, "alice", "book2", "write");
        Assert.True(sampleWatcher.WatcherMessage is null);

        enforcer.UpdatePolicy(new[] { "alice", "data1", "read" }, "alice", "book2", "write");
        MessageEquals(sampleWatcher.WatcherMessage,
            PolicyChangedMessage.CreateUpdatePolicy("p", "p",
                Policy.ValuesFrom(new[] { "alice", "data1", "read" }),
                Policy.ValuesFrom(new[] { "alice", "book2", "write" })));
    }

    [Fact]
    public void TestSavePolicy()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);
        enforcer.SavePolicy();
        MessageEquals(sampleWatcher.WatcherMessage, PolicyChangedMessage.CreateSavePolicy());
    }

    [Fact]
    public void TestAddPolicies()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        enforcer.AddPolicies(new[] { new[] { "data2_admin", "data2", "read" }, new[] { "frank", "book4", "read" } });
        Assert.True(sampleWatcher.WatcherMessage is null);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            new[] { "cindy", "book5", "write" }, new[] { "frank", "book4", "read" }
        };
        enforcer.AddPolicies(rules);
        MessageEquals(sampleWatcher.WatcherMessage,
            PolicyChangedMessage.CreateAddPolicies("p", "p", Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public void TestRemovePolicies()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            new[] { "data200_admin", "data2", "read" }, new[] { "alice", "book3", "read" }
        };
        enforcer.RemovePolicies(rules);
        Assert.True(sampleWatcher.WatcherMessage is null);

        rules = new[] { new[] { "data2_admin", "data2", "read" }, new[] { "alice", "book3", "read" } };
        enforcer.RemovePolicies(rules);
        MessageEquals(sampleWatcher.WatcherMessage,
            PolicyChangedMessage.CreateRemovePolicies("p", "p", Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public void TestUpdatePolicies()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        enforcer.UpdatePolicies(new[] { new[] { "data2_admin", "data2", "read" }, new[] { "frank", "book4", "read" } },
            new[] { new[] { "data3_admin", "data200", "read" }, new[] { "frank", "book6", "read" } });
        Assert.True(sampleWatcher.WatcherMessage is null);

        IEnumerable<IEnumerable<string>> oldRules = new[]
        {
            new[] { "data2_admin", "data2", "read" }, new[] { "alice", "data1", "read" }
        };
        IEnumerable<IEnumerable<string>> newRules = new[]
        {
            new[] { "data4_admin", "data2", "read" }, new[] { "alice", "data3", "read" }
        };
        enforcer.UpdatePolicies(oldRules, newRules);
        MessageEquals(sampleWatcher.WatcherMessage,
            PolicyChangedMessage.CreateUpdatePolicies("p", "p", Policy.ValuesListFrom(oldRules),
                Policy.ValuesListFrom(newRules)));
    }

    [Fact]
    public void TestRemoveFilteredPolicy()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        enforcer.RemoveFilteredPolicy(1, "data3");
        Assert.True(sampleWatcher.WatcherMessage is null);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            new[] { "data2_admin", "data2", "read" }, new[] { "data2_admin", "data2", "write" }
        };
        enforcer.RemoveFilteredPolicy(0, "data2_admin");
        MessageEquals(sampleWatcher.WatcherMessage,
            PolicyChangedMessage.CreateRemoveFilteredPolicy("p", "p", 0, Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public async Task TestAddPolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        await enforcer.AddPolicyAsync("alice", "data1", "read");
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        await enforcer.AddPolicyAsync("alice", "book1", "write");
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateAddPolicy("p", "p", Policy.ValuesFrom(new[] { "alice", "book1", "write" })));
    }

    [Fact]
    public async Task TestRemovePolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        await enforcer.RemovePolicyAsync("alice", "data1", "write");
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        await enforcer.RemovePolicyAsync("alice", "data1", "read");
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateRemovePolicy("p", "p", Policy.ValuesFrom(new[] { "alice", "data1", "read" })));
    }

    [Fact]
    public async Task TestUpdatePolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        await enforcer.UpdatePolicyAsync(new[] { "alice", "book1", "read" }, "alice", "book2", "write");
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        await enforcer.UpdatePolicyAsync(new[] { "alice", "data1", "read" }, "alice", "book2", "write");
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateUpdatePolicy("p", "p", Policy.ValuesFrom(new[] { "alice", "data1", "read" }),
                Policy.ValuesFrom(new[] { "alice", "book2", "write" })));
    }

    [Fact]
    public async Task TestSavePolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        await enforcer.SavePolicyAsync();
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateSavePolicy());
    }

    [Fact]
    public async Task TestAddPoliciesAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        await enforcer.AddPoliciesAsync(new[]
        {
            new[] { "data2_admin", "data2", "read" }, new[] { "frank", "book4", "read" }
        });
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            new[] { "cindy", "book5", "write" }, new[] { "frank", "book4", "read" }
        };
        await enforcer.AddPoliciesAsync(rules);
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateAddPolicies("p", "p", Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public async Task TestRemovePoliciesAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            new[] { "data200_admin", "data2", "read" }, new[] { "alice", "book3", "read" }
        };
        await enforcer.RemovePoliciesAsync(rules);
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        rules = new[] { new[] { "data2_admin", "data2", "read" }, new[] { "alice", "book3", "read" } };
        await enforcer.RemovePoliciesAsync(rules);
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateRemovePolicies("p", "p", Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public async Task TestUpdatePoliciesAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        await enforcer.UpdatePoliciesAsync(
            new[] { new[] { "data2_admin", "data2", "read" }, new[] { "frank", "book4", "read" } },
            new[] { new[] { "data3_admin", "data200", "read" }, new[] { "frank", "book6", "read" } });
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        IEnumerable<IEnumerable<string>> oldRules = new[]
        {
            new[] { "data2_admin", "data2", "read" }, new[] { "alice", "data1", "read" }
        };
        IEnumerable<IEnumerable<string>> newRules = new[]
        {
            new[] { "data4_admin", "data2", "read" }, new[] { "alice", "data3", "read" }
        };
        await enforcer.UpdatePoliciesAsync(oldRules, newRules);
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateUpdatePolicies("p", "p", Policy.ValuesListFrom(oldRules),
                Policy.ValuesListFrom(newRules)));
    }

    [Fact]
    public async Task TestRemoveFilteredPolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        await enforcer.RemoveFilteredPolicyAsync(1, "data3");
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            new[] { "data2_admin", "data2", "read" }, new[] { "data2_admin", "data2", "write" }
        };
        await enforcer.RemoveFilteredPolicyAsync(0, "data2_admin");
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateRemoveFilteredPolicy("p", "p", 0, Policy.ValuesListFrom(rules)));
    }

    private class SampleWatcher : IWatcher
    {
        private Func<Task> _asyncCallback;
        private Action _callback;

        public PolicyChangedMessage WatcherMessage { get; private set; }

        public PolicyChangedMessage AsyncWatcherMessage { get; private set; }

        public void SetUpdateCallback(Action callback) => _callback = callback;

        public void SetUpdateCallback(Func<Task> callback) => _asyncCallback = callback;
        public void Update() => _callback?.Invoke();

        public async Task UpdateAsync()
        {
            if (_asyncCallback is not null)
            {
                await _asyncCallback.Invoke();
            }
        }

        public void SetUpdateCallback(Action<PolicyChangedMessage> callback) => throw new NotImplementedException();

        public void SetUpdateCallback(Func<PolicyChangedMessage, Task> callback) => throw new NotImplementedException();

        public void Update(PolicyChangedMessage watcherMessage)
        {
            _callback?.Invoke();
            WatcherMessage = watcherMessage;
        }

        public async Task UpdateAsync(PolicyChangedMessage watcherMessage)
        {
            if (_asyncCallback is not null)
            {
                await _asyncCallback.Invoke();
            }

            AsyncWatcherMessage = watcherMessage;
        }

        public void Close()
        {
        }
    }
}
