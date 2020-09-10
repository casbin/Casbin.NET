using System;
using NetCasbin.Abstractions;

namespace NetCasbin.Util.Function
{
    public class KeyMatch3Func : AbstractFunction
    {
        public KeyMatch3Func() : base("keyMatch3")
        {

        }

        protected override Delegate GetFunc()
        {
            Func<string, string, bool> call = BuiltInFunctions.KeyMatch3;
            return call;
        }
    }
}
