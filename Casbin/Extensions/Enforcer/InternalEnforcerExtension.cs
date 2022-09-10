using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.Model;
#if !NET452
using Microsoft.Extensions.Logging;
#endif

namespace Casbin
{
    internal static class InternalEnforcerExtension
    {
        internal static IEnumerable<IPolicyValues> InternalGetPolicy(this IEnforcer enforcer, string section,
            string policyType) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.GetPolicy(section, policyType);

        internal static IEnumerable<IPolicyValues> InternalGetFilteredPolicy(this IEnforcer enforcer,
            string section, string policyType, int fieldIndex, IPolicyValues fieldValues) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.GetFilteredPolicy(section, policyType, fieldIndex,
                fieldValues);

        internal static IEnumerable<string> InternalGetValuesForFieldInPolicy(this IEnforcer enforcer, string section,
            string policyType, int fieldIndex) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.GetValuesForFieldInPolicy(section, policyType, fieldIndex);

        internal static IEnumerable<string> InternalGetValuesForFieldInPolicyAllTypes(this IEnforcer enforcer,
            string section, int fieldIndex) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.GetValuesForFieldInPolicyAllTypes(section, fieldIndex);

        internal static bool InternalHasPolicy(this IEnforcer enforcer, string section, string policyType,
            IPolicyValues rule) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.HasPolicy(section, policyType, rule);

        internal static bool InternalHasPolicies(this IEnforcer enforcer, string section, string policyType,
            IReadOnlyList<IPolicyValues> rules) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.HasPolicies(section, policyType, rules);

        /// <summary>
        /// Adds a values to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static bool InternalAddPolicy(this IEnforcer enforcer, string section, string policyType,
            IPolicyValues values)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManger(section, policyType);
            bool ruleAdded = policyManager.AddPolicy(values);

            if (ruleAdded is false)
            {
                return false;
            }

