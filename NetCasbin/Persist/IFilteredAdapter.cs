using System;

namespace NetCasbin.Persist
{
    public interface IFilteredAdapter
    {
        void LoadFilteredPolicy(Model.Model model, Filter filter);

        Boolean IsFiltered { get; }
    }
}
