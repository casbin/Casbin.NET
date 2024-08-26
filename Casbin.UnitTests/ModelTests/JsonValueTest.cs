#if !NET452 && !NET461 && !NET462
using System;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Text.Json;
using DynamicExpresso;
using Xunit;

namespace Casbin.UnitTests.ModelTests;

public class JsonValueTest
{
    [Fact]
    public void GetJsonValueTest()
    {
        string json = "{\"name\":\"John\",\"age\":30,\"car\":null}";
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var interpreter = new Interpreter();
        interpreter.SetVariable("obj", new JsonValue(root));
        object result = interpreter.Eval("obj.name");
        Assert.Equal("John", result);

        string arrayJson = "[{\"name\":\"John\"},{\"name\":\"Doe\"}]";
        using var arrayDoc = JsonDocument.Parse(arrayJson);
        var arrayRoot = arrayDoc.RootElement;

        interpreter.SetVariable("array", new JsonValue(arrayRoot));
        object arrayResult = interpreter.Eval("array[0].name");
        Assert.Equal("John", arrayResult);
    }

    public class JsonValue : DynamicObject
    {
        private readonly JsonElement _element;

        public JsonValue(JsonElement element)
        {
            _element = element;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_element.ValueKind != JsonValueKind.Object)
            {
                result = null;
                return false;
            }

            if (_element.TryGetProperty(binder.Name, out var value))
            {
                result = GetValue(value);
                return true;
            }

            result = null;
            return false;
        }

        public object this[int index] => GetValue(_element[index]);

        public override string ToString()
        {
            return _element.ToString();
        }

        private object GetValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Object => new JsonValue(element),
                JsonValueKind.Array => new JsonValue(element),
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.GetInt32(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                JsonValueKind.Undefined => null,
                _ => throw new InvalidOperationException(),
            };
        }

    }

}
#endif


