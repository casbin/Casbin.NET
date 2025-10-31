using System.IO;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Persist.Adapter.File;
using Xunit;

namespace Casbin.UnitTests.PersistTests;

public class IncrementalFilteredAdapterTest
{
    [Fact]
    public void TestLoadIncrementalFilteredPolicy()
    {
        Enforcer e = new("Examples/rbac_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        FileAdapter a = new("Examples/rbac_policy.csv");
        e.SetAdapter(a);

        // Load only p policies for alice
        e.LoadFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["alice"])));

        // alice can read data1 (from p policy)
        Assert.True(e.Enforce("alice", "data1", "read"));
        // bob cannot write data2 (not loaded yet)
        Assert.False(e.Enforce("bob", "data2", "write"));

        // Incrementally load p policies for data2_admin role
        e.LoadIncrementalFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["data2_admin"])));

        // alice still can only read data1 (no role link yet)
        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data2", "read"));

        // Incrementally load g policies for alice
        e.LoadIncrementalFilteredPolicy(new PolicyFilter(PermConstants.DefaultRoleType, 0, Policy.ValuesFrom(["alice"])));

        // Now alice can read data2 through role inheritance
        Assert.True(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));
        // bob still cannot write data2 (not loaded)
        Assert.False(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestLoadIncrementalFilteredPolicyAsync()
    {
        Enforcer e = new("Examples/rbac_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

        FileAdapter a = new("Examples/rbac_policy.csv");
        e.SetAdapter(a);

        // Load only p policies for alice
        await e.LoadFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["alice"])));

        // alice can read data1 (from p policy)
        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        // bob cannot write data2 (not loaded yet)
        Assert.False(await e.EnforceAsync("bob", "data2", "write"));

        // Incrementally load p policies for data2_admin role
        await e.LoadIncrementalFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["data2_admin"])));

        // alice still can only read data1 (no role link yet)
        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));

        // Incrementally load g policies for alice
        await e.LoadIncrementalFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultRoleType, 0, Policy.ValuesFrom(["alice"])));

        // Now alice can read data2 through role inheritance
        Assert.True(await e.EnforceAsync("alice", "data2", "read"));
        Assert.True(await e.EnforceAsync("alice", "data2", "write"));
        // bob still cannot write data2 (not loaded)
        Assert.False(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestLoadIncrementalFilteredPolicyMultiplePTypes()
    {
        Enforcer e = new("Examples/rbac_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        FileAdapter a = new("Examples/rbac_policy.csv");
        e.SetAdapter(a);

        // Load only p policies for alice
        e.LoadFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["alice"])));

        // alice can read data1
        Assert.True(e.Enforce("alice", "data1", "read"));
        // bob cannot write data2
        Assert.False(e.Enforce("bob", "data2", "write"));

        // Incrementally load p policies for bob
        e.LoadIncrementalFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        // alice still can read data1
        Assert.True(e.Enforce("alice", "data1", "read"));
        // Now bob can write data2
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestLoadIncrementalFilteredPolicyMultiplePTypesAsync()
    {
        Enforcer e = new("Examples/rbac_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

        FileAdapter a = new("Examples/rbac_policy.csv");
        e.SetAdapter(a);

        // Load only p policies for alice
        await e.LoadFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["alice"])));

        // alice can read data1
        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        // bob cannot write data2
        Assert.False(await e.EnforceAsync("bob", "data2", "write"));

        // Incrementally load p policies for bob
        await e.LoadIncrementalFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        // alice still can read data1
        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        // Now bob can write data2
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestLoadFilteredPolicyClearsExistingPolicies()
    {
        Enforcer e = new("Examples/rbac_model.conf");
        FileAdapter a = new("Examples/rbac_policy.csv");
        e.SetAdapter(a);

        // Load only p policies for alice
        e.LoadFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["alice"])));
        Assert.True(e.Enforce("alice", "data1", "read"));

        // Load filtered policy again (should clear previous policies)
        e.LoadFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        // alice can no longer read data1 (previous policies cleared)
        Assert.False(e.Enforce("alice", "data1", "read"));
        // bob can write data2 (new filtered policies loaded)
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestLoadFilteredPolicyClearsExistingPoliciesAsync()
    {
        Enforcer e = new("Examples/rbac_model.conf");
        FileAdapter a = new("Examples/rbac_policy.csv");
        e.SetAdapter(a);

        // Load only p policies for alice
        await e.LoadFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["alice"])));
        Assert.True(await e.EnforceAsync("alice", "data1", "read"));

        // Load filtered policy again (should clear previous policies)
        await e.LoadFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        // alice can no longer read data1 (previous policies cleared)
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));
        // bob can write data2 (new filtered policies loaded)
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }
}
