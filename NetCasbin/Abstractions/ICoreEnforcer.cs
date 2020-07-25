using System;
using System.Threading.Tasks;
using NetCasbin.Effect;
using NetCasbin.Persist;
using NetCasbin.Rbac;

namespace NetCasbin.Abstractions
{
    /// <summary>
    /// ICoreEnforcer is the API interface of CoreEnforcer
    /// </summary>
    [Obsolete("The interface will be removed at next mainline version, you can see https://github.com/casbin/Casbin.NET/issues/56 to know more information.")]
    public interface ICoreEnforcer
    {
        /// <summary>
        /// LoadModel reloads the model from the model CONF file. Because the policy is
        /// Attached to a model, so the policy is invalidated and needs to be reloaded by
        /// calling LoadPolicy().
        /// </summary>
        void LoadModel();

        /// <summary>
        /// Gets the current model.
        /// </summary>
        /// <returns>The model of the enforcer.</returns>
        Model.Model GetModel();

        /// <summary>
        /// Sets the current model.
        /// </summary>
        /// <param name="model"></param>
        void SetModel(Model.Model model);

        /// <summary>
        /// Gets the current adapter.
        /// </summary>
        /// <returns></returns>
        IAdapter GetAdapter();

        /// <summary>
        /// Sets an adapter.
        /// </summary>
        /// <param name="adapter"></param>
        void SetAdapter(IAdapter adapter);

        /// <summary>
        /// Sets an watcher.
        /// </summary>
        /// <param name="watcher"></param>
        /// <param name="useAsync">Whether use async update callback.</param>
        void SetWatcher(IWatcher watcher, bool useAsync = true);

        /// <summary>
        /// Sets the current role manager.
        /// </summary>
        /// <param name="roleManager"></param>
        void SetRoleManager(IRoleManager roleManager);

        /// <summary>
        /// Sets the current effector.
        /// </summary>
        /// <param name="effector"></param>
        void SetEffector(IEffector effector);

        /// <summary>
        /// Clears all policy.
        /// </summary>
        void ClearPolicy();

        /// <summary>
        /// Reloads the policy from file/database.
        /// </summary>
        void LoadPolicy();

        /// <summary>
        /// Reloads the policy from file/database.
        /// </summary>
        Task LoadPolicyAsync();

        /// <summary>
        /// Reloads a filtered policy from file/database.
        /// </summary>
        /// <param name="filter">The filter used to specify which type of policy should be loaded.</param>
        /// <returns></returns>
        bool LoadFilteredPolicy(Filter filter);

        /// <summary>
        /// Reloads a filtered policy from file/database.
        /// </summary>
        /// <param name="filter">The filter used to specify which type of policy should be loaded.</param>
        /// <returns></returns>
        Task<bool> LoadFilteredPolicyAsync(Filter filter);

        /// <summary>
        /// Returns true if the loaded policy has been filtered.
        /// </summary>
        /// <returns>if the loaded policy has been filtered.</returns>
        bool IsFiltered();

        /// <summary>
        /// Saves the current policy (usually after changed with Casbin API)
        /// back to file/database.
        /// </summary>
        void SavePolicy();

        /// <summary>
        /// Saves the current policy (usually after changed with Casbin API)
        /// back to file/database.
        /// </summary>
        Task SavePolicyAsync();

        /// <summary>
        /// Changes the enforcing state of Casbin, when Casbin is disabled,
        /// all access will be allowed by the enforce() function.
        /// </summary>
        /// <param name="enable"></param>
        void EnableEnforce(bool enable);

        /// <summary>
        /// Controls whether to save a policy rule automatically to the
        /// adapter when it is added or removed.
        /// </summary>
        /// <param name="autoSave"></param>
        void EnableAutoSave(bool autoSave);

        /// <summary>
        /// Controls whether to save a policy rule automatically
        /// to the adapter when it is added or removed.
        /// </summary>
        /// <param name="autoBuildRoleLinks">Whether to automatically build the role links.</param>
        void EnableAutoBuildRoleLinks(bool autoBuildRoleLinks);

        /// <summary>
        /// Manually rebuilds the role inheritance relations.
        /// </summary>
        void BuildRoleLinks();

        /// <summary>
        /// Decides whether a "subject" can access a "object" with the operation
        /// "action", input parameters are usually: (sub, obj, act).
        /// </summary>
        /// <param name="rvals">The request needs to be mediated, usually an array of strings, 
        /// can be class instances if ABAC is used.</param>
        /// <returns>Whether to allow the request.</returns>
        bool Enforce(params object[] rvals);
    }

}
