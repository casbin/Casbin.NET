using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NetCasbin.Test
{
    public class BuiltInOperatorTest
    {
        [Fact]
        public void Test_IpMatch()
        {
            Assert.True(BuiltInFunctions.IPMatch("192.168.2.123", "192.168.2.0/24"));
            Assert.False(BuiltInFunctions.IPMatch("192.168.2.123", "192.168.2.0/25"));
            Assert.False(BuiltInFunctions.IPMatch("192.168.2.123", "192.168.2.0"));
        }
    }
}
