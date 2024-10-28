using System.IO;
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
}
