﻿using System.Collections.Generic;
using Casbin.Util;
using Xunit;

namespace Casbin.UnitTests.UtilTests;

public class BuiltInFunctionTest
{
    public static IEnumerable<object[]> ipMatchTestData = new[]
    {
        new object[] { "192.168.2.123", "192.168.2.0/24", true },
        new object[] { "192.168.2.123", "192.168.3.0/24", false },
        new object[] { "192.168.2.123", "192.168.2.0/16", true },
        new object[] { "192.168.2.123", "192.168.2.123", true },
        new object[] { "192.168.2.123", "192.168.2.123/32", true },
        new object[] { "10.0.0.11", "10.0.0.0/8", true }, new object[] { "11.0.0.123", "10.0.0.0/8", false },
        new object[] { "2001:db8::1", "2001:db8::1", true }, new object[] { "2001:db8::1", "2001:db9::1", false },
        new object[] { "2001:db8::1", "2001:db8::1/128", true },
        new object[] { "2001:db8::1", "2001:db9::/64", false }
    };

    public static IEnumerable<object[]> regexMatchTestData = new[]
    {
        new object[] { "/topic/create", "/topic/create", true },
        new object[] { "/topic/create/123", "/topic/create", true },
        new object[] { "/topic/delete", "/topic/create", false },
        new object[] { "/topic/edit", "/topic/edit/[0-9]+", false },
        new object[] { "/topic/edit/123", "/topic/edit/[0-9]+", true },
        new object[] { "/topic/edit/abc", "/topic/edit/[0-9]+", false },
        new object[] { "/foo/delete/123", "/topic/delete/[0-9]+", false },
        new object[] { "/topic/delete/0", "/topic/delete/[0-9]+", true },
        new object[] { "/topic/edit/123s", "/topic/delete/[0-9]+", false }
    };

    public static IEnumerable<object[]> KeyGetTestData = new[]
    {
        new object[] { "/foo", "/foo", "" },
        new object[] { "/foo", "/foo*", "" },
        new object[] { "/foo", "/foo/*", "" },
        new object[] { "/foo/bar", "/foo", "" },
        new object[] { "/foo/bar", "/foo*", "/bar" },
        new object[] { "/foo/bar", "/foo/*", "bar" },
        new object[] { "/foobar", "/foo", "" },
        new object[] { "/foobar", "/foo*", "bar" },
        new object[] { "/foobar", "/foo/*", "" }
    };

    public static IEnumerable<object[]> KeyGet2TestData = new[]
    {
        new object[] { "/foo", "/foo", "id", ""},
        new object[] { "/foo", "/foo*", "id", ""},
        new object[] { "/foo", "/foo/*", "id", ""},
        new object[] { "/foo/bar", "/foo", "id", ""},
        new object[] { "/foo/bar", "/foo*", "id", ""}, // different with KeyMatch.
        new object[] { "/foo/bar", "/foo/*", "id", ""},
        new object[] { "/foobar", "/foo", "id", ""},
        new object[] { "/foobar", "/foo*", "id", ""}, // different with KeyMatch.
        new object[] { "/foobar", "/foo/*", "id", ""},

        new object[] { "/", "/:resource", "resource", ""},
        new object[] { "/resource1", "/:resource", "resource", "resource1"},
        new object[] { "/myid", "/:id/using/:resId", "id", ""},
        new object[] { "/myid/using/myresid", "/:id/using/:resId", "id", "myid"},
        new object[] { "/myid/using/myresid", "/:id/using/:resId", "resId", "myresid"},

        new object[] { "/proxy/myid", "/proxy/:id/*", "id", ""},
        new object[] { "/proxy/myid/", "/proxy/:id/*", "id", "myid"},
        new object[] { "/proxy/myid/res", "/proxy/:id/*", "id", "myid"},
        new object[] { "/proxy/myid/res/res2", "/proxy/:id/*", "id", "myid"},
        new object[] { "/proxy/myid/res/res2/res3", "/proxy/:id/*", "id", "myid"},
        new object[] { "/proxy/myid/res/res2/res3", "/proxy/:id/res/*", "id", "myid"},
        new object[] { "/proxy/", "/proxy/:id/*", "id", ""},

        new object[] { "/alice", "/:id", "id", "alice"},
        new object[] { "/alice/all", "/:id/all", "id", "alice"},
        new object[] { "/alice", "/:id/all", "id", ""},
        new object[] { "/alice/all", "/:id", "id", ""},

        new object[] { "/alice/all", "/:/all", "", ""}
    };

