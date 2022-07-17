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
        var r = Request.CreateValues("A", 1);
        PolicyValues<string, int> p = Policy.CreateValues("A", 1);
        var func1 = Compile("r.Value1 == p.Value1 && r.Value2 == p.Value2",
            nameof(r), in r, nameof(p), in p);

        Assert.True(func1(Request.CreateValues("A", 1), Policy.CreateValues("A", 1)));
        Assert.False(func1(Request.CreateValues("A", 1), Policy.CreateValues("A", 2)));
        Assert.False(func1(Request.CreateValues("B", 1), Policy.CreateValues("B", 2)));

        var r2 = Request.CreateValues("A", 1, "read");
        PolicyValues<string, int, string> p2 = Policy.CreateValues("A", 1, "read");
        var func2 = Compile("r2.Value1 == p2.Value1 && r2.Value2 == p2.Value2 && r2.Value3 == p2.Value3",
            nameof(r2), in r2, nameof(p2), in p2);

        Assert.True(func2(Request.CreateValues("A", 1, "read"), Policy.CreateValues("A", 1, "read")));
        Assert.False(func2(Request.CreateValues("A", 1, "read"), Policy.CreateValues("A", 2, "read")));
        Assert.False(func2(Request.CreateValues("B", 1, "read"), Policy.CreateValues("B", 2, "read")));
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
