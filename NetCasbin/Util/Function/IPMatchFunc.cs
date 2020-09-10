using System;
using System.Net;
using NetCasbin.Abstractions;

namespace NetCasbin.Util.Function
{
    public class IPMatchFunc : AbstractFunction
    {
        protected override Delegate GetFunc()
        {
            Func<string, string, bool> call = BuiltInFunctions.IPMatch;
            return call;
        }

        public IPMatchFunc() : base("ipMatch")
        {

        }
    }
}
