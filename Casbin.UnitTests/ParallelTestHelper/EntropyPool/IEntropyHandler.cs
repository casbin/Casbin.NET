using Casbin.Model;

namespace Casbin.UnitTests.ParallelTest
{
    public interface IEntropyHandler<TRequest> where TRequest : IRequestValues
    {
        /// <summary>
        /// The weight of the handler, which impacts on its possibility to be selected.
        /// </summary>
        int Weight { get; }
        /// <summary>
        /// Decide how to generate a request with requests from existed pool and random pool respectively.
        /// </summary>
        /// <param name="requestFromExistedPool"></param>
        /// <param name="requestFromRandomPool"></param>
        /// <returns></returns>
        TRequest Handle(TRequest requestFromExistedPool, TRequest requestFromRandomPool);
    }
}