using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.UnitTests.ParallelTest
{
    public interface IConsumer<TRequest> where TRequest : IRequestValues
    {
        Task<bool> GetAccessAsync(TRequest request);
        Task<bool> UpdatePolicyAsync(TRequest oldRequest, TRequest newRequest);
        Task<bool> RemovePolicyAsync(TRequest request);
        Task<bool> AddPolicyAsync(TRequest request);
    }
}