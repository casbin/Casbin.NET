using System.Threading.Tasks;
using System.Collections.Generic;
using Casbin.Model;

namespace Casbin.UnitTests.ParallelTest
{
    public interface ITransaction<TRequest> where TRequest : IRequestValues
    {
        /// <summary>
        /// Execute the transaction in async environment.
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns> Whether the result from consumer is correct.</returns>
        Task<bool> ExecuteAsync(IConsumer<TRequest> consumer);
        /// <summary>
        /// Set the truth with sync method of enforcer
        /// </summary>
        /// <param name="enforcer"></param>
        void SetTruth(IEnforcer enforcer);
        /// <summary>
        /// The request values. At least one element is expected.
        /// </summary>
        IEnumerable<TRequest> Request { get; }
        TransactionType TransactionType { get; }
        bool ExpectedResult { get; }
        bool ActualResult { get; }
        bool HasCompleted { get; }
    }
}