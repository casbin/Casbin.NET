using System;

namespace NetCasbin.Util.Function
{
    public sealed class KeyMatchFunc : AbstractFunction
    {
        protected override Delegate GetFunc()
        {
            Func<string, string, bool> call = BuiltInFunctions.KeyMatch;
            return call;
        }

        public KeyMatchFunc() : base("keyMatch")
        {

        }
    }
}
