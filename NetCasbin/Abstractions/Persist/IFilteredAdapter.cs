using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Persist
{
    public interface IFilteredAdapter
    {
        bool IsFiltered { get; }

        void LoadFilteredPolicy(IPolicyStore store, Filter filter);

        Task LoadFilteredPolicyAsync(IPolicyStore store, Filter filter);
    }
}
