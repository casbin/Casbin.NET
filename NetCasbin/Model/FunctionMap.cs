using System.Collections.Generic;
using NetCasbin.Abstractions;
using NetCasbin.Util.Function;

namespace NetCasbin.Model
{
    public class FunctionMap
    {
        public IDictionary<string, AbstractFunction> FunctionDict { get; private set; }

        public void AddFunction(string name, AbstractFunction function)
        {
            FunctionDict.Add(name, function);
        }

        public static FunctionMap LoadFunctionMap()
        {
            var map = new FunctionMap
            {
                FunctionDict = new Dictionary<string, AbstractFunction>()
            };

            map.AddFunction("keyMatch", new KeyMatchFunc());
            map.AddFunction("keyMatch2", new KeyMatch2Func());
            map.AddFunction("keyMatch3", new KeyMatch3Func());
            map.AddFunction("keyMatch4", new KeyMatch4Func());
            map.AddFunction("regexMatch", new RegexMatchFunc());
            map.AddFunction("ipMatch", new IPMatchFunc());
            return map;
        }
    }
}
