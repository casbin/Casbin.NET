using Casbin.Model;

namespace Casbin.UnitTests.ParallelTest
{
    public enum TransactionType
    {
        GetAccess = 0, 
        AddPolicy, 
        RemovePolicy,
        UpdatePolicy
    }
    public interface ITransactionFactory
    {
        ITransaction<TRequest> CreateTransaction<TRequest>(TransactionType transactionType, params TRequest[] request) 
            where TRequest : IRequestValues;
    }
}