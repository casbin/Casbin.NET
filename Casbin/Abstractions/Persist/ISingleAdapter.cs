using System.Collections.Generic;
using System.Threading.Tasks;

namespace Casbin.Persist
{
    public interface ISingleAdapter
    {
        void AddPolicy(string section, string policyType, IEnumerable<string> rule);

        Task AddPolicyAsync(string section, string policyType, IEnumerable<string> rule);

        void RemovePolicy(string section, string policyType, IEnumerable<string> rule);

        Task RemovePolicyAsync(string section, string policyType, IEnumerable<string> rule);
    }
}
