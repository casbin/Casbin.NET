using System.Collections.Generic;

namespace Casbin.Model
{
    public interface IPolicyStore : IReadOnlyPolicyStore
    {
        public bool AddNode(string section, string type, PolicyAssertion policyAssertion);

        public bool AddPolicy(string section, string policyType, IPolicyValues values);

        public bool AddPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        public bool UpdatePolicy(string section, string policyType, IPolicyValues oldRule, IPolicyValues newRule);

        public bool UpdatePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> oldRules,
            IReadOnlyList<IPolicyValues> newRules);

        public bool RemovePolicy(string section, string policyType, IPolicyValues rule);

        public bool RemovePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        public IEnumerable<IPolicyValues> RemoveFilteredPolicy(string section, string policyType, int fieldIndex,
            IPolicyValues fieldValues);

        public bool SortPolicyByPriority(string section, string policyType);

        public bool SortPolicyBySubjectHierarchy(string section, string policyType,
            IDictionary<string, int> subjectHierarchyMap);

        public void ClearPolicy();
    }
}
