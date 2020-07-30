using System.Collections.Generic;
using NetCasbin.Util;
using Xunit;

namespace NetCasbin.UnitTest
{
    public class BuiltInOperatorTest
    {
        [Fact]
        public void TestIpMatch()
        {
            Assert.True(BuiltInFunctions.IpMatch("192.168.2.123", "192.168.2.0/24"));
            Assert.False(BuiltInFunctions.IpMatch("192.168.2.123", "192.168.2.0/25"));
            Assert.False(BuiltInFunctions.IpMatch("192.168.2.123", "192.168.2.0"));
        }

        public static IEnumerable<object[]> KeyMatch4TestData = new[]
        {
            new object[] {"/parent/123/child/123", "/parent/{id}/child/{id}", true},
            new object[] {"/parent/123/child/123", "/parent/{i/d}/child/{i/d}", false},

            new object[] {"/parent/123/child/456", "/parent/{id}/child/{id}", false},
            new object[] {"/parent/123/child/123", "/parent/{id}/child/{another_id}", true},
            new object[] {"/parent/123/child/456", "/parent/{id}/child/{another_id}", true},

            new object[] {"/parent/123/child/123/book/123", "/parent/{id}/child/{id}/book/{id}", true},
            new object[] {"/parent/123/child/123/book/456", "/parent/{id}/child/{id}/book/{id}", false},
            new object[] {"/parent/123/child/456/book/123", "/parent/{id}/child/{id}/book/{id}", false},

            new object[] {"/parent/123/child/456/book/", "/parent/{id}/child/{id}/book/{id}", false},
            new object[] {"/parent/123/child/456", "/parent/{id}/child/{id}/book/{id}", false}
        };

        [Theory]
        [MemberData(nameof(KeyMatch4TestData))]
        public void TestKeyMatch4(string key1, string key2, bool exceptResult)
        {
            Assert.Equal(exceptResult,
                BuiltInFunctions.KeyMatch4(key1, key2));
        }
    }
}
