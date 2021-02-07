using System;

namespace Casbin.Util.Function
{
    public class KeyMatch4Func : AbstractFunction
    {
        public KeyMatch4Func() : base("keyMatch4")
        {

        }

        protected override Delegate GetFunc()
        {
            Func<string, string, bool> call = BuiltInFunctions.KeyMatch4;
            return call;
        }
    }
}
