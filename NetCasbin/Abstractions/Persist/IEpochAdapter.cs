using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Persist
{
    public interface IEpochAdapter : IReadOnlyAdapter
    {
        void SavePolicy(IModel model);

        Task SavePolicyAsync(IModel model);
    }
}