    public static IEnumerable<object[]> keyMatchTestData = new[]
    {
        new object[] { "/foo", "/foo", true }, new object[] { "/foo", "/foo*", true },
        new object[] { "/foo", "/foo/*", false }, new object[] { "/foo/bar", "/foo", false },
        new object[] { "/foo/bar", "/foo*", true }, new object[] { "/foo/bar", "/foo/*", true },
        new object[] { "/foobar", "/foo", false }, new object[] { "/foobar", "/foo*", true },
        new object[] { "/foobar", "/foo/*", false }
    };

    public static IEnumerable<object[]> KeyMatch2TestData = new[]
    {
        new object[] { "/foo", "/foo", true }, new object[] { "/foo", "/foo*", true },
        new object[] { "/foo", "/foo/*", false }, new object[] { "/foo/bar", "/foo", false },
        new object[] { "/foo/bar", "/foo*", false }, // different with KeyMatch.
        new object[] { "/foo/bar", "/foo/*", true }, new object[] { "/foobar", "/foo", false },
        new object[] { "/foobar", "/foo*", false }, // different with KeyMatch.
        new object[] { "/foobar", "/foo/*", false }, new object[] { "/", "/:resource", false },
        new object[] { "/resource1", "/:resource", true }, new object[] { "/myid", "/:id/using/:resId", false },
        new object[] { "/myid/using/myresid", "/:id/using/:resId", true },
        new object[] { "/proxy/myid", "/proxy/:id/*", false }, new object[] { "/proxy/myid/", "/proxy/:id/*", true },
        new object[] { "/proxy/myid/res", "/proxy/:id/*", true },
        new object[] { "/proxy/myid/res/res2", "/proxy/:id/*", true },
        new object[] { "/proxy/myid/res/res2/res3", "/proxy/:id/*", true },
        new object[] { "/proxy/", "/proxy/:id/*", false }, new object[] { "/alice", "/:id", true },
        new object[] { "/alice/all", "/:id/all", true }, new object[] { "/alice", "/:id/all", false },
        new object[] { "/alice/all", "/:id", false }, new object[] { "/alice/all", "/:/all", false }
    };

    public static IEnumerable<object[]> KeyMatch3TestData = new[]
    {
        // keyMatch3(}, is similar with KeyMatch2(},, except using "/proxy/{id}" instead of "/proxy/:id".
        new object[] { "/foo", "/foo", true }, new object[] { "/foo", "/foo*", true },
        new object[] { "/foo", "/foo/*", false }, new object[] { "/foo/bar", "/foo", false },
        new object[] { "/foo/bar", "/foo*", false }, new object[] { "/foo/bar", "/foo/*", true },
        new object[] { "/foobar", "/foo", false }, new object[] { "/foobar", "/foo*", false },
        new object[] { "/foobar", "/foo/*", false }, new object[] { "/", "/{resource}", false },
        new object[] { "/resource1", "/{resource}", true }, new object[] { "/myid", "/{id}/using/{resId}", false },
        new object[] { "/myid/using/myresid", "/{id}/using/{resId}", true },
        new object[] { "/proxy/myid", "/proxy/{id}/*", false }, new object[] { "/proxy/myid/", "/proxy/{id}/*", true },
        new object[] { "/proxy/myid/res", "/proxy/{id}/*", true },
        new object[] { "/proxy/myid/res/res2", "/proxy/{id}/*", true },
        new object[] { "/proxy/myid/res/res2/res3", "/proxy/{id}/*", true },
        new object[] { "/proxy/", "/proxy/{id}/*", false },
        new object[] { "/myid/using/myresid", "/{id/using/{resId}", false }
    };

    public static IEnumerable<object[]> KeyMatch4TestData = new[]
    {
        new object[] { "/parent/123/child/123", "/parent/{id}/child/{id}", true },
        new object[] { "/parent/123/child/123", "/parent/{i/d}/child/{i/d}", false },
        new object[] { "/parent/123/child/456", "/parent/{id}/child/{id}", false },
        new object[] { "/parent/123/child/123", "/parent/{id}/child/{another_id}", true },
        new object[] { "/parent/123/child/456", "/parent/{id}/child/{another_id}", true },
        new object[] { "/parent/123/child/123/book/123", "/parent/{id}/child/{id}/book/{id}", true },
        new object[] { "/parent/123/child/123/book/456", "/parent/{id}/child/{id}/book/{id}", false },
        new object[] { "/parent/123/child/456/book/123", "/parent/{id}/child/{id}/book/{id}", false },
        new object[] { "/parent/123/child/456/book/", "/parent/{id}/child/{id}/book/{id}", false },
        new object[] { "/parent/123/child/456", "/parent/{id}/child/{id}/book/{id}", false }
    };

