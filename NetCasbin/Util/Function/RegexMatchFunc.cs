using System;

namespace NetCasbin.Util.Function
{
    public class RegexMatchFunc : AbstractFunction
    {
        protected override Delegate GetFunc()
        {
            Func<string, string, bool> call = (arg1, arg2) =>
            {
                return BuiltInFunctions.RegexMatch(arg1, arg2);
            };
            return call;
        }

        public RegexMatchFunc() : base("regexMatch")
        {

        }
    }
}
