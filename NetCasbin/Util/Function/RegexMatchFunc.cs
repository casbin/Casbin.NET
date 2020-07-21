using System;

namespace NetCasbin.Util.Function
{
    public class RegexMatchFunc : AbstractFunction
    {
        protected override Delegate GetFunc()
        {
            Func<string, string, bool> call = BuiltInFunctions.RegexMatch;
            return call;
        }

        public RegexMatchFunc() : base("regexMatch")
        {

        }
    }
}
