using System;

namespace NetCasbin.Util.Function
{
    public class IPMatchFunc : AbstractFunction
    {
        protected override Delegate GetFunc()
        {
            Func<string, string, bool> call = (arg1, arg2) =>
            {
                return BuiltInFunctions.IPMatch(arg1, arg2);
            };
            return call;
        }

        public IPMatchFunc() : base("ipMatch")
        {

        }
    }
}
