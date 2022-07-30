using DynamicExpresso;
using Xunit;

namespace Casbin.UnitTests.UtilTests;

public class UtilityTest
{
    [Fact]
    public void TestParseGFunction()
    {
        static bool GetGFunction(string arg = null)
        {
            return arg is not null;
        }

        ;

        Interpreter interpreter = new();
        interpreter.SetFunction("GFunction", (GFunction)GetGFunction);
        interpreter.SetVariable("arg", "arg");

        Assert.True((bool)interpreter.Eval("GFunction(arg)"));
        Assert.False((bool)interpreter.Eval("GFunction()"));
    }

    private delegate bool GFunction(string arg = null);
}
