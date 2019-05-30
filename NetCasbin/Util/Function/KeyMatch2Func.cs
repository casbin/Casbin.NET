using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NetCasbin
{
    public class KeyMatch2Func : AbstractFunction
    {

        protected override Delegate GetFunc()
        {
            Func<string, string, bool> call = (arg1, arg2) =>
            {
                return BuiltInFunctions.KeyMatch2(arg1, arg2);
            };
            return call;
        }

        public KeyMatch2Func() : base("keyMatch2")
        {

        }
    }
}
