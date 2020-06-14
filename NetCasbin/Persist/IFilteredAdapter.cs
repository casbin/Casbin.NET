using System;
using System.Threading.Tasks;

namespace NetCasbin.Persist
{
    public interface IFilteredAdapter
    {
        void LoadFilteredPolicy(Model.Model model, Filter filter);

        Task LoadFilteredPolicyAsync(Model.Model model, Filter filter);

        bool IsFiltered { get; }
    }
}
