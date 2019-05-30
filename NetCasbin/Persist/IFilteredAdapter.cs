using System;
using System.Collections.Generic;
using System.Text;

namespace NetCasbin.Persist
{
    public interface IFilteredAdapter
    {
        void LoadFilteredPolicy(Model model, Filter filter);

        Boolean IsFiltered { get; }
    }
}
