using System.Collections.Generic;
using Casbin.Rbac;

namespace Casbin
{
   public interface IPolicy
   {
        /// <summary>
        /// Provides incremental build the role inheritance relation.
        /// </summary>
        /// <param name="roleManager"></param>
        /// <param name="policyOperation"></param>
        /// <param name="sectiontion"></param>
        /// <param name="policyType"></param>
        /// <param name="rule"></param>
        public void BuildIncrementalRoleLink(IRoleManager roleManager, PolicyOperation policyOperation,
            string sectiontion, string policyType, IEnumerable<string> rule);

        /// <summary>
        /// Provides incremental build the role inheritance relations.
        /// </summary>
        /// <param name="roleManager"></param>
        /// <param name="policyOperation"></param>
        /// <param name="sectiontion"></param>
        /// <param name="policyType"></param>
        /// <param name="rules"></param>
        public void BuildIncrementalRoleLinks(IRoleManager roleManager, PolicyOperation policyOperation,
            string sectiontion, string policyType, IEnumerable<IEnumerable<string>> rules);

        /// <summary>
        /// Initializes the roles in RBAC.
        /// </summary>
        /// <param name="roleManager"></param>
        public void BuildRoleLinks(IRoleManager roleManager);

        public void RefreshPolicyStringSet();

        public List<List<string>> GetPolicy(string section, string policyType);

        public List<List<string>> GetFilteredPolicy(string section, string policyType, int fieldIndex,
            params string[] fieldValues);

        public List<string> GetValuesForFieldInPolicy(string section, string policyType, int fieldIndex);

        public List<string> GetValuesForFieldInPolicyAllTypes(string section, int fieldIndex);

        public bool HasPolicy(string section, string policyType, List<string> rule);

        public bool HasPolicies(string section, string policyType, IEnumerable<List<string>> rules);

        public bool AddPolicy(string section, string policyType, List<string> rule);

        public bool AddPolicies(string section, string policyType, IEnumerable<List<string>> rules);

        public bool RemovePolicy(string section, string policyType, List<string> rule);

        public bool RemovePolicies(string section, string policyType, IEnumerable<List<string>> rules);

        public bool RemoveFilteredPolicy(string section, string policyType, int fieldIndex, params string[] fieldValues);

        public void ClearPolicy();
   }
}
