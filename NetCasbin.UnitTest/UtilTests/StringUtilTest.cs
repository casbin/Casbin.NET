using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Casbin.Util;
using Xunit;

namespace Casbin.UnitTests.UtilTests
{
    public class StringUtilTest
    {
        public static IEnumerable<object[]> EscapeAssertionTestData = new[]
        {
            new object[] {"r_attr.value == p_attr", "r.attr.value == p.attr"},
            new object[] {"r_attp.value || p_attr", "r.attp.value || p.attr"},
            new object[] {"r_attp.value &&p_attr", "r.attp.value &&p.attr"},
            new object[] {"r_attp.value >p_attr", "r.attp.value >p.attr"},
            new object[] {"r_attp.value <p_attr", "r.attp.value <p.attr"},
            new object[] {"r_attp.value -p_attr", "r.attp.value -p.attr"},
            new object[] {"r_attp.value +p_attr", "r.attp.value +p.attr"},
            new object[] {"r_attp.value *p_attr", "r.attp.value *p.attr"},
            new object[] {"r_attp.value /p_attr", "r.attp.value /p.attr"},
            new object[] {"!r_attp.value /p_attr", "!r.attp.value /p.attr"},
            new object[] {"g(r_sub, p_sub) == p_attr", "g(r.sub, p.sub) == p.attr"},
            new object[] {"g(r_sub,p_sub) == p_attr", "g(r.sub,p.sub) == p.attr"},
            new object[] {"(r_attp.value || p_attr)p_u", "(r.attp.value || p.attr)p.u"}
        };

        [Theory]
        [MemberData(nameof(EscapeAssertionTestData))]
        public void TestEscapeAssertion(string except, string actual)
        {
            Assert.Equal(except, StringUtil.EscapeAssertion(actual));

        }

        public static IEnumerable<object[]> RemoveCommentsTestData = new[]
        {
            new object[] {"r.act == p.act", "r.act == p.act # comments" },
            new object[] {"r.act == p.act", "r.act == p.act#comments" },
            new object[] {"r.act == p.act", "r.act == p.act###" },
            new object[] {"", "### comments" },
            new object[] {"r.act == p.act", "r.act == p.act" },
        };

        [Theory]
        [MemberData(nameof(RemoveCommentsTestData))]
        public void TestRemoveComments(string except, string actual)
        {
            Assert.Equal(except, StringUtil.RemoveComments(actual));
        }

        public static IEnumerable<object[]> ReplaceEvalTestData = new[]
        {
            new object[] {"eval(rule1)", "a == b",
                new Dictionary<string, string>{["rule1"] = "a == b"}},
            new object[] {"eval(rule1) && c && d", "a == b && c && d",
                new Dictionary<string, string>{["rule1"] = "a == b"}},

            new object[] {"eval(rule1)", "eval(rule1)",
                null},
            new object[] {"eval(rule1) && c && d", "eval(rule1) && c && d",
                null},

            new object[] {"eval(rule1) || eval(rule2)", "a == b || a == c",
                new Dictionary<string, string>{["rule1"] = "a == b", ["rule2"] = "a == c"}},
            new object[] {"eval(rule1) || eval(rule2) && c && d", "a == b || a == c && c && d",
                new Dictionary<string, string>{["rule1"] = "a == b", ["rule2"] = "a == c"}},

            new object[] {"eval(rule1) || eval(rule2)", "a == b || eval(rule2)",
                new Dictionary<string, string>{["rule1"] = "a == b"}},
            new object[] {"eval(rule1) || eval(rule2) && c && d", "a == b || eval(rule2) && c && d",
                new Dictionary<string, string>{["rule1"] = "a == b"}},

            new object[] {"eval(rule1) || eval(rule2)", "eval(rule1) || a == c",
                new Dictionary<string, string>{["rule2"] = "a == c"}},
            new object[] {"eval(rule1) || eval(rule2) && c && d", "eval(rule1) || a == c && c && d",
                new Dictionary<string, string>{["rule2"] = "a == c"}},

            new object[] {"eval(rule1) || eval(rule2)", "eval(rule1) || eval(rule2)",
                null},
            new object[] {"eval(rule1) || eval(rule2) && c && d", "eval(rule1) || eval(rule2) && c && d",
                null}
        };

        [Theory]
        [MemberData(nameof(ReplaceEvalTestData))]
        public void TestReplaceEval(
            string oldExpressionString,
            string newExpressionString,
            IDictionary<string, string> rules)
        {
            Assert.Equal(newExpressionString, StringUtil.ReplaceEval(oldExpressionString, rules));
        }
    }
}
