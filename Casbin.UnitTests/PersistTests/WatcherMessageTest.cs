using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.Adapter.File;
using Casbin.Model;
using Casbin.Persist;
using Casbin.UnitTests.Fixtures;
using Xunit;

namespace Casbin.UnitTests.PersistTests;

[Collection("Model collection")]
public class WatcherMessageTest
{
    private readonly TestModelFixture _testModelFixture;

    public WatcherMessageTest(TestModelFixture testModelFixture) => _testModelFixture = testModelFixture;

    private void MessageEquals(IWatcherMessage message, IWatcherMessage message2)
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

        enforcer.SetWatcher(sampleWatcher, false);

        enforcer.AddPolicy("alice", "data1", "read");
        Assert.True(sampleWatcher.WatcherMessage is null);

        enforcer.AddPolicy("alice", "book1", "write");
        MessageEquals(sampleWatcher.WatcherMessage,
            WatcherMessage.CreateAddPolicyMessage("p", "p", Policy.ValuesFrom(new[] { "alice", "book1", "write" })));
    }

    [Fact]
    public void TestRemovePolicy()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);

        enforcer.RemovePolicy("alice", "data1", "write");
        Assert.True(sampleWatcher.WatcherMessage is null);

        enforcer.RemovePolicy("alice", "data1", "read");
        MessageEquals(sampleWatcher.WatcherMessage,
            WatcherMessage.CreateRemovePolicyMessage("p", "p", Policy.ValuesFrom(new[] { "alice", "data1", "read" })));
    }

    [Fact]
    public void TestUpdatePolicy()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);

        enforcer.UpdatePolicy(new[] { "alice", "book1", "read" }, "alice", "book2", "write");
        Assert.True(sampleWatcher.WatcherMessage is null);

        enforcer.UpdatePolicy(new[] { "alice", "data1", "read" }, "alice", "book2", "write");
        MessageEquals(sampleWatcher.WatcherMessage,
            WatcherMessage.CreateUpdatePolicyMessage("p", "p",
                Policy.ValuesFrom(new[] { "alice", "data1", "read" }),
                Policy.ValuesFrom(new[] { "alice", "book2", "write" })));
    }

    [Fact]
    public void TestSavePolicy()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);
        enforcer.SavePolicy();
        MessageEquals(sampleWatcher.WatcherMessage, WatcherMessage.CreateSavePolicyMessage());
    }

    [Fact]
    public void TestAddPolicies()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);

        enforcer.AddPolicies(new[] { new[] { "data2_admin", "data2", "read" }, new[] { "frank", "book4", "read" } });
        Assert.True(sampleWatcher.WatcherMessage is null);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            new[] { "cindy", "book5", "write" }, new[] { "frank", "book4", "read" }
        };
        enforcer.AddPolicies(rules);
        MessageEquals(sampleWatcher.WatcherMessage,
            WatcherMessage.CreateAddPoliciesMessage("p", "p", Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public void TestRemovePolicies()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            new[] { "data200_admin", "data2", "read" }, new[] { "alice", "book3", "read" }
        };
        enforcer.RemovePolicies(rules);
        Assert.True(sampleWatcher.WatcherMessage is null);

        rules = new[] { new[] { "data2_admin", "data2", "read" }, new[] { "alice", "book3", "read" } };
        enforcer.RemovePolicies(rules);
        MessageEquals(sampleWatcher.WatcherMessage,
            WatcherMessage.CreateRemovePoliciesMessage("p", "p", Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public void TestUpdatePolicies()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);

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
            WatcherMessage.CreateUpdatePoliciesMessage("p", "p", Policy.ValuesListFrom(oldRules),
                Policy.ValuesListFrom(newRules)));
    }

    [Fact]
    public void TestRemoveFilteredPolicy()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);

        enforcer.RemoveFilteredPolicy(1, "data3");
        Assert.True(sampleWatcher.WatcherMessage is null);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            new[] { "data2_admin", "data2", "read" }, new[] { "data2_admin", "data2", "write" }
        };
        enforcer.RemoveFilteredPolicy(0, "data2_admin");
        MessageEquals(sampleWatcher.WatcherMessage,
            WatcherMessage.CreateRemoveFilteredPolicyMessage("p", "p", 0, Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public async Task TestAddPolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);

        await enforcer.AddPolicyAsync("alice", "data1", "read");
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        await enforcer.AddPolicyAsync("alice", "book1", "write");
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            WatcherMessage.CreateAddPolicyMessage("p", "p", Policy.ValuesFrom(new[] { "alice", "book1", "write" })));
    }

    [Fact]
    public async Task TestRemovePolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);

        await enforcer.RemovePolicyAsync("alice", "data1", "write");
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        await enforcer.RemovePolicyAsync("alice", "data1", "read");
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            WatcherMessage.CreateRemovePolicyMessage("p", "p", Policy.ValuesFrom(new[] { "alice", "data1", "read" })));
    }

    [Fact]
    public async Task TestUpdatePolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);

        await enforcer.UpdatePolicyAsync(new[] { "alice", "book1", "read" }, "alice", "book2", "write");
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        await enforcer.UpdatePolicyAsync(new[] { "alice", "data1", "read" }, "alice", "book2", "write");
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            WatcherMessage.CreateUpdatePolicyMessage("p", "p", Policy.ValuesFrom(new[] { "alice", "data1", "read" }),
                Policy.ValuesFrom(new[] { "alice", "book2", "write" })));
    }

    [Fact]
    public async Task TestSavePolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);

        await enforcer.SavePolicyAsync();
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            WatcherMessage.CreateSavePolicyMessage());
    }

    [Fact]
    public async Task TestAddPoliciesAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);

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
            WatcherMessage.CreateAddPoliciesMessage("p", "p", Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public async Task TestRemovePoliciesAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            new[] { "data200_admin", "data2", "read" }, new[] { "alice", "book3", "read" }
        };
        await enforcer.RemovePoliciesAsync(rules);
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        rules = new[] { new[] { "data2_admin", "data2", "read" }, new[] { "alice", "book3", "read" } };
        await enforcer.RemovePoliciesAsync(rules);
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            WatcherMessage.CreateRemovePoliciesMessage("p", "p", Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public async Task TestUpdatePoliciesAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);

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
            WatcherMessage.CreateUpdatePoliciesMessage("p", "p", Policy.ValuesListFrom(oldRules),
                Policy.ValuesListFrom(newRules)));
    }

    [Fact]
    public async Task TestRemoveFilteredPolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(_testModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher, false);

        await enforcer.RemoveFilteredPolicyAsync(1, "data3");
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            new[] { "data2_admin", "data2", "read" }, new[] { "data2_admin", "data2", "write" }
        };
        await enforcer.RemoveFilteredPolicyAsync(0, "data2_admin");
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            WatcherMessage.CreateRemoveFilteredPolicyMessage("p", "p", 0, Policy.ValuesListFrom(rules)));
    }

    private class SampleWatcher : IWatcher
    {
        private Func<Task> _asyncCallback;
        private Action _callback;

        public IWatcherMessage WatcherMessage { get; private set; }

        public IWatcherMessage AsyncWatcherMessage { get; private set; }

        public void SetUpdateCallback(Action callback) => _callback = callback;

        public void SetUpdateCallback(Func<Task> callback) => _asyncCallback = callback;

        public void Update(IWatcherMessage watcherMessage)
        {
            _callback?.Invoke();
            WatcherMessage = watcherMessage;
        }

        public async Task UpdateAsync(IWatcherMessage watcherMessage)
        {
            if (!(_asyncCallback is null))
            {
                await _asyncCallback.Invoke();
            }

            AsyncWatcherMessage = watcherMessage;
        }
    }
}
