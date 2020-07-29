using System;
using NetCasbin.Abstractions;

namespace NetCasbin.Util.Function
{
    public class KeyMatch2Func : AbstractFunction
    {
        protected override Delegate GetFunc()
        {
            Func<string, string, bool> call = BuiltInFunctions.KeyMatch2;
            return call;
        }

        public KeyMatch2Func() : base("keyMatch2")
        {

        }
    }
}
