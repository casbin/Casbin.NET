#if !NET452 && !NET461 && !NET462
using Casbin.Model;
using DynamicExpresso;
using Xunit;

namespace Casbin.UnitTests.ModelTests;

public class JsonValueTest
{
    [Fact]
    public void GetJsonValueTest()
    {
        string json = "{\"name\":\"John\",\"age\":30,\"car\":null}";

        var interpreter = new Interpreter();
        interpreter.SetVariable("obj", new JsonValue(json));
        object result = interpreter.Eval("obj.name");
        Assert.Equal("John", result);

        string arrayJson = "[{\"name\":\"John\"},{\"name\":\"Doe\"}]";

        interpreter.SetVariable("array", new JsonValue(arrayJson));
        object arrayResult = interpreter.Eval("array[0].name");
        Assert.Equal("John", arrayResult);
    }
}
#endif


