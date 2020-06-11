using System;
using System.Collections.Generic;

namespace NetCasbin.Persist
{
    public class Filter
    {
        public IEnumerable<string> G { set; get; }

        public IEnumerable<string> P { set; get; }
    }
}
