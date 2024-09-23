using System.IO;
using System.Threading.Tasks;
using Casbin.Persist.Adapter.File;
using Xunit;

namespace Casbin.UnitTests.PersistTests.FileTests;

public class FileAdapterTest
{
    [Fact]
    public void TestLoadPolicy()
    {
        // Test read file
        FileAdapter adapter = new("examples/rbac_policy.csv");
        Enforcer e = new("examples/rbac_model.conf", adapter);
        e.LoadPolicy();
        e.BuildRoleLinks();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.True(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));

        // Test read file by create API
        adapter = FileAdapter.CreateFromFile("examples/rbac_policy.csv");
        e = new("examples/rbac_model.conf", adapter);
        e.LoadPolicy();
        e.BuildRoleLinks();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.True(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));

        // Test read text
        string text = File.ReadAllText("examples/rbac_policy.csv");
        adapter = FileAdapter.CreateFromText(text);
        e = new("examples/rbac_model.conf", adapter);
        e.LoadPolicy();
        e.BuildRoleLinks();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.True(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestLoadPolicyAsync()
    {
        // Test read file
        FileAdapter adapter = new("examples/rbac_policy.csv");
        Enforcer e = new("examples/rbac_model.conf", adapter);
        await e.LoadPolicyAsync();
        e.BuildRoleLinks();

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.True(await e.EnforceAsync("alice", "data2", "read"));
        Assert.True(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));

        // Test read file by create API
        adapter = FileAdapter.CreateFromFile("examples/rbac_policy.csv");
        e = new("examples/rbac_model.conf", adapter);
        await e.LoadPolicyAsync();
        e.BuildRoleLinks();

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.True(await e.EnforceAsync("alice", "data2", "read"));
        Assert.True(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));

        // Test read text
        string text = File.ReadAllText("examples/rbac_policy.csv");
        adapter = FileAdapter.CreateFromText(text);
        e = new("examples/rbac_model.conf", adapter);
        await e.LoadPolicyAsync();
        e.BuildRoleLinks();

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.True(await e.EnforceAsync("alice", "data2", "read"));
        Assert.True(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }
}
