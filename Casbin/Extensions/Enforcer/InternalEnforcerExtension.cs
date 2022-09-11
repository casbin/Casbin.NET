using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;
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

            OnPolicyChanged(enforcer, WatcherMessage.CreateAddPolicyMessage(section, policyType, values));
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

            await OnPolicyAsyncChanged(enforcer, WatcherMessage.CreateAddPolicyMessage(section, policyType, values));
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

            OnPoliciesChanged(enforcer, WatcherMessage.CreateAddPoliciesMessage(section, policyType, valuesList));
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

            await OnPoliciesAsyncChanged(enforcer, WatcherMessage.CreateAddPoliciesMessage(section, policyType, valuesList));
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

            OnPolicyChanged(enforcer, WatcherMessage.CreateUpdatePolicyMessage(section, policyType, oldValues, newValues));
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

            await OnPolicyAsyncChanged(enforcer,
                WatcherMessage.CreateUpdatePolicyMessage(section, policyType, oldValues, newValues));
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

            OnPoliciesChanged(enforcer,
                WatcherMessage.CreateUpdatePoliciesMessage(section, policyType, oldValuesList, newValuesList));
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

            await OnPoliciesAsyncChanged(enforcer,
                WatcherMessage.CreateUpdatePoliciesMessage(section, policyType, oldValuesList, newValuesList));
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

            OnPolicyChanged(enforcer, WatcherMessage.CreateRemovePolicyMessage(section, policyType, values));
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

            await OnPolicyAsyncChanged(enforcer, WatcherMessage.CreateRemovePolicyMessage(section, policyType, rule));
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

            OnPoliciesChanged(enforcer, WatcherMessage.CreateRemovePoliciesMessage(section, policyType, rules));
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

            await OnPoliciesAsyncChanged(enforcer,
                WatcherMessage.CreateRemovePoliciesMessage(section, policyType, rules));
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
            IEnumerable<IPolicyValues> effectPolicies = policyManager.RemoveFilteredPolicy(fieldIndex, fieldValues);

            if (effectPolicies is null)
            {
                return false;
            }

            IReadOnlyList<IPolicyValues> rules = effectPolicies as IReadOnlyList<IPolicyValues> ?? effectPolicies.ToList();
            OnPoliciesChanged(enforcer,
                WatcherMessage.CreateRemoveFilteredPolicyMessage(section, policyType, fieldIndex, rules));
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

            IReadOnlyList<IPolicyValues> rules = effectPolicies as IReadOnlyList<IPolicyValues> ?? effectPolicies.ToList();
            await OnPoliciesAsyncChanged(enforcer,
                WatcherMessage.CreateRemoveFilteredPolicyMessage(section, policyType, fieldIndex, rules));
            return true;
        }

        private static void OnPolicyChanged(IEnforcer enforcer, WatcherMessage watcherMessage)
        {
            if (watcherMessage is null)
            {
                return;
            }

            if (watcherMessage.Section.Equals(PermConstants.Section.RoleSection))
            {
                if (watcherMessage.NewValues is null)
                {
                    enforcer.Model.BuildIncrementalRoleLink(watcherMessage.Operation, watcherMessage.Section,
                        watcherMessage.PolicyType, watcherMessage.Values);
                }
                else
                {
                    enforcer.Model.BuildIncrementalRoleLink(watcherMessage.Operation, watcherMessage.Section,
                        watcherMessage.PolicyType, watcherMessage.Values, watcherMessage.NewValues);
                }
            }

            NotifyPolicyChanged(enforcer, watcherMessage);
        }

        private static async Task OnPolicyAsyncChanged(IEnforcer enforcer, WatcherMessage watcherMessage)
        {
            if (watcherMessage is null)
            {
                return;
            }

            if (watcherMessage.Section.Equals(PermConstants.Section.RoleSection))
            {
                if (watcherMessage.NewValues is null)
                {
                    enforcer.Model.BuildIncrementalRoleLink(watcherMessage.Operation, watcherMessage.Section,
                        watcherMessage.PolicyType, watcherMessage.Values);
                }
                else
                {
                    enforcer.Model.BuildIncrementalRoleLink(watcherMessage.Operation, watcherMessage.Section,
                        watcherMessage.PolicyType, watcherMessage.Values, watcherMessage.NewValues);
                }
            }

            await NotifyPolicyChangedAsync(enforcer, watcherMessage);
        }

        private static void OnPoliciesChanged(IEnforcer enforcer, WatcherMessage watcherMessage)
        {
            if (watcherMessage is null)
            {
                return;
            }

            if (watcherMessage.Section.Equals(PermConstants.Section.RoleSection))
            {
                if (watcherMessage.NewValuesList is null)
                {
                    enforcer.Model.BuildIncrementalRoleLinks(watcherMessage.Operation, watcherMessage.Section,
                        watcherMessage.PolicyType, watcherMessage.ValuesList);
                }
                else
                {
                    enforcer.Model.BuildIncrementalRoleLinks(watcherMessage.Operation, watcherMessage.Section,
                        watcherMessage.PolicyType, watcherMessage.ValuesList, watcherMessage.NewValuesList);
                }
            }

            NotifyPolicyChanged(enforcer, watcherMessage);
        }

        private static async Task OnPoliciesAsyncChanged(IEnforcer enforcer, WatcherMessage watcherMessage)
        {
            if (watcherMessage is null)
            {
                return;
            }

            if (watcherMessage.Section.Equals(PermConstants.Section.RoleSection))
            {
                if (watcherMessage.NewValuesList is null)
                {
                    enforcer.Model.BuildIncrementalRoleLinks(watcherMessage.Operation, watcherMessage.Section,
                        watcherMessage.PolicyType, watcherMessage.ValuesList);
                }
                else
                {
                    enforcer.Model.BuildIncrementalRoleLinks(watcherMessage.Operation, watcherMessage.Section,
                        watcherMessage.PolicyType, watcherMessage.ValuesList, watcherMessage.NewValuesList);
                }

            }

            await NotifyPolicyChangedAsync(enforcer, watcherMessage);
        }

        private static void NotifyPolicyChanged(IEnforcer enforcer, WatcherMessage watcherMessage)
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
                enforcer.Watcher?.Update(watcherMessage);
            }
        }

        private static async Task NotifyPolicyChangedAsync(IEnforcer enforcer, WatcherMessage watcherMessage)
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
                await enforcer.Watcher.UpdateAsync(watcherMessage);
            }
        }
    }
}
