using System;
using System.Collections.Generic;
using System.Text;

namespace NetCasbin.Persist
{
    public class Filter
    {
        public IEnumerable<string> G { set; get; }

        public IEnumerable<String> P { set; get; }
    }
}
