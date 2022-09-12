using System;
using System.Collections.Generic;
using Casbin.Model;

namespace Casbin.UnitTests.ParallelTest
{
    public class DefaultExistedEntropyPool<TRequest> : IEntropyPool<TRequest> where TRequest : IRequestValues
    {
        private readonly List<TRequest> _cachedRequests = new();
        private Func<IPolicyValues, TRequest> _convertFunc;

        /// <summary>
        /// The enforcer here is only used to init the pool.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policy2RequestFunc"></param>
        public DefaultExistedEntropyPool(IEnforcer enforcer, Func<IPolicyValues, TRequest> policy2RequestFunc)
        {
            _convertFunc = policy2RequestFunc;
            IDictionary<string, PolicyAssertion> policies = enforcer.Model.Sections.GetPolicyAssertions();
            foreach (var assertion in policies.Values)
            {
                foreach (IPolicyValues policy in assertion.PolicyManager.GetPolicy())
                {
                    _cachedRequests.Add(policy2RequestFunc(policy));
                }
            }
        }

        public TRequest Get()
        {
            int total = _cachedRequests.Count;
            if (total == 0)
            {
                throw new InvalidOperationException("The pool is empty.");
            }

            Random rd = new();
            return _cachedRequests[rd.Next(0, total)];
        }

        public void Add(TRequest request)
        {
            _cachedRequests.Add(request);
        }
    }

    public static class DefaultExistedEntropyPool
    {
        public static DefaultExistedEntropyPool<RequestValues<string>> Create(Enforcer enforcer)
        {
            return new DefaultExistedEntropyPool<RequestValues<string>>(enforcer, x => Request.CreateValues(x[0]));
        }

        public static DefaultExistedEntropyPool<RequestValues<string, string>> Create2(Enforcer enforcer)
        {
            return new DefaultExistedEntropyPool<RequestValues<string, string>>(enforcer,
                x => Request.CreateValues(x[0], x[1]));
        }

        public static DefaultExistedEntropyPool<RequestValues<string, string, string>> Create3(Enforcer enforcer)
        {
            return new DefaultExistedEntropyPool<RequestValues<string, string, string>>(enforcer,
                x => Request.CreateValues(x[0], x[1], x[2]));
        }

        public static DefaultExistedEntropyPool<RequestValues<string, string, string, string>> Create4(
            Enforcer enforcer)
        {
            return new DefaultExistedEntropyPool<RequestValues<string, string, string, string>>(
                enforcer, x => Request.CreateValues(x[0], x[1], x[2], x[3])
            );
        }

        public static DefaultExistedEntropyPool<RequestValues<string, string, string, string, string>> Create5(
            Enforcer enforcer)
        {
            return new DefaultExistedEntropyPool<RequestValues<string, string, string, string, string>>(
                enforcer, x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4])
            );
        }

        public static DefaultExistedEntropyPool<RequestValues<string, string, string, string, string, string>> Create6(
            Enforcer enforcer)
        {
            return new DefaultExistedEntropyPool<RequestValues<string, string, string, string, string, string>>(
                enforcer, x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4], x[5])
            );
        }

        public static DefaultExistedEntropyPool<RequestValues<string, string, string, string, string, string, string>>
            Create7(Enforcer enforcer)
        {
            return new DefaultExistedEntropyPool<RequestValues<string, string, string, string, string, string, string>>(
                enforcer, x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4], x[5], x[6])
            );
        }

        public static DefaultExistedEntropyPool<
                RequestValues<string, string, string, string, string, string, string, string>>
            Create8(Enforcer enforcer)
        {
            return new DefaultExistedEntropyPool<
                RequestValues<string, string, string, string, string, string, string, string>>(
                enforcer, x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4], x[5], x[6], x[7])
            );
        }

        public static DefaultExistedEntropyPool<
                RequestValues<string, string, string, string, string, string, string, string, string>>
            Create9(Enforcer enforcer)
        {
            return new DefaultExistedEntropyPool<
                RequestValues<string, string, string, string, string, string, string, string, string>>(
                enforcer, x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4], x[5], x[6], x[7], x[8])
            );
        }

        public static DefaultExistedEntropyPool<
                RequestValues<string, string, string, string, string, string, string, string, string, string>>
            Create10(Enforcer enforcer)
        {
            return new DefaultExistedEntropyPool<RequestValues<string, string, string, string, string, string, string,
                string, string, string>>(
                enforcer, x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4], x[5], x[6], x[7], x[8], x[9])
            );
        }

        public static DefaultExistedEntropyPool<RequestValues<string, string, string, string, string, string, string,
                string, string, string, string>>
            Create11(Enforcer enforcer)
        {
            return new DefaultExistedEntropyPool<RequestValues<string, string, string, string, string, string, string,
                string, string, string, string>>(
                enforcer, x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4], x[5], x[6], x[7], x[8], x[9], x[10])
            );
        }

        public static DefaultExistedEntropyPool<RequestValues<string, string, string, string, string, string, string,
                string, string, string, string, string>>
            Create12(Enforcer enforcer)
        {
            return new DefaultExistedEntropyPool<RequestValues<string, string, string, string, string, string, string,
                string, string, string, string, string>>(
                enforcer,
                x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4], x[5], x[6], x[7], x[8], x[9], x[10], x[11])
            );
        }
    }
}
