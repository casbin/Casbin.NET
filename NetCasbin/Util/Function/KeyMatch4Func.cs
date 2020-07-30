using System;
using NetCasbin.Abstractions;

namespace NetCasbin.Util.Function
{
    public sealed class KeyMatch4Func : AbstractFunction
    {
        protected override Delegate GetFunc()
        {
            Func<string, string, bool> call = BuiltInFunctions.KeyMatch4;
            return call;
        }

        public KeyMatch4Func() : base("keyMatch4")
        {

        }
    }
}
