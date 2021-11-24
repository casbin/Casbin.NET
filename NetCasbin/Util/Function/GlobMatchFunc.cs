using System;
using System.Net;
using NetCasbin.Abstractions;

namespace NetCasbin.Util.Function
{
    public class GlobMatchFunc : AbstractFunction
    {
        protected override Delegate GetFunc()
        {
            Func<string, string, bool> call = BuiltInFunctions.GlobMatch;
            return call;
        }

        public GlobMatchFunc() : base("globMatch")
        {

        }
    }
}
