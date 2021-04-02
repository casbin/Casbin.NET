using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.Persist;
using Casbin.Rbac;
#if !NET45
using Microsoft.Extensions.Logging;
#endif
namespace Casbin
{
    /// <summary>
    /// IEnforcer is the API interface of Enforcer
    /// </summary>
    public interface IEnforcer
    {
        #region Options
        public bool Enabled { get; set; }
        public bool EnabledCache { get; set; }
        public bool AutoSave { get; set; }
        public bool AutoBuildRoleLinks { get; set; }
        public bool AutoNotifyWatcher { get; set; }
        public bool AutoCleanEnforceCache { get; set; }
        #endregion

        #region Extensions
        public IEffector Effector { get; set; }
        public IModel Model { get; set; }
        public IPolicyManager PolicyManager { get; set; }
        public IAdapter Adapter { get; set; }
        public IWatcher Watcher { get; set; }
        public IRoleManager RoleManager { get; set; }
        public IEnforceCache EnforceCache { get; set; }
        public IExpressionHandler ExpressionHandler { get; set; }
#if !NET45
        public ILogger Logger { get; set; }
#endif
        #endregion

        public string ModelPath { get; }
        public bool IsFiltered { get; }

        /// <summary>
        /// LoadModel reloads the model from the model CONF file. Because the policy is
        /// Attached to a model, so the policy is invalidated and needs to be reloaded by
        /// calling LoadPolicy().
        /// </summary>
        public void LoadModel();

        /// <summary>
        /// Manually rebuilds the role inheritance relations.
        /// </summary>
        public void BuildRoleLinks();

        #region Policy Management

        /// <summary>
        /// Reloads the policy from file/database.
        /// </summary>
        public void LoadPolicy();

        /// <summary>
        /// Reloads the policy from file/database.
        /// </summary>
        public Task LoadPolicyAsync();

        /// <summary>
        /// Reloads a filtered policy from file/database.
        /// </summary>
        /// <param name="filter">The filter used to specify which type of policy should be loaded.</param>
        /// <returns></returns>
        public bool LoadFilteredPolicy(Filter filter);

        /// <summary>
        /// Reloads a filtered policy from file/database.
        /// </summary>
        /// <param name="filter">The filter used to specify which type of policy should be loaded.</param>
        /// <returns></returns>
        public Task<bool> LoadFilteredPolicyAsync(Filter filter);

        /// <summary>
        /// Saves the current policy (usually after changed with Casbin API)
        /// back to file/database.
        /// </summary>
        public void SavePolicy();

        /// <summary>
        /// Saves the current policy (usually after changed with Casbin API)
        /// back to file/database.
        /// </summary>
        public Task SavePolicyAsync();

        /// <summary>
        /// Clears all policy.
        /// </summary>
        public void ClearPolicy();

        #endregion

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public bool Enforce(params object[] requestValues);

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        public Task<bool> EnforceAsync(params object[] requestValues);

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
#if !NET45

        public (bool Result, IEnumerable<IEnumerable<string>> Explains)
            EnforceEx(params object[] requestValues);
#else
        public Tuple<bool, IEnumerable<IEnumerable<string>>>
            EnforceEx(params object[] requestValues);
#endif

        /// <summary>
        /// Explains enforcement by informing matched rules
        /// </summary>
        /// <param name="requestValues">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request and explains.</returns>
#if !NET45
        public Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
            EnforceExAsync(params object[] requestValues);
#else
        public Task<Tuple<bool, IEnumerable<IEnumerable<string>>>>
            EnforceExAsync(params object[] requestValues);
#endif
    }

}
