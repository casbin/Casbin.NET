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
            string policyType)
        {
            return enforcer.PolicyManager.GetPolicy(section, policyType);
        }

        internal static IEnumerable<IPolicyValues> InternalGetFilteredPolicy(this IEnforcer enforcer,
            string section, string policyType, int fieldIndex, IPolicyValues fieldValues)
        {
            return enforcer.PolicyManager.GetFilteredPolicy(section, policyType, fieldIndex, fieldValues);
        }

        internal static IEnumerable<string> InternalGetValuesForFieldInPolicy(this IEnforcer enforcer, string section,
            string policyType, int fieldIndex)
        {
            return enforcer.PolicyManager.GetValuesForFieldInPolicy(section, policyType, fieldIndex);
        }

        internal static IEnumerable<string> InternalGetValuesForFieldInPolicyAllTypes(this IEnforcer enforcer,
            string section, int fieldIndex)
        {
            return enforcer.PolicyManager.GetValuesForFieldInPolicyAllTypes(section, fieldIndex);
        }

        internal static bool InternalHasPolicy(this IEnforcer enforcer, string section, string policyType,
            IPolicyValues rule)
        {
            return enforcer.PolicyManager.HasPolicy(section, policyType, rule);
        }

        internal static bool InternalHasPolicies(this IEnforcer enforcer, string section, string policyType,
            IReadOnlyList<IPolicyValues> rules)
        {
            return enforcer.PolicyManager.HasPolicies(section, policyType, rules);
        }

        /// <summary>
        /// Adds a rule to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        internal static bool InternalAddPolicy(this IEnforcer enforcer, string section, string policyType,
            IPolicyValues rule)
        {
            bool ruleAdded = enforcer.PolicyManager.AddPolicy(section, policyType, rule);

            if (ruleAdded is false)
            {
                return false;
            }

            OnPolicyChanged(enforcer, WatcherMessage.CreateAddPolicyMessage(section, policyType, rule));
            return true;
        }

        /// <summary>
        /// Adds a rule to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalAddPolicyAsync(this IEnforcer enforcer, string section,
            string policyType, IPolicyValues rule)
        {
            if (enforcer.PolicyManager.HasPolicy(section, policyType, rule))
            {
                return false;
            }

            bool ruleAdded = await enforcer.PolicyManager.AddPolicyAsync(section, policyType, rule);

            if (ruleAdded is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer, WatcherMessage.CreateAddPolicyMessage(section, policyType, rule));
            return true;
        }

        /// <summary>
        /// Adds rules to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        internal static bool InternalAddPolicies(this IEnforcer enforcer, string section, string policyType,
            IReadOnlyList<IPolicyValues> rules)
        {
            if (enforcer.PolicyManager.HasPolicies(section, policyType, rules))
            {
                return false;
            }

            bool ruleAdded = enforcer.PolicyManager.AddPolicies(section, policyType, rules);

            if (ruleAdded is false)
            {
                return false;
            }

            OnPoliciesChanged(enforcer, WatcherMessage.CreateAddPoliciesMessage(section, policyType, rules));
            return true;
        }


        /// <summary>
        /// Adds rules to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalAddPoliciesAsync(this IEnforcer enforcer, string section,
            string policyType, IReadOnlyList<IPolicyValues> rules)
        {
            if (enforcer.PolicyManager.HasPolicies(section, policyType, rules))
            {
                return false;
            }

            bool ruleAdded = await enforcer.PolicyManager.AddPoliciesAsync(section, policyType, rules);

            if (ruleAdded is false)
            {
                return false;
            }

            await OnPoliciesAsyncChanged(enforcer, WatcherMessage.CreateAddPoliciesMessage(section, policyType, rules));
            return true;
        }

        /// <summary>
        ///     Updates a rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldRule"></param>
        /// <param name="newRule"></param>
        /// <returns></returns>
        internal static bool InternalUpdatePolicy(this IEnforcer enforcer, string section, string policyType,
            IPolicyValues oldRule, IPolicyValues newRule)
        {
            if (enforcer.PolicyManager.HasPolicy(section, policyType, oldRule) is false)
            {
                return false;
            }

            bool ruleUpdated = enforcer.PolicyManager.UpdatePolicy(section, policyType, oldRule, newRule);

            if (ruleUpdated is false)
            {
                return false;
            }

            OnPolicyChanged(enforcer, WatcherMessage.CreateUpdatePolicyMessage(section, policyType, oldRule, newRule));
            return true;
        }

        /// <summary>
        ///     Updates a rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldRule"></param>
        /// <param name="newRule"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalUpdatePolicyAsync(this IEnforcer enforcer, string section,
            string policyType, IPolicyValues oldRule, IPolicyValues newRule)
        {
            if (enforcer.PolicyManager.HasPolicy(section, policyType, oldRule) is false)
            {
                return false;
            }

            bool ruleUpdated = await enforcer.PolicyManager.UpdatePolicyAsync(section, policyType, oldRule, newRule);

            if (ruleUpdated is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer,
                WatcherMessage.CreateUpdatePolicyMessage(section, policyType, oldRule, newRule));
            return true;
        }

        /// <summary>
        ///     Updates rules from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldRules"></param>
        /// <param name="newRules"></param>
        /// <returns></returns>
        internal static bool InternalUpdatePolicies(this IEnforcer enforcer, string section, string policyType,
            IReadOnlyList<IPolicyValues> oldRules, IReadOnlyList<IPolicyValues> newRules)
        {
            if (enforcer.PolicyManager.HasAllPolicies(section, policyType, oldRules) is false)
            {
                return false;
            }

            bool ruleUpdated = enforcer.PolicyManager.UpdatePolicies(section, policyType, oldRules, newRules);

            if (ruleUpdated is false)
            {
                return false;
            }

            OnPoliciesChanged(enforcer,
                WatcherMessage.CreateUpdatePoliciesMessage(section, policyType, oldRules, newRules));
            return true;
        }

        /// <summary>
        ///     Updates rules from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldRules"></param>
        /// <param name="newRules"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalUpdatePoliciesAsync(this IEnforcer enforcer, string section,
            string policyType, IReadOnlyList<IPolicyValues> oldRules, IReadOnlyList<IPolicyValues> newRules)
        {
            if (enforcer.PolicyManager.HasAllPolicies(section, policyType, oldRules) is false)
            {
                return false;
            }

            bool ruleUpdated = await enforcer.PolicyManager.UpdatePoliciesAsync(section, policyType, oldRules, newRules);

            if (ruleUpdated is false)
            {
                return false;
            }

            await OnPoliciesAsyncChanged(enforcer,
                WatcherMessage.CreateUpdatePoliciesMessage(section, policyType, oldRules, newRules));
            return true;
        }

        /// <summary>
        /// Removes a rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        internal static bool InternalRemovePolicy(this IEnforcer enforcer, string section, string policyType,
            IPolicyValues rule)
        {
            if (enforcer.PolicyManager.HasPolicy(section, policyType, rule) is false)
            {
                return false;
            }

            bool ruleRemoved = enforcer.PolicyManager.RemovePolicy(section, policyType, rule);

            if (ruleRemoved is false)
            {
                return false;
            }

            OnPolicyChanged(enforcer, WatcherMessage.CreateRemovePolicyMessage(section, policyType, rule));
            return true;
        }

        /// <summary>
        /// Removes a rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalRemovePolicyAsync(this IEnforcer enforcer, string section,
            string policyType, IPolicyValues rule)
        {
            if (enforcer.PolicyManager.HasPolicy(section, policyType, rule) is false)
            {
                return false;
            }

            bool ruleRemoved = await enforcer.PolicyManager.RemovePolicyAsync(section, policyType, rule);

            if (ruleRemoved is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer, WatcherMessage.CreateRemovePolicyMessage(section, policyType, rule));
            return true;
        }

        /// <summary>
        /// Removes rules from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        internal static bool InternalRemovePolicies(this IEnforcer enforcer, string section, string policyType,
            IReadOnlyList<IPolicyValues> rules)
        {
            if (enforcer.PolicyManager.HasPolicies(section, policyType, rules) is false)
            {
                return false;
            }

            bool ruleRemoved = enforcer.PolicyManager.RemovePolicies(section, policyType, rules);

            if (ruleRemoved is false)
            {
                return false;
            }

            OnPoliciesChanged(enforcer, WatcherMessage.CreateRemovePoliciesMessage(section, policyType, rules));
            return true;
        }

        /// <summary>
        /// Removes rules from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalRemovePoliciesAsync(this IEnforcer enforcer, string section,
            string policyType, IReadOnlyList<IPolicyValues> rules)
        {
            if (enforcer.PolicyManager.HasPolicies(section, policyType, rules) is false)
            {
                return false;
            }

            bool ruleRemoved = await enforcer.PolicyManager.RemovePoliciesAsync(section, policyType, rules);

            if (ruleRemoved is false)
            {
                return false;
            }

            await OnPoliciesAsyncChanged(enforcer,
                WatcherMessage.CreateRemovePoliciesMessage(section, policyType, rules));
            return true;
        }

        /// <summary>
        /// Removes rules based on field filters from the current policy.
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
            IEnumerable<IPolicyValues> effectPolicies =
                enforcer.PolicyManager.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);

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
        /// Removes rules based on field filters from the current policy.
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
            IEnumerable<IPolicyValues> effectPolicies =
                await enforcer.PolicyManager.RemoveFilteredPolicyAsync(section, policyType, fieldIndex, fieldValues);

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
                enforcer.Logger?.LogInformation("Enforcer Cache, Cleared all enforce cache.");
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
                enforcer.Logger?.LogInformation("Enforcer Cache, Cleared all enforce cache.");
#endif
            }

            if (enforcer.AutoNotifyWatcher && enforcer.Watcher is not null)
            {
                await enforcer.Watcher.UpdateAsync(watcherMessage);
            }
        }
    }
}
