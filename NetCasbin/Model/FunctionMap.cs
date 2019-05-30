using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NetCasbin
{
    public class FunctionMap
    {
        private IDictionary<string, AbstractFunction> _fm;

        public IDictionary<string, AbstractFunction> FunctionDict => _fm;

        public void AddFunction(String name, AbstractFunction function)
        {
            _fm.Add(name, function);
        }

        public static FunctionMap LoadFunctionMap()
        {
            FunctionMap fm = new FunctionMap();
            fm._fm = new Dictionary<string, AbstractFunction>();
            fm.AddFunction("keyMatch", new KeyMatchFunc());
            fm.AddFunction("keyMatch2", new KeyMatch2Func());
            fm.AddFunction("regexMatch", new RegexMatchFunc());
            fm.AddFunction("ipMatch", new IPMatchFunc());
            return fm;
        }
    }
}
