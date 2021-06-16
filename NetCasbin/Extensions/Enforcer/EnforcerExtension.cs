using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Casbin.Evaluation;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Rbac;
#if !NET45
using Microsoft.Extensions.Logging;
#endif

namespace Casbin.Extensions
{
    public static class EnforcerExtension
    {
        #region Set options
        /// <summary>
        /// Changes the enforcing state of Casbin, when Casbin is disabled,
        /// all access will be allowed by the enforce() function.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="enable"></param>
        public static IEnforcer EnableEnforce(this IEnforcer enforcer, bool enable)
        {
            enforcer.Enabled = enable;
            return enforcer;
        }

        /// <summary>
        /// Controls whether to save a policy rule automatically to the
        /// adapter when it is added or removed.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="autoSave"></param>
        public static IEnforcer EnableAutoSave(this IEnforcer enforcer, bool autoSave)
        {
            enforcer.AutoSave = autoSave;
            return enforcer;
        }

        /// <summary>
        /// Controls whether to save a policy rule automatically
        /// to the adapter when it is added or removed.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="autoBuildRoleLinks">Whether to automatically build the role links.</param>
        public static IEnforcer EnableAutoBuildRoleLinks(this IEnforcer enforcer, bool autoBuildRoleLinks)
        {
            enforcer.AutoBuildRoleLinks = autoBuildRoleLinks;
            return enforcer;
        }

        /// <summary>
        /// Controls whether to save a policy rule automatically
        /// notify the Watcher when it is added or removed.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="autoNotifyWatcher">Whether to automatically notify watcher.</param>
        public static IEnforcer EnableAutoNotifyWatcher(this IEnforcer enforcer, bool autoNotifyWatcher)
        {
            enforcer.AutoNotifyWatcher = autoNotifyWatcher;
            return enforcer;
        }

        public static IEnforcer EnableCache(this IEnforcer enforcer, bool enableCache)
        {
            enforcer.EnabledCache = enableCache;
            return enforcer;
        }

        public static IEnforcer EnableAutoCleanEnforceCache(this IEnforcer enforcer, bool autoCleanEnforceCache)
        {
            enforcer.AutoCleanEnforceCache = autoCleanEnforceCache;
            return enforcer;
        }
        #endregion

        #region Set extensions
        /// <summary>
        /// Sets the current effector.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="effector"></param>
        public static IEnforcer SetEffector(this IEnforcer enforcer, IEffector effector)
        {
            enforcer.Effector = effector;
            return enforcer;
        }

        /// <summary>
        /// Sets the current model.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="modelPath"></param>
        public static IEnforcer SetModel(this IEnforcer enforcer, string modelPath)
        {
            IModel model = DefaultModel.CreateFromFile(modelPath);
            enforcer.SetModel(model);
            return enforcer;
        }

        /// <summary>
        /// Sets the current model.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="model"></param>
        public static IEnforcer SetModel(this IEnforcer enforcer, IModel model)
        {
            if (enforcer.IsSynchronized)
            {
                model = model.ToSyncModel();
            }
            enforcer.Model = model;
            enforcer.ExpressionHandler = new ExpressionHandler(model);
            if (enforcer.AutoCleanEnforceCache)
            {
                enforcer.EnforceCache?.Clear();
#if !NET45
                enforcer.Logger?.LogInformation("Enforcer Cache, Cleared all enforce cache.");
#endif
            }
            return enforcer;
        }

        /// <summary>
        /// Sets an adapter.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="adapter"></param>
        public static IEnforcer SetAdapter(this IEnforcer enforcer, IAdapter adapter)
        {
            enforcer.Adapter = adapter;
            return enforcer;
        }

        /// <summary>
        /// Sets an watcher.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="watcher"></param>
        /// <param name="useAsync">Whether use async update callback.</param>
        public static IEnforcer SetWatcher(this IEnforcer enforcer, IWatcher watcher, bool useAsync = true)
        {
            enforcer.Watcher = watcher;
            if (useAsync)
            {
                watcher?.SetUpdateCallback(enforcer.LoadPolicyAsync);
                return enforcer;
            }
            watcher?.SetUpdateCallback(enforcer.LoadPolicy);
            return enforcer;
        }

        /// <summary>
        /// Sets the current role manager.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="roleManager"></param>
        public static IEnforcer SetRoleManager(this IEnforcer enforcer, IRoleManager roleManager)
        {
            enforcer.RoleManager = roleManager;
            enforcer.SetRoleManager(PermConstants.DefaultRoleType, roleManager);
            enforcer.ExpressionHandler.SetGFunctions();
            return enforcer;
        }

        /// <summary>
        /// Sets the current role manager.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="roleType"></param>
        /// <param name="roleManager"></param>
        public static IEnforcer SetRoleManager(this IEnforcer enforcer, string roleType, IRoleManager roleManager)
        {
            Assertion assertion = enforcer.Model.Sections[PermConstants.Section.RoleSection][roleType];
            assertion.RoleManager = roleManager;
            if (enforcer.AutoBuildRoleLinks)
            {
                assertion.BuildRoleLinks();
            }
            if (enforcer.AutoCleanEnforceCache)
            {
                enforcer.EnforceCache?.Clear();
            }
            return enforcer;
        }

        /// <summary>
        /// Sets an enforce cache.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="enforceCache"></param>
        public static IEnforcer SetEnforceCache(this IEnforcer enforcer, IEnforceCache enforceCache)
        {
            enforcer.EnforceCache = enforceCache;
            return enforcer;
        }
        #endregion

