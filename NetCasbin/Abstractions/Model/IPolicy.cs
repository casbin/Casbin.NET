using System.Collections.Generic;

namespace Casbin.Model
{
   public interface IPolicy
   {
        public Dictionary<string, Dictionary<string, Assertion>> Sections { get; }

        public Assertion GetRequiredAssertion(string section, string type);

        public IEnumerable<IEnumerable<string>> GetPolicy(string section, string policyType);

        public IEnumerable<IEnumerable<string>> GetFilteredPolicy(string section, string policyType, int fieldIndex,
            params string[] fieldValues);

        public IEnumerable<string> GetValuesForFieldInPolicy(string section, string policyType, int fieldIndex);

        public IEnumerable<string> GetValuesForFieldInPolicyAllTypes(string section, int fieldIndex);

        public bool HasPolicy(string section, string policyType, IEnumerable<string> rule);

        public bool HasPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules);

        public bool AddPolicy(string section, string policyType, IEnumerable<string> rule);

        public bool AddPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules);

        public bool RemovePolicy(string section, string policyType, IEnumerable<string> rule);

        public bool RemovePolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules);

        public IEnumerable<IEnumerable<string>> RemoveFilteredPolicy(string section, string policyType, int fieldIndex, params string[] fieldValues);

        public void ClearPolicy();
   }
}
