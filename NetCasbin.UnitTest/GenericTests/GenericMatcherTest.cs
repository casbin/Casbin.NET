using System;
using Casbin.Model;
using DynamicExpresso;
using Xunit;

namespace Casbin.UnitTests.GenericTests;

public class GenericMatcherTest
{
    [Fact]
    public void TestGenericMatcher()
    {
        var r = Request.Create("A", 1);
        var p = Policy.Create("A", 1);
        var func1 = Compile("r.Value1 == p.Value1 && r.Value2 == p.Value2",
            nameof(r), in r, nameof(p), in p);

        Assert.True(func1(Request.Create("A", 1), Policy.Create("A", 1)));
        Assert.False(func1(Request.Create("A", 1), Policy.Create("A", 2)));
        Assert.False(func1(Request.Create("B", 1), Policy.Create("B", 2)));

        var r2 = Request.Create("A", 1, "read");
        var p2 = Policy.Create("A", 1, "read");
        var func2 = Compile("r2.Value1 == p2.Value1 && r2.Value2 == p2.Value2 && r2.Value3 == p2.Value3",
            nameof(r2), in r2, nameof(p2), in p2);

        Assert.True(func2(Request.Create("A", 1, "read"), Policy.Create("A", 1, "read")));
        Assert.False(func2(Request.Create("A", 1, "read"), Policy.Create("A", 2, "read")));
        Assert.False(func2(Request.Create("B", 1, "read"), Policy.Create("B", 2, "read")));
    }

    private static Func<TRequest, TPolicy, bool> Compile<TRequest, TPolicy> 
    (
        string expressionText,
        string requestType, in TRequest r,
        string policyType, in TPolicy p)
        where TRequest : struct, IRequestValues where TPolicy : IPolicyValues
    {
        var interpreter = new Interpreter();
        return interpreter.ParseAsDelegate<Func<TRequest, TPolicy, bool>>(
            expressionText, requestType, policyType
        );
    }
}
