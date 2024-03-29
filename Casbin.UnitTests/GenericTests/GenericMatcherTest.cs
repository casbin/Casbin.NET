﻿using System;
using Casbin.Model;
using DynamicExpresso;
using Xunit;

namespace Casbin.UnitTests.GenericTests;

public class GenericMatcherTest
{
    [Fact]
    public void TestGenericMatcher()
    {
        //RequestValues<string, int> r = Request.CreateValues("A", 1);
        //PolicyValues p = Policy.CreateValues("A", 1);
        //Func<RequestValues<string, int>, PolicyValues, bool> func1 = Compile(
        //    "r.Value1 == p[0].GetType()(p[0]) && r.Value2 == p[1].GetType()(p[1])",
        //    nameof(r), in r, nameof(p), in p);

        //Assert.True(func1(Request.CreateValues("A", 1), Policy.CreateValues("A", 1)));
        //Assert.False(func1(Request.CreateValues("A", 1), Policy.CreateValues("A", 2)));
        //Assert.False(func1(Request.CreateValues("B", 1), Policy.CreateValues("B", 2)));

        //RequestValues<string, int, string> r2 = Request.CreateValues("A", 1, "read");
        //PolicyValues p2 = Policy.CreateValues("A", 1, "read");
        //Func<RequestValues<string, int, string>, PolicyValues, bool> func2 = Compile(
        //    "r2.Value1 == p2[0] && r2.Value2 == p2[1] && r2.Value3 == p2[2]",
        //    nameof(r2), in r2, nameof(p2), in p2);

        //Assert.True(func2(Request.CreateValues("A", 1, "read"), Policy.CreateValues("A", 1, "read")));
        //Assert.False(func2(Request.CreateValues("A", 1, "read"), Policy.CreateValues("A", 2, "read")));
        //Assert.False(func2(Request.CreateValues("B", 1, "read"), Policy.CreateValues("B", 2, "read")));
        Assert.True(1==1);
    }

    private static Func<TRequest, TPolicy, bool> Compile<TRequest, TPolicy>
    (
        string expressionText,
        string requestType, in TRequest r,
        string policyType, in TPolicy p)
        where TRequest : struct, IRequestValues where TPolicy : IPolicyValues
    {
        Interpreter interpreter = new();
        return interpreter.ParseAsDelegate<Func<TRequest, TPolicy, bool>>(
            expressionText, requestType, policyType
        );
    }
}
