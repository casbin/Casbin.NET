using System.Collections.Generic;

namespace Casbin.Model
{
    public interface IPolicyStore
    {
        public Dictionary<string, Dictionary<string, Assertion>> Sections { get; }

        public Assertion GetRequiredAssertion(string section, string type);

        public bool TryGetAssertion(string section, string policyType, out Assertion returnAssertion);

        public IEnumerable<IPolicyValues> GetPolicy(string section, string policyType);

        public IEnumerable<IPolicyValues> GetFilteredPolicy(string section, string policyType, int fieldIndex,
            IPolicyValues fieldValues);

        public IEnumerable<string> GetValuesForFieldInPolicy(string section, string policyType, int fieldIndex);

        public IEnumerable<string> GetValuesForFieldInPolicyAllTypes(string section, int fieldIndex);

        public bool HasPolicy(string section, string policyType, IPolicyValues rule);

        public bool HasPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        public bool HasAllPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        public bool AddPolicy(string section, string policyType, IPolicyValues values);

        public bool AddPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        public bool UpdatePolicy(string section, string policyType, IPolicyValues oldRule, IPolicyValues newRule);

        public bool UpdatePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> oldRules,
            IReadOnlyList<IPolicyValues> newRules);

        public bool RemovePolicy(string section, string policyType, IPolicyValues rule);

        public bool RemovePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        public IEnumerable<IPolicyValues> RemoveFilteredPolicy(string section, string policyType, int fieldIndex,
            IPolicyValues fieldValues);

        public void ClearPolicy();
    }
}
