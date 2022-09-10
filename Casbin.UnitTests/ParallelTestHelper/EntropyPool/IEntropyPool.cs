using Casbin.Model;

namespace Casbin.UnitTests.ParallelTest
{
    public interface IEntropyPool<TRequest> where TRequest : IRequestValues
    {
        /// <summary>
        /// Get a value from the pool.
        /// </summary>
        /// <returns></returns>
        TRequest Get();
        /// <summary>
        /// Add a request/policy to the pool.
        /// </summary>
        /// <returns></returns>
        void Add(TRequest request);
    }
}