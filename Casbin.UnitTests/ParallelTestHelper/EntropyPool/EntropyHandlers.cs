using Casbin.Model;

namespace Casbin.UnitTests.ParallelTest
{
    public class PureRandomEntropyHandler<TRequest> : IEntropyHandler<TRequest> where TRequest : IRequestValues
    {
        public int Weight { get; }
        public PureRandomEntropyHandler(int weight)
        {
            Weight = weight;
        }

        public TRequest Handle(TRequest requestFromExistedPool, TRequest requestFromRandomPool)
        {
            return requestFromRandomPool;
        }
    }
    public class PureExistedEntropyHandler<TRequest> : IEntropyHandler<TRequest> where TRequest : IRequestValues
    {
        public int Weight { get; }
        public PureExistedEntropyHandler(int weight)
        {
            Weight = weight;
        }

        public TRequest Handle(TRequest requestFromExistedPool, TRequest requestFromRandomPool)
        {
            return requestFromExistedPool;
        }
    }
}