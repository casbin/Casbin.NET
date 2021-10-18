using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Persist
{
    public interface IReadOnlyAdapter
    {
        void LoadPolicy(IModel model);

        Task LoadPolicyAsync(IModel model);
    }
}
