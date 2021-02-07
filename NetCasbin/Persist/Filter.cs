using System.Collections.Generic;

namespace Casbin.Persist
{
    public class Filter
    {
        public IEnumerable<string> G { set; get; }

        public IEnumerable<string> P { set; get; }
    }
}
