using System.Collections.Generic;
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
            var fm = new FunctionMap { FunctionDict = new Dictionary<string, AbstractFunction>() };
            fm.AddFunction("keyMatch", new KeyMatchFunc());
            fm.AddFunction("keyMatch2", new KeyMatch2Func());
            fm.AddFunction("regexMatch", new RegexMatchFunc());
            fm.AddFunction("ipMatch", new IpMatchFunc());
            return fm;
        }
    }
}
