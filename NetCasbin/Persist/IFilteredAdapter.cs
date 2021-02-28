using System.Threading.Tasks;

namespace Casbin.Persist
{
    public interface IFilteredAdapter
    {
        void LoadFilteredPolicy(IModel model, Filter filter);

        Task LoadFilteredPolicyAsync(IModel model, Filter filter);

        bool IsFiltered { get; }
    }
}
