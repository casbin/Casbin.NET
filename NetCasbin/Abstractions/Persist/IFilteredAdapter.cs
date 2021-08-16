using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Persist
{
    public interface IFilteredAdapter
    {
        bool IsFiltered { get; }

        void LoadFilteredPolicy(IModel model, Filter filter);

        Task LoadFilteredPolicyAsync(IModel model, Filter filter);
    }
}
