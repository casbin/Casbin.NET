using System.Collections.Generic;
using Casbin.Caching;
using Casbin.Evaluation;
using Casbin.Rbac;

namespace Casbin.Model
{
    public interface IModel : IPolicyStore
    {
        public bool IsSynchronized { get; }

        public string ModelPath { get; }

        public IEnforceViewCache EnforceViewCache { get; }

        public IExpressionHandler ExpressionHandler { get; }

        public IPolicyManager PolicyManager { get; set; }

        public void LoadModelFromFile(string path);

        public void LoadModelFromText(string text);

        public bool AddDef(string section, string key, string value);

        public void SetRoleManager(string roleType, IRoleManager roleManager);

        /// <summary>
        /// Provides incremental build the role inheritance relation.
        /// </summary>
        /// <param name="policyOperation"></param>
        /// <param name="section"></param>
        /// <param name="roleType"></param>
        /// <param name="rule"></param>
        public void BuildIncrementalRoleLink(PolicyOperation policyOperation,
            string section, string roleType, IPolicyValues rule);

        /// <summary>
        /// Provides incremental build the role inheritance relation.
        /// </summary>
        /// <param name="policyOperation"></param>
        /// <param name="section"></param>
        /// <param name="roleType"></param>
        /// <param name="oldRule"></param>
        /// <param name="newRule"></param>
        public void BuildIncrementalRoleLink(PolicyOperation policyOperation,
            string section, string roleType, IPolicyValues oldRule, IPolicyValues newRule);

        /// <summary>
        /// Provides incremental build the role inheritance relations.
        /// </summary>
        /// <param name="policyOperation"></param>
        /// <param name="section"></param>
        /// <param name="roleType"></param>
        /// <param name="rules"></param>
        public void BuildIncrementalRoleLinks(PolicyOperation policyOperation,
            string section, string roleType, IEnumerable<IPolicyValues> rules);

        /// <summary>
        /// Provides incremental build the role inheritance relations.
        /// </summary>
        /// <param name="policyOperation"></param>
        /// <param name="section"></param>
        /// <param name="roleType"></param>
        /// <param name="oldRules"></param>
        /// <param name="newRules"></param>
        public void BuildIncrementalRoleLinks(PolicyOperation policyOperation,
            string section, string roleType, IEnumerable<IPolicyValues> oldRules, IEnumerable<IPolicyValues> newRules);

        /// <summary>
        /// Initializes the roles in RBAC.
        /// </summary>
        public void BuildRoleLinks(string roleType = null);

        public void RefreshPolicyStringSet();

        public void SortPoliciesByPriority();

        public void SortPoliciesBySubjectHierarchy();
    }
}
