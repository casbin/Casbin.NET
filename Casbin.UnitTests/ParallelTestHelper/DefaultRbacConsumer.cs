using System.Threading.Tasks;
using Casbin.Model;
using Casbin.UnitTests.Extensions;
using Xunit.Abstractions;

namespace Casbin.UnitTests.ParallelTest
{
    public class DefaultRbacConsumer<TRequest> : IConsumer<TRequest> where TRequest : IRequestValues
    {
        private Enforcer _enforcer;
        public DefaultRbacConsumer(Enforcer enforcer)
        {
            _enforcer = enforcer;
        }
        public DefaultRbacConsumer(string modelPath, string policyPath)
        {
            if(!string.IsNullOrEmpty(modelPath) && !string.IsNullOrEmpty(policyPath))
            {
                _enforcer = new Enforcer(modelPath, policyPath);
            }
            else
            {
                _enforcer = new Enforcer();
            }
            _enforcer.BuildRoleLinks();
        }
        public async Task<bool> GetAccessAsync(TRequest request)
        {
            return await _enforcer.EnforceAsync(_enforcer.CreateContext(), request);
        }
        public async Task<bool> UpdatePolicyAsync(TRequest oldRequest, TRequest newRequest)
        {
            // TODO
            return true;
        }
        public async Task<bool> RemovePolicyAsync(TRequest request)
        {
            return await _enforcer.RemovePolicyAsync(request.ToEnumerable());
        }
        public async Task<bool> AddPolicyAsync(TRequest request)
        {
            return await _enforcer.AddPolicyAsync(request.ToEnumerable());
        }
    }
}