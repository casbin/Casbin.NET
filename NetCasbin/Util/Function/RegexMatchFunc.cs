using System;
using NetCasbin.Abstractions;

namespace NetCasbin.Util.Function
{
    public class RegexMatchFunc : AbstractFunction
    {
        public RegexMatchFunc() : base("regexMatch")
        {

        }

        protected override Delegate GetFunc()
        {
            Func<string, string, bool> call = BuiltInFunctions.RegexMatch;
            return call;
        }
    }
}
