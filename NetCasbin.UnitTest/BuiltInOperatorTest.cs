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
    }
}
