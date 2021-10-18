using System.Collections.Generic;
using System.Threading.Tasks;

namespace Casbin.Persist
{
    public interface IBatchAdapter
    {
        void AddPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules);

        Task AddPoliciesAsync(string section, string policyType, IEnumerable<IEnumerable<string>> rules);

        void RemovePolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules);

        Task RemovePoliciesAsync(string section, string policyType, IEnumerable<IEnumerable<string>> rules);

        void RemoveFilteredPolicy(string section, string policyType, int fieldIndex, params string[] fieldValues);

        Task RemoveFilteredPolicyAsync(string section, string policyType, int fieldIndex, params string[] fieldValues);
    }
}
