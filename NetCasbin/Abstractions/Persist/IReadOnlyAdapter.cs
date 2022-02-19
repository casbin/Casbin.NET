using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Persist
{
    public interface IReadOnlyAdapter
    {
        void LoadPolicy(IPolicyStore model);

        Task LoadPolicyAsync(IPolicyStore model);
    }
}