        #region Poilcy management
        /// <summary>
        /// Reloads the policy from file/database.
        /// </summary>
        public static void LoadPolicy(this IEnforcer enforcer)
        {
            if (enforcer.Adapter is null)
            {
                return;
            }

            enforcer.ClearPolicy();
            enforcer.Adapter.LoadPolicy(enforcer.Model);
            enforcer.Model.RefreshPolicyStringSet();
            if (enforcer.AutoBuildRoleLinks)
            {
                enforcer.BuildRoleLinks();
            }
        }

        /// <summary>
        /// Reloads the policy from file/database.
        /// </summary>
        public static async Task LoadPolicyAsync(this IEnforcer enforcer)
        {
            if (enforcer.Adapter is null)
            {
                return;
            }
            enforcer.ClearPolicy();
            await enforcer.Adapter.LoadPolicyAsync(enforcer.Model);
            if (enforcer.AutoBuildRoleLinks)
            {
                enforcer.BuildRoleLinks();
            }
        }

        /// <summary>
        /// Reloads a filtered policy from file/database.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="filter">The filter used to specify which type of policy should be loaded.</param>
        /// <returns></returns>
        public static bool LoadFilteredPolicy(this IEnforcer enforcer, Filter filter)
        {
            if (enforcer.Adapter is not IFilteredAdapter filteredAdapter)
            {
                throw new NotSupportedException("Filtered policies are not supported by this adapter.");
            }
            enforcer.ClearPolicy();
            filteredAdapter.LoadFilteredPolicy(enforcer.Model, filter);
            if (enforcer.AutoBuildRoleLinks)
            {
                enforcer.BuildRoleLinks();
            }
            return true;
        }

        /// <summary>
        /// Reloads a filtered policy from file/database.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="filter">The filter used to specify which type of policy should be loaded.</param>
        /// <returns></returns>
        public static async Task<bool> LoadFilteredPolicyAsync(this IEnforcer enforcer, Filter filter)
        {
            if (enforcer.Adapter is not IFilteredAdapter filteredAdapter)
            {
                throw new NotSupportedException("Filtered policies are not supported by this adapter.");
            }
            enforcer.ClearPolicy();
            await filteredAdapter.LoadFilteredPolicyAsync(enforcer.Model, filter);
            if (enforcer.AutoBuildRoleLinks)
            {
                enforcer.BuildRoleLinks();
            }
            return true;
        }

        /// <summary>
        /// Saves the current policy (usually after changed with Casbin API)
        /// back to file/database.
        /// </summary>
        public static void SavePolicy(this IEnforcer enforcer)
        {
            if (enforcer.Adapter is null)
            {
                return;
            }

            if (enforcer.IsFiltered)
            {
                throw new InvalidOperationException("Cannot save a filtered policy");
            }
            enforcer.Adapter.SavePolicy(enforcer.Model);
            enforcer.Watcher?.Update();
        }

        /// <summary>
        /// Saves the current policy (usually after changed with Casbin API)
        /// back to file/database.
        /// </summary>
        public static async Task SavePolicyAsync(this IEnforcer enforcer)
        {
            if (enforcer.Adapter is null)
            {
                return;
            }

            if (enforcer.IsFiltered)
            {
                throw new InvalidOperationException("Cannot save a filtered policy");
            }

            await enforcer.Adapter.SavePolicyAsync(enforcer.Model);
            if (enforcer.Watcher is not null)
            {
                await enforcer.Watcher.UpdateAsync();
            }
        }

        /// <summary>
        /// Clears all policy.
        /// </summary>
        public static void ClearPolicy(this IEnforcer enforcer)
        {
            enforcer.Model.ClearPolicy();
            if (enforcer.AutoCleanEnforceCache)
            {
                enforcer.EnforceCache?.Clear();
#if !NET45
                enforcer.Logger?.LogInformation("Enforcer Cache, Cleared all enforce cache.");
#endif
            }
#if !NET45
            enforcer.Logger?.LogInformation("Policy Management, Cleared all policy.");
#endif
        }
        #endregion

        
        /// <summary>
        /// LoadModel reloads the model from the model CONF file. Because the policy is
        /// Attached to a model, so the policy is invalidated and needs to be reloaded by
        /// calling LoadPolicy().
        /// </summary>
        public static void LoadModel(this IEnforcer enforcer)
        {
            if (enforcer.ModelPath is null)
            {
                return;
            }
            enforcer.Model = DefaultModel.CreateFromFile(enforcer.ModelPath);
        }

        /// <summary>
        /// Manually rebuilds the role inheritance relations.
        /// </summary>
        public static void BuildRoleLinks(this IEnforcer enforcer)
        {
            enforcer.RoleManager.Clear();
            enforcer.Model.BuildRoleLinks(enforcer.RoleManager);
        }

        public static Enforcer AddMatchingFunc(this Enforcer enforcer, Func<string, string, bool> func)
        {
            enforcer.AddNamedMatchingFunc(PermConstants.DefaultRoleType, func);
            return enforcer;
        }

        public static Enforcer AddDomainMatchingFunc(this Enforcer enforcer, Func<string, string, bool> func)
        {
            enforcer.AddNamedDomainMatchingFunc(PermConstants.DefaultRoleType, func);
            return enforcer;
        }

        public static Enforcer AddNamedMatchingFunc(this Enforcer enforcer, string roleType, Func<string, string, bool> func)
        {
            enforcer.Model.GetRoleManger(roleType).AddMatchingFunc(func);
            return enforcer;
        }

        public static Enforcer AddNamedDomainMatchingFunc(this Enforcer enforcer, string roleType, Func<string, string, bool> func)
        {
            enforcer.Model.GetRoleManger(roleType).AddMatchingFunc(func);
            return enforcer;
        }
    }
}
