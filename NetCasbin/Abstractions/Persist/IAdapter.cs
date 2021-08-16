using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Persist
{
    public interface IAdapter
    {
        public void LoadPolicy(IModel model, CancellationToken cancellationToken = default);

        public Task LoadPolicyAsync(IModel model, CancellationToken cancellationToken = default);

        public void SavePolicy(IModel model, CancellationToken cancellationToken = default);

        public Task SavePolicyAsync(IModel model, CancellationToken cancellationToken = default);

        public void AddPolicy(string section, string policyType, IEnumerable<string> rule, CancellationToken cancellationToken = default);

        public Task AddPolicyAsync(string section, string policyType, IEnumerable<string> rule, CancellationToken cancellationToken = default);

        public void AddPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules, CancellationToken cancellationToken = default);

        public Task AddPoliciesAsync(string section, string policyType, IEnumerable<IEnumerable<string>> rules, CancellationToken cancellationToken = default);

        public void RemovePolicy(string section, string policyType, IEnumerable<string> rule, CancellationToken cancellationToken = default);

        public Task RemovePolicyAsync(string section, string policyType, IEnumerable<string> rule, CancellationToken cancellationToken = default);

        public void RemovePolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules, CancellationToken cancellationToken = default);

        public Task RemovePoliciesAsync(string section, string policyType, IEnumerable<IEnumerable<string>> rules, CancellationToken cancellationToken = default);

        public void RemoveFilteredPolicy(string section, string policyType, int fieldIndex, params string[] fieldValues);

        public Task RemoveFilteredPolicyAsync(string section, string policyType, int fieldIndex, params string[] fieldValues);
    }
}
