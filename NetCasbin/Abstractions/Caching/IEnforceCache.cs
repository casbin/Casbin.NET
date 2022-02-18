using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Caching
{
    public interface IEnforceCache
    {
        public bool TryGetResult<TRequest>(in TRequest requestValues, out bool result)
            where TRequest : IRequestValues;

        public Task<bool?> TryGetResultAsync<TRequest>(in TRequest requestValues)
            where TRequest : IRequestValues;

        public bool TrySetResult<TRequest>(in TRequest requestValues, bool result)
            where TRequest : IRequestValues;

        public Task<bool> TrySetResultAsync<TRequest>(in TRequest requestValues, bool result)
            where TRequest : IRequestValues;

        public void Clear();

        public Task ClearAsync();
    }
}
