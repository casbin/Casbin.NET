using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.Persist;

namespace Casbin.Model
{
    public interface IPolicyManager
    {
        public bool IsSynchronized { get; }

        public IReadOnlyAdapter Adapter { get; set; }

        public bool HasAdapter { get; }

        public bool AutoSave { get; set; }

        public IPolicy Policy { get; set; }

        public void StartRead();

        public void StartWrite();

        public bool TryStartRead();

        public bool TryStartWrite();

        public void EndRead();

        public void EndWrite();

        public IEnumerable<IEnumerable<string>> GetPolicy(string section, string policyType);

        public IEnumerable<IEnumerable<string>> GetFilteredPolicy(string section, string policyType, int fieldIndex,
            params string[] fieldValues);

        public IEnumerable<string> GetValuesForFieldInPolicy(string section, string policyType, int fieldIndex);

        public IEnumerable<string> GetValuesForFieldInPolicyAllTypes(string section, int fieldIndex);

        public bool LoadPolicy();

        public bool SavePolicy();

        public bool HasPolicy(string section, string policyType, IEnumerable<string> rule);

        public bool HasPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules);

        public bool AddPolicy(string section, string policyType, IEnumerable<string> rule);

        public bool AddPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules);

        public bool RemovePolicy(string section, string policyType, IEnumerable<string> rule);

        public bool RemovePolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules);

        public IEnumerable<IEnumerable<string>> RemoveFilteredPolicy(string section, string policyType, int fieldIndex, params string[] fieldValues);

        public bool SavePolicyAsync();

        public Task<bool> AddPolicyAsync(string section, string policyType, IEnumerable<string> rule);

        public Task<bool> AddPoliciesAsync(string section, string policyType, IEnumerable<IEnumerable<string>> rules);

        public Task<bool> RemovePolicyAsync(string section, string policyType, IEnumerable<string> rule);

        public Task<bool> RemovePoliciesAsync(string section, string policyType, IEnumerable<IEnumerable<string>> rules);

        public Task<IEnumerable<IEnumerable<string>>> RemoveFilteredPolicyAsync(string section, string policyType, int fieldIndex, params string[] fieldValues);

        public void ClearPolicy();
    }
}
