using System.Collections.Generic;
using System.Threading.Tasks;

namespace Casbin.Model
{
    public interface IPolicyManager : IReadOnlyPolicyManager
    {
        public bool AutoSave { get; set; }

        public bool AddPolicy(IPolicyValues rule);

        public Task<bool> AddPolicyAsync(IPolicyValues rule);

        public bool AddPolicies(IReadOnlyList<IPolicyValues> rules);

        public Task<bool> AddPoliciesAsync(IReadOnlyList<IPolicyValues> rules);

        public bool UpdatePolicy(IPolicyValues oldRule,
            IPolicyValues newRule);

        public Task<bool> UpdatePolicyAsync(IPolicyValues oldRule,
            IPolicyValues newRule);

        public bool UpdatePolicies(IReadOnlyList<IPolicyValues> oldRules, IReadOnlyList<IPolicyValues> newRules);

        public Task<bool> UpdatePoliciesAsync(IReadOnlyList<IPolicyValues> oldRules,
            IReadOnlyList<IPolicyValues> newRules);

        public bool RemovePolicy(IPolicyValues rule);

        public Task<bool> RemovePolicyAsync(IPolicyValues rule);

        public bool RemovePolicies(IReadOnlyList<IPolicyValues> rules);

        public Task<bool> RemovePoliciesAsync(IReadOnlyList<IPolicyValues> rules);

        public IEnumerable<IPolicyValues> RemoveFilteredPolicy(int fieldIndex, IPolicyValues fieldValues);

        public Task<IEnumerable<IPolicyValues>> RemoveFilteredPolicyAsync(int fieldIndex, IPolicyValues fieldValues);
    }
}
