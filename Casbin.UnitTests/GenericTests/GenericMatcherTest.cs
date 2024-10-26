using System;
using Casbin.Evaluation;
using Casbin.Model;
using Casbin.UnitTests.Util;
using DynamicExpresso;
using Xunit;

namespace Casbin.UnitTests.GenericTests;

public class GenericMatcherTest
{
    [Fact]
    public void TestGenericMatcher()
    {
        Interpreter interpreter = new();
        RequestValues<string, int> r = Request.CreateValues("A", 1);
        PolicyValues<string, int> p = Policy.CreateValues("A", 1);
        Func<RequestValues<string, int>, PolicyValues<string, int>, bool> func1 =
            ExpressionUtil.Compile(interpreter, "r.Value1 == p.Value1 && r.Value2 == p.Value2",
                nameof(r), in r, nameof(p), in p);

        Assert.True(func1(Request.CreateValues("A", 1), Policy.CreateValues("A", 1)));
        Assert.False(func1(Request.CreateValues("A", 1), Policy.CreateValues("A", 2)));
        Assert.False(func1(Request.CreateValues("B", 1), Policy.CreateValues("B", 2)));

        RequestValues<string, int, string> r2 = Request.CreateValues("A", 1, "read");
        PolicyValues<string, int, string> p2 = Policy.CreateValues("A", 1, "read");
        Func<RequestValues<string, int, string>, PolicyValues<string, int, string>, bool> func2 =
            ExpressionUtil.Compile(interpreter, "r2.Value1 == p2.Value1 && r2.Value2 == p2.Value2 && r2.Value3 == p2.Value3",
                nameof(r2), in r2, nameof(p2), in p2);

        Assert.True(func2(Request.CreateValues("A", 1, "read"), Policy.CreateValues("A", 1, "read")));
        Assert.False(func2(Request.CreateValues("A", 1, "read"), Policy.CreateValues("A", 2, "read")));
        Assert.False(func2(Request.CreateValues("B", 1, "read"), Policy.CreateValues("B", 2, "read")));
    }



}
