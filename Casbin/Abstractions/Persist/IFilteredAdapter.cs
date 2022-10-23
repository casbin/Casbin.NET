using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Persist
{
    public interface IFilteredAdapter
    {
        bool IsFiltered { get; }

        void LoadFilteredPolicy(IPolicyStore store, IPolicyFilter filter);

        Task LoadFilteredPolicyAsync(IPolicyStore store, IPolicyFilter filter);
    }
}
