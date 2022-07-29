using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Persist
{
    public interface IBatchAdapter
    {
        void AddPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        Task AddPoliciesAsync(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        void UpdatePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> oldRules,
            IReadOnlyList<IPolicyValues> newRules);

        Task UpdatePoliciesAsync(string section, string policyType, IReadOnlyList<IPolicyValues> oldRules,
            IReadOnlyList<IPolicyValues> newRules);

        void RemovePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        Task RemovePoliciesAsync(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        void RemoveFilteredPolicy(string section, string policyType, int fieldIndex, IPolicyValues fieldValues);

        Task RemoveFilteredPolicyAsync(string section, string policyType, int fieldIndex, IPolicyValues fieldValues);
    }
}
