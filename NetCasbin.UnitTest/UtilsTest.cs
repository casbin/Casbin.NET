using System;
using Xunit;

namespace NetCasbin.Test
{
    public class UtilsTest
    {
        [Fact]
        public void Test_EscapeAssertion()
        {
            Assert.Equal("r_attr.value == p_attr", Util.EscapeAssertion("r.attr.value == p.attr"));
            Assert.Equal("r_attp.value || p_attr", Util.EscapeAssertion("r.attp.value || p.attr"));
            Assert.Equal("r_attp.value &&p_attr", Util.EscapeAssertion("r.attp.value &&p.attr"));
            Assert.Equal("r_attp.value >p_attr", Util.EscapeAssertion("r.attp.value >p.attr"));
            Assert.Equal("r_attp.value <p_attr", Util.EscapeAssertion("r.attp.value <p.attr"));
            Assert.Equal("r_attp.value -p_attr", Util.EscapeAssertion("r.attp.value -p.attr"));
            Assert.Equal("r_attp.value +p_attr", Util.EscapeAssertion("r.attp.value +p.attr"));
            Assert.Equal("r_attp.value *p_attr", Util.EscapeAssertion("r.attp.value *p.attr"));
            Assert.Equal("r_attp.value /p_attr", Util.EscapeAssertion("r.attp.value /p.attr"));
            Assert.Equal("!r_attp.value /p_attr", Util.EscapeAssertion("!r.attp.value /p.attr"));
            Assert.Equal("g(r_sub, p_sub) == p_attr", Util.EscapeAssertion("g(r.sub, p.sub) == p.attr"));
            Assert.Equal("g(r_sub,p_sub) == p_attr", Util.EscapeAssertion("g(r.sub,p.sub) == p.attr"));
            Assert.Equal("(r_attp.value || p_attr)p_u", Util.EscapeAssertion("(r.attp.value || p.attr)p.u"));
        }

        [Fact]
        public void Test_RemoveComments()
        {
            Assert.Equal("r.act == p.act", Util.RemoveComments("r.act == p.act # comments"));
            Assert.Equal("r.act == p.act", Util.RemoveComments("r.act == p.act#comments"));
            Assert.Equal("r.act == p.act", Util.RemoveComments("r.act == p.act###"));
            Assert.Equal("", Util.RemoveComments("### comments"));
            Assert.Equal("r.act == p.act", Util.RemoveComments("r.act == p.act"));
        }
    }
}
