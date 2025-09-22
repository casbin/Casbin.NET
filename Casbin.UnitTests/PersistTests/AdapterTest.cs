using System.IO;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Persist.Adapter.File;
using Casbin.Persist.Adapter.Stream;
using Casbin.Persist.Adapter.Text;
using Xunit;

namespace Casbin.UnitTests.PersistTests;

public class AdapterTest
{
    [Fact]
    public void TestFileAdapter()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        FileAdapter a = new("Examples/basic_policy.csv");
        e.SetAdapter(a);
        e.LoadPolicy();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestFileAdapterAsync()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

        FileAdapter a = new("Examples/basic_policy.csv");
        e.SetAdapter(a);
        await e.LoadPolicyAsync();

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestFilteredFileAdapter()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        FileAdapter a = new("Examples/basic_policy.csv");
        e.SetAdapter(a);
        e.LoadFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        Assert.False(e.Enforce("alice", "data1", "read")); // because "alice" is not in the filtered policy
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestFilteredFileAdapterAsync()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

        FileAdapter a = new("Examples/basic_policy.csv");
        e.SetAdapter(a);
        await e.LoadFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        Assert.False(await e.EnforceAsync("alice", "data1", "read")); // because "alice" is not in the filtered policy
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestStreamAdapter()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        StreamAdapter a = new(File.OpenRead("Examples/basic_policy.csv"));
        e.SetAdapter(a);
        e.LoadPolicy();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestStreamAdapterAsync()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

        StreamAdapter a = new(File.OpenRead("Examples/basic_policy.csv"));
        e.SetAdapter(a);
        await e.LoadPolicyAsync();

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestFilteredStreamAdapter()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        StreamAdapter a = new(File.OpenRead("Examples/basic_policy.csv"));
        e.SetAdapter(a);
        e.LoadFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        Assert.False(e.Enforce("alice", "data1", "read")); // because "alice" is not in the filtered policy
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestFilteredStreamAdapterAsync()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

        StreamAdapter a = new(File.OpenRead("Examples/basic_policy.csv"));
        e.SetAdapter(a);
        await e.LoadFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        Assert.False(await e.EnforceAsync("alice", "data1", "read")); // because "alice" is not in the filtered policy
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestTextAdapter()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        TextAdapter a = new(File.ReadAllText("Examples/basic_policy.csv"));
        e.SetAdapter(a);
        e.LoadPolicy();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestTextAdapterAsync()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

#if NET452 || NET461 || NET462
        TextAdapter a = new(File.ReadAllText("Examples/basic_policy.csv"));
#else
        TextAdapter a = new(await File.ReadAllTextAsync("Examples/basic_policy.csv"));
#endif
        e.SetAdapter(a);
        await e.LoadPolicyAsync();

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestFilteredTextAdapter()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        TextAdapter a = new(File.ReadAllText("Examples/basic_policy.csv"));
        e.SetAdapter(a);
        e.LoadFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        Assert.False(e.Enforce("alice", "data1", "read")); // because "alice" is not in the filtered policy
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestFilteredTextAdapterAsync()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

#if NET452 || NET461 || NET462
        TextAdapter a = new(File.ReadAllText("Examples/basic_policy.csv"));
#else
        TextAdapter a = new(await File.ReadAllTextAsync("Examples/basic_policy.csv"));
#endif
        e.SetAdapter(a);
        await e.LoadFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        Assert.False(await e.EnforceAsync("alice", "data1", "read")); // because "alice" is not in the filtered policy
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }
}