    public static IEnumerable<object[]> KeyMatch5TestData = new[]
    {
        new object[] { "/parent/child?status=1&type=2", "/parent/child", true },
        new object[] { "/parent?status=1&type=2", "/parent/child", false },
        new object[] { "/parent/child/?status=1&type=2", "/parent/child/", true },
        new object[] { "/parent/child/?status=1&type=2", "/parent/child", false },
        new object[] { "/parent/child?status=1&type=2", "/parent/child/", false }
    };

    public static IEnumerable<object[]> GlobMatchTestData = new[]
    {
        new object[] { "/foo", "/foo", true }, new object[] { "/foo", "/foo*", true },
        new object[] { "/foo", "/foo/*", false }, new object[] { "/foo/bar", "/foo", false },
        new object[] { "/foo/bar", "/foo*", false }, new object[] { "/foo/bar", "/foo/*", true },
        new object[] { "/foobar", "/foo", false }, new object[] { "/foobar", "/foo*", true },
        new object[] { "/foobar", "/foo/*", false }, new object[] { "/foo", "*/foo", true },
        new object[] { "/foo", "*/foo*", true }, new object[] { "/foo", "*/foo/*", false },
        new object[] { "/foo/bar", "*/foo", false }, new object[] { "/foo/bar", "*/foo*", false },
        new object[] { "/foo/bar", "*/foo/*", true }, new object[] { "/foobar", "*/foo", false },
        new object[] { "/foobar", "*/foo*", true }, new object[] { "/foobar", "*/foo/*", false },
        new object[] { "/prefix/foo", "*/foo", false }, new object[] { "/prefix/foo", "*/foo*", false },
        new object[] { "/prefix/foo", "*/foo/*", false }, new object[] { "/prefix/foo/bar", "*/foo", false },
        new object[] { "/prefix/foo/bar", "*/foo*", false }, new object[] { "/prefix/foo/bar", "*/foo/*", false },
        new object[] { "/prefix/foobar", "*/foo", false }, new object[] { "/prefix/foobar", "*/foo*", false },
        new object[] { "/prefix/foobar", "*/foo/*", false },
        new object[] { "/prefix/subprefix/foo", "*/foo", false },
        new object[] { "/prefix/subprefix/foo", "*/foo*", false },
        new object[] { "/prefix/subprefix/foo", "*/foo/*", false },
        new object[] { "/prefix/subprefix/foo/bar", "*/foo", false },
        new object[] { "/prefix/subprefix/foo/bar", "*/foo*", false },
        new object[] { "/prefix/subprefix/foo/bar", "*/foo/*", false },
        new object[] { "/prefix/subprefix/foobar", "*/foo", false },
        new object[] { "/prefix/subprefix/foobar", "*/foo*", false },
        new object[] { "/prefix/subprefix/foobar", "*/foo/*", false }
    };

    [Theory]
    [MemberData(nameof(ipMatchTestData))]
    public void TestIPMatch(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.IPMatch(key1, key2));

    [Theory]
    [MemberData(nameof(regexMatchTestData))]
    public void TestRegexMatch(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.RegexMatch(key1, key2));

    [Theory]
    [MemberData(nameof(KeyGetTestData))]
    public void TestKeyGet(string key1, string key2, string expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyGet(key1, key2));

    [Theory]
    [MemberData(nameof(KeyGet2TestData))]
    public void TestKeyGet2(string key1, string key2, string pathVar, string expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyGet2(key1, key2, pathVar));

    [Theory]
    [MemberData(nameof(keyMatchTestData))]
    public void TestKeyMatch(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyMatch(key1, key2));

    [Theory]
    [MemberData(nameof(KeyMatch2TestData))]
    public void TestKeyMatch2(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyMatch2(key1, key2));

    [Theory]
    [MemberData(nameof(KeyMatch3TestData))]
    public void TestKeyMatch3(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyMatch3(key1, key2));

    [Theory]
    [MemberData(nameof(KeyMatch4TestData))]
    public void TestKeyMatch4(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyMatch4(key1, key2));

    [Theory]
    [MemberData(nameof(KeyMatch5TestData))]
    public void TestKeyMatch5(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyMatch5(key1, key2));

    [Theory]
    [MemberData(nameof(GlobMatchTestData))]
    public void TestGlobMatch(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.GlobMatch(key1, key2));
}
