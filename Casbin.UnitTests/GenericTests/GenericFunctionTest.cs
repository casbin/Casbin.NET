using System;
using Casbin.Model;
using Casbin.UnitTests.Util;
using DynamicExpresso;
using Xunit;

namespace Casbin.UnitTests.GenericTests;

public class GenericFunctionTest
{
    [Fact]
    public void TestGenericFunction()
    {
        Interpreter interpreter = new();
        RequestValues<string, int> r = Request.CreateValues("A", 1);
        PolicyValues<string, int> p = Policy.CreateValues("A", 1);
        interpreter.SetFunction("equal", new Func<string, string, bool>(
            (a, b) => a == b)
        );
        interpreter.SetFunction("equal", new Func<int, int, bool>(
            (a, b) => a == b)
        );

        Func<RequestValues<string, int>, PolicyValues<string, int>, bool> func1 =
            ExpressionUtil.Compile(interpreter, "equal(r.Value2, p.Value2) && equal(r.Value2, p.Value2)",
                nameof(r), in r, nameof(p), in p);

        Assert.True(func1(Request.CreateValues("A", 1), Policy.CreateValues("A", 1)));
        Assert.False(func1(Request.CreateValues("A", 1), Policy.CreateValues("A", 2)));
        Assert.False(func1(Request.CreateValues("B", 1), Policy.CreateValues("B", 2)));
    }

#if !NET452
    [Fact]
    public void TestGenericFunctionModel()
    {
        Enforcer e = new Enforcer(DefaultModel.NewModelFromText(
            """
                [request_definition]
                r = obj1, obj2

                [policy_definition]
                p = _

                [policy_effect]
                e = some(where (p.eft == allow))

                [matchers]
                m = max(r.obj1, r.obj2) > 2
            """));

        e.AddFunction("max", new Func<int, int, int>(
            // ReSharper disable once ConvertClosureToMethodGroup
            (a, b) => Math.Max(a, b)
        ));
        Assert.True(e.Enforce(1, 3));
        Assert.False(e.Enforce(1, 2));
        Assert.False(e.Enforce("1", "111"));

        e.AddFunction("max", new Func<string, string, int>(
            (a, b) => Math.Max(a.Length, b.Length)
        ));
        Assert.True(e.Enforce(1, 3));
        Assert.False(e.Enforce(1, 2));
        Assert.True(e.Enforce("1", "111"));
    }
#endif

}
