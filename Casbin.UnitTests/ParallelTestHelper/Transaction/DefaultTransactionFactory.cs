using Casbin.Model;

namespace Casbin.UnitTests.ParallelTest
{
    public class DefaultTransactionFactory : ITransactionFactory
    {
        public ITransaction<TRequest> CreateTransaction<TRequest>(TransactionType transactionType, params TRequest[] requests) 
            where TRequest : IRequestValues
        {
            return transactionType switch
            {
                TransactionType.GetAccess => new DefaultGetAccessTransaction<TRequest>(requests[0]),
                TransactionType.AddPolicy => new DefaultAddPolicyTransaction<TRequest>(requests[0]),
                TransactionType.RemovePolicy => new DefaultRemovePolicyTransaction<TRequest>(requests[0]),
                // TODO : update policy
                _ => throw new System.ArgumentException("Invalid transaction type.")
            };
        }
    }
}