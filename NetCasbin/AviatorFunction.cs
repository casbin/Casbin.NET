using System;
using System.Collections.Generic;
using System.Text;

namespace NetCasbin
{
    class AviatorFunction : AbstractFunction
    {
        private readonly Delegate _func;

        public AviatorFunction(string name, Delegate func) : base(name)
        {
            _func = func;
        }
        protected override Delegate GetFunc()
        {
            return _func;
        }
    }
}
