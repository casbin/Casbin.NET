using System;
using System.Collections.Generic;
using DynamicExpresso;
using NetCasbin.Util;
using Xunit;

namespace NetCasbin.UnitTest
{
    public class UtilityTest
    {
        private delegate bool GFunction(string arg = null);

        [Fact]
        public void TestParseGFunction()
        {
            static bool GetGFunction(string arg = null)
            {
                return arg is not null;
            };

            var interpreter = new Interpreter();
            interpreter.SetFunction("GFunction", (GFunction) GetGFunction);
            interpreter.SetVariable("arg", "arg");

            Assert.True((bool) interpreter.Eval("GFunction(arg)"));
            Assert.False((bool) interpreter.Eval("GFunction()"));
        }

        [Fact]
        public void TestEscapeAssertion()
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
        public void TestRemoveComments()
        {
            Assert.Equal("r.act == p.act", Utility.RemoveComments("r.act == p.act # comments"));
            Assert.Equal("r.act == p.act", Utility.RemoveComments("r.act == p.act#comments"));
            Assert.Equal("r.act == p.act", Utility.RemoveComments("r.act == p.act###"));
            Assert.Equal("", Utility.RemoveComments("### comments"));
            Assert.Equal("r.act == p.act", Utility.RemoveComments("r.act == p.act"));
        }

        public static IEnumerable<object[]> replaceEvalTestData = new[]
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
        [MemberData(nameof(replaceEvalTestData))]
        public void TestReplaceEval(
            string oldExpressionString,
            string newExpressionString,
            IDictionary<string, string> rules)
        {
            Assert.Equal(newExpressionString, Utility.ReplaceEval(oldExpressionString, rules));
        }
    }
}
