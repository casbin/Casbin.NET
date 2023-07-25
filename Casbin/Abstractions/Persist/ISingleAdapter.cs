using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Persist
{
    public interface ISingleAdapter
    {
        void AddPolicy(string section, string policyType, IPolicyValues rule);

        Task AddPolicyAsync(string section, string policyType, IPolicyValues rule);

        void UpdatePolicy(string section, string policyType, IPolicyValues oldRule, IPolicyValues newRule);

        Task UpdatePolicyAsync(string section, string policyType, IPolicyValues oldRules, IPolicyValues newRules);

        void RemovePolicy(string section, string policyType, IPolicyValues rule);

        Task RemovePolicyAsync(string section, string policyType, IPolicyValues rule);
    }
}
