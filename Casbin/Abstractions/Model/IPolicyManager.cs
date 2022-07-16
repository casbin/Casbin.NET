using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.Persist;

namespace Casbin.Model
{
    public interface IPolicyManager : IPolicyStore
    {
        public bool IsSynchronized { get; }

        public IReadOnlyAdapter Adapter { get; set; }

        public bool HasAdapter { get; }

        public bool IsFiltered { get; }

        public bool AutoSave { get; set; }

        public IPolicyStore PolicyStore { get; set; }

        public void StartRead();

        public void StartWrite();

        public bool TryStartRead();

        public bool TryStartWrite();

        public void EndRead();

        public void EndWrite();

        public bool LoadPolicy();

        public Task<bool> LoadPolicyAsync();

        public bool LoadFilteredPolicy(Filter filter);

        public Task<bool> LoadFilteredPolicyAsync(Filter filter);

        public bool SavePolicy();

        public Task<bool> SavePolicyAsync();

        public Task<bool> AddPolicyAsync(string section, string policyType, IEnumerable<string> rule);

        public Task<bool> AddPoliciesAsync(string section, string policyType, IEnumerable<IEnumerable<string>> rules);

        public Task<bool> UpdatePolicyAsync(string section, string policyType, IEnumerable<string> oldRule, IEnumerable<string> newRule);

        public Task<bool> UpdatePoliciesAsync(string section, string policyType, IEnumerable<IEnumerable<string>> oldRules, IEnumerable<IEnumerable<string>> newRules);

        public Task<bool> RemovePolicyAsync(string section, string policyType, IEnumerable<string> rule);

        public Task<bool> RemovePoliciesAsync(string section, string policyType,
            IEnumerable<IEnumerable<string>> rules);

        public Task<IEnumerable<IEnumerable<string>>> RemoveFilteredPolicyAsync(string section, string policyType,
            int fieldIndex, params string[] fieldValues);
    }
}