            OnPolicyChanged(enforcer, PolicyOperation.PolicyAdd, section, policyType, values);
            return true;
        }

        /// <summary>
        /// Adds a values to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalAddPolicyAsync(this IEnforcer enforcer, string section,
            string policyType, IPolicyValues values)
        {
            IPolicyManager policyManger = enforcer.Model.GetPolicyManger(section, policyType);
            if (policyManger.HasPolicy(values))
            {
                return false;
            }

            bool ruleAdded = await policyManger.AddPolicyAsync(values);

            if (ruleAdded is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer, PolicyOperation.PolicyAdd, section, policyType, values);
            return true;
        }

        /// <summary>
        /// Adds valuesList to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="valuesList"></param>
        /// <returns></returns>
        internal static bool InternalAddPolicies(this IEnforcer enforcer, string section, string policyType,
            IReadOnlyList<IPolicyValues> valuesList)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManger(section, policyType);
            if (policyManager.HasPolicies(valuesList))
            {
                return false;
            }

            bool ruleAdded = policyManager.AddPolicies(valuesList);

            if (ruleAdded is false)
            {
                return false;
            }

            OnPoliciesChanged(enforcer, PolicyOperation.PolicyAdd, section, policyType, valuesList);
            return true;
        }


        /// <summary>
        /// Adds valuesList to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="valuesList"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalAddPoliciesAsync(this IEnforcer enforcer, string section,
            string policyType, IReadOnlyList<IPolicyValues> valuesList)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManger(section, policyType);
            if (policyManager.HasPolicies(valuesList))
            {
                return false;
            }

            bool ruleAdded = await policyManager.AddPoliciesAsync(valuesList);

            if (ruleAdded is false)
            {
                return false;
            }

            await OnPoliciesAsyncChanged(enforcer, PolicyOperation.PolicyAdd, section, policyType, valuesList);
            return true;
        }

        /// <summary>
        ///     Updates a values from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldValues"></param>
        /// <param name="newValues"></param>
        /// <returns></returns>
        internal static bool InternalUpdatePolicy(this IEnforcer enforcer, string section, string policyType,
            IPolicyValues oldValues, IPolicyValues newValues)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManger(section, policyType);
            if (policyManager.HasPolicy(oldValues) is false)
            {
                return false;
            }

            bool ruleUpdated = policyManager.UpdatePolicy(oldValues, newValues);

            if (ruleUpdated is false)
            {
                return false;
            }

            OnPolicyChanged(enforcer, PolicyOperation.PolicyUpdate, section, policyType, oldValues, newValues);
            return true;
        }

        /// <summary>
        /// Updates a values from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldValues"></param>
        /// <param name="newValues"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalUpdatePolicyAsync(this IEnforcer enforcer, string section,
            string policyType, IPolicyValues oldValues, IPolicyValues newValues)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManger(section, policyType);
            if (policyManager.HasPolicy(oldValues) is false)
            {
                return false;
            }

            bool ruleUpdated = await policyManager.UpdatePolicyAsync(oldValues, newValues);

            if (ruleUpdated is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer, PolicyOperation.PolicyUpdate, section, policyType, oldValues,
                newValues);
            return true;
        }

        /// <summary>
        ///     Updates valuesList from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldValuesList"></param>
        /// <param name="newValuesList"></param>
        /// <returns></returns>
        internal static bool InternalUpdatePolicies(this IEnforcer enforcer, string section, string policyType,
            IReadOnlyList<IPolicyValues> oldValuesList, IReadOnlyList<IPolicyValues> newValuesList)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManger(section, policyType);
            if (policyManager.HasAllPolicies(oldValuesList) is false)
            {
                return false;
            }

            bool ruleUpdated = policyManager.UpdatePolicies(oldValuesList, newValuesList);

            if (ruleUpdated is false)
            {
                return false;
            }

            OnPoliciesChanged(enforcer, PolicyOperation.PolicyUpdate, section, policyType, oldValuesList,
                newValuesList);
            return true;
        }

        /// <summary>
        ///     Updates valuesList from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldValuesList"></param>
        /// <param name="newValuesList"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalUpdatePoliciesAsync(this IEnforcer enforcer, string section,
            string policyType, IReadOnlyList<IPolicyValues> oldValuesList, IReadOnlyList<IPolicyValues> newValuesList)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManger(section, policyType);
            if (policyManager.HasAllPolicies(oldValuesList) is false)
            {
                return false;
            }

            bool ruleUpdated = await policyManager.UpdatePoliciesAsync(oldValuesList, newValuesList);

            if (ruleUpdated is false)
            {
                return false;
            }

            await OnPoliciesAsyncChanged(enforcer, PolicyOperation.PolicyUpdate, section, policyType, oldValuesList,
                newValuesList);
            return true;
        }

        /// <summary>
        /// Removes a values from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static bool InternalRemovePolicy(this IEnforcer enforcer, string section, string policyType,
            IPolicyValues values)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManger(section, policyType);
            if (policyManager.HasPolicy(values) is false)
            {
                return false;
            }

            bool ruleRemoved = policyManager.RemovePolicy(values);

            if (ruleRemoved is false)
            {
                return false;
            }

            OnPoliciesChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, new[] { values });
            return true;
        }

        /// <summary>
        /// Removes a values from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalRemovePolicyAsync(this IEnforcer enforcer, string section,
            string policyType, IPolicyValues rule)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManger(section, policyType);
            if (policyManager.HasPolicy(rule) is false)
            {
                return false;
            }

            bool ruleRemoved = await policyManager.RemovePolicyAsync(rule);

            if (ruleRemoved is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, rule);
            return true;
        }

        /// <summary>
        /// Removes valuesList from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        internal static bool InternalRemovePolicies(this IEnforcer enforcer, string section, string policyType,
            IReadOnlyList<IPolicyValues> rules)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManger(section, policyType);
            if (policyManager.HasPolicies(rules) is false)
            {
                return false;
            }

            bool ruleRemoved = policyManager.RemovePolicies(rules);

            if (ruleRemoved is false)
            {
                return false;
            }

            OnPoliciesChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, rules);
            return true;
        }

        /// <summary>
        /// Removes valuesList from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalRemovePoliciesAsync(this IEnforcer enforcer, string section,
            string policyType, IReadOnlyList<IPolicyValues> rules)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManger(section, policyType);
            if (policyManager.HasPolicies(rules) is false)
            {
                return false;
            }

            bool ruleRemoved = await policyManager.RemovePoliciesAsync(rules);

            if (ruleRemoved is false)
            {
                return false;
            }

            await OnPoliciesAsyncChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, rules);
            return true;
        }

        /// <summary>
        /// Removes valuesList based on field filters from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        internal static bool InternalRemoveFilteredPolicy(this IEnforcer enforcer, string section, string policyType,
            int fieldIndex, IPolicyValues fieldValues)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManger(section, policyType);
            IEnumerable<IPolicyValues> effectPolices = policyManager.RemoveFilteredPolicy(fieldIndex, fieldValues);

            if (effectPolices is null)
            {
                return false;
            }

            OnPoliciesChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, effectPolices);
            return true;
        }

        /// <summary>
        /// Removes valuesList based on field filters from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalRemoveFilteredPolicyAsync(this IEnforcer enforcer, string section,
            string policyType, int fieldIndex, IPolicyValues fieldValues)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManger(section, policyType);
            IEnumerable<IPolicyValues> effectPolicies =
                await policyManager.RemoveFilteredPolicyAsync(fieldIndex, fieldValues);

            if (effectPolicies is null)
            {
                return false;
            }

            await OnPoliciesAsyncChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, effectPolicies);
            return true;
        }

        private static void OnPolicyChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IPolicyValues values)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLink(policyOperation, section, policyType, values);
            }

            NotifyPolicyChanged(enforcer);
        }

        private static void OnPolicyChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IPolicyValues oldRule, IPolicyValues newRule)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLink(policyOperation, section, policyType, oldRule, newRule);
            }

            NotifyPolicyChanged(enforcer);
        }

        private static async Task OnPolicyAsyncChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IPolicyValues rule)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLink(policyOperation, section, policyType, rule);
            }

            await NotifyPolicyChangedAsync(enforcer);
        }

        private static async Task OnPolicyAsyncChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IPolicyValues oldRule, IPolicyValues newRule)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLink(policyOperation, section, policyType, oldRule, newRule);
            }

            await NotifyPolicyChangedAsync(enforcer);
        }

        private static void OnPoliciesChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<IPolicyValues> rules)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLinks(policyOperation, section, policyType, rules);
            }

            NotifyPolicyChanged(enforcer);
        }

        private static void OnPoliciesChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<IPolicyValues> oldRules, IEnumerable<IPolicyValues> newRules)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLinks(policyOperation, section, policyType, oldRules, newRules);
            }

            NotifyPolicyChanged(enforcer);
        }

        private static async Task OnPoliciesAsyncChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<IPolicyValues> rules)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLinks(policyOperation, section, policyType, rules);
            }

            await NotifyPolicyChangedAsync(enforcer);
        }

        private static async Task OnPoliciesAsyncChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<IPolicyValues> oldRules,
            IEnumerable<IPolicyValues> newRules)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLinks(policyOperation, section, policyType, oldRules, newRules);
            }

            await NotifyPolicyChangedAsync(enforcer);
        }

        private static void NotifyPolicyChanged(IEnforcer enforcer)
        {
            if (enforcer.AutoCleanEnforceCache)
            {
                enforcer.EnforceCache?.Clear();
#if !NET452
                enforcer.Logger?.LogInformation("Enforcer Cache, Cleared all enforce cache");
#endif
            }

            if (enforcer.AutoNotifyWatcher)
            {
                enforcer.Watcher?.Update();
            }
        }

        private static async Task NotifyPolicyChangedAsync(IEnforcer enforcer)
        {
            if (enforcer.AutoCleanEnforceCache && enforcer.EnforceCache is not null)
            {
                await enforcer.EnforceCache.ClearAsync();
#if !NET452
                enforcer.Logger?.LogInformation("Enforcer Cache, Cleared all enforce cache");
#endif
            }

            if (enforcer.AutoNotifyWatcher && enforcer.Watcher is not null)
            {
                await enforcer.Watcher.UpdateAsync();
            }
        }
    }
}
