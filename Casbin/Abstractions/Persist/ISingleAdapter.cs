using System.Collections.Generic;
using System.Threading.Tasks;

namespace Casbin.Persist
{
    public interface ISingleAdapter
    {
        void AddPolicy(string section, string policyType, IEnumerable<string> rule);

        Task AddPolicyAsync(string section, string policyType, IEnumerable<string> rule);

        void UpdatePolicy(string section, string policyType, IEnumerable<string> oldRule, IEnumerable<string> newRule);

        Task UpdatePolicyAsync(string section, string policyType, IEnumerable<string> oldRules, IEnumerable<string> newRules);

        void RemovePolicy(string section, string policyType, IEnumerable<string> rule);

        Task RemovePolicyAsync(string section, string policyType, IEnumerable<string> rule);
    }
}
