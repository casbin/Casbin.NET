using System;

namespace Casbin.Util.Function
{
    public class KeyMatchFunc : AbstractFunction
    {
        public KeyMatchFunc() : base("keyMatch")
        {

        }

        protected override Delegate GetFunc()
        {
            Func<string, string, bool> call = BuiltInFunctions.KeyMatch;
            return call;
        }
    }
}
