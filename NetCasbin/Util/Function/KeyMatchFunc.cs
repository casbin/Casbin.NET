using System;

namespace NetCasbin.Util.Function
{
    public sealed class KeyMatchFunc : AbstractFunction
    {
        protected override Delegate GetFunc()
        {
            Func< string, string, bool> call = ( arg1, arg2) =>
            {
                return BuiltInFunctions.KeyMatch(arg1, arg2);
            };
            return call;
        }

        public KeyMatchFunc() : base("keyMatch")
        {

        }
    }
}
