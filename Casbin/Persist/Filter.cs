using System.Collections.Generic;

namespace Casbin.Persist
{
    public class Filter
    {
        public IEnumerable<string> G { get; set; }

        public IEnumerable<string> P { get; set; }
    }
}
