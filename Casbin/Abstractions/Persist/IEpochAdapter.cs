using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Persist
{
    public interface IEpochAdapter : IReadOnlyAdapter
    {
        void SavePolicy(IPolicyStore model);

        Task SavePolicyAsync(IPolicyStore model);
    }
}
