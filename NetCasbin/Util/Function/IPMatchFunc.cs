using System;
using NetCasbin.Abstractions;

namespace NetCasbin.Util.Function
{
    public class IpMatchFunc : AbstractFunction
    {
        protected override Delegate GetFunc()
        {
            Func<string, string, bool> call = BuiltInFunctions.IpMatch;
            return call;
        }

        public IpMatchFunc() : base("ipMatch")
        {

        }
    }
}
