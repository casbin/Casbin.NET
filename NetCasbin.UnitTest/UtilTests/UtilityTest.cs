using DynamicExpresso;
using Xunit;

namespace Casbin.UnitTests
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
    }
}
