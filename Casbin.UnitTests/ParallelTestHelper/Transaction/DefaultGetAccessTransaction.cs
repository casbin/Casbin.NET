using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Casbin.Model;
using Casbin.UnitTests.Extensions;

namespace Casbin.UnitTests.ParallelTest
{
    public class DefaultGetAccessTransaction<TRequest> : ITransaction<TRequest> where TRequest : IRequestValues
    {
        private List<TRequest> _requests = new List<TRequest>();
        public bool ExpectedResult { get; private set; } = false;
        public bool ActualResult { get; private set; } = true;
        public bool HasCompleted { get; private set; } = false;
        public DefaultGetAccessTransaction(TRequest request)
        {
            _requests.Add(request);
        }
        public async Task<bool> ExecuteAsync(IConsumer<TRequest> consumer)
        {
            ActualResult = await consumer.GetAccessAsync(Request.First());
            HasCompleted = true;
            return true;
        }
        public void SetTruth(IEnforcer enforcer)
        {
            ExpectedResult = enforcer.Enforce(enforcer.CreateContext(), Request.First());
        }
        public IEnumerable<TRequest> Request { get { return _requests; } }
        public TransactionType TransactionType { get; } = TransactionType.GetAccess;
    }
}