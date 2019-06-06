using NetCasbin.Util;
using Xunit;

namespace NetCasbin.Test
{
    public class UtilsTest
    {
        [Fact]
        public void Test_EscapeAssertion()
        {
            Assert.Equal("r_attr.value == p_attr", Utility.EscapeAssertion("r.attr.value == p.attr"));
            Assert.Equal("r_attp.value || p_attr", Utility.EscapeAssertion("r.attp.value || p.attr"));
            Assert.Equal("r_attp.value &&p_attr", Utility.EscapeAssertion("r.attp.value &&p.attr"));
            Assert.Equal("r_attp.value >p_attr", Utility.EscapeAssertion("r.attp.value >p.attr"));
            Assert.Equal("r_attp.value <p_attr", Utility.EscapeAssertion("r.attp.value <p.attr"));
            Assert.Equal("r_attp.value -p_attr", Utility.EscapeAssertion("r.attp.value -p.attr"));
            Assert.Equal("r_attp.value +p_attr", Utility.EscapeAssertion("r.attp.value +p.attr"));
            Assert.Equal("r_attp.value *p_attr", Utility.EscapeAssertion("r.attp.value *p.attr"));
            Assert.Equal("r_attp.value /p_attr", Utility.EscapeAssertion("r.attp.value /p.attr"));
            Assert.Equal("!r_attp.value /p_attr", Utility.EscapeAssertion("!r.attp.value /p.attr"));
            Assert.Equal("g(r_sub, p_sub) == p_attr", Utility.EscapeAssertion("g(r.sub, p.sub) == p.attr"));
            Assert.Equal("g(r_sub,p_sub) == p_attr", Utility.EscapeAssertion("g(r.sub,p.sub) == p.attr"));
            Assert.Equal("(r_attp.value || p_attr)p_u", Utility.EscapeAssertion("(r.attp.value || p.attr)p.u"));
        }

        [Fact]
        public void Test_RemoveComments()
        {
            Assert.Equal("r.act == p.act", Utility.RemoveComments("r.act == p.act # comments"));
            Assert.Equal("r.act == p.act", Utility.RemoveComments("r.act == p.act#comments"));
            Assert.Equal("r.act == p.act", Utility.RemoveComments("r.act == p.act###"));
            Assert.Equal("", Utility.RemoveComments("### comments"));
            Assert.Equal("r.act == p.act", Utility.RemoveComments("r.act == p.act"));
        }
    }
}
