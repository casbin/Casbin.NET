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
        /// <param name="values"></param>
        /// <returns></returns>
        internal static bool InternalAddPolicy(this IEnforcer enforcer, string section, string policyType,
            IPolicyValues values)
        {
            bool ruleAdded = enforcer.PolicyManager.AddPolicy(section, policyType, values);

            if (ruleAdded is false)
            {
                return false;
            }

            OnPolicyChanged(enforcer, PolicyOperation.PolicyAdd, section, policyType, values);
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

            await OnPolicyAsyncChanged(enforcer, PolicyOperation.PolicyAdd, section, policyType, rule);
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

            OnPoliciesChanged(enforcer, PolicyOperation.PolicyAdd, section, policyType, rules);
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

            await OnPoliciesAsyncChanged(enforcer, PolicyOperation.PolicyAdd, section, policyType, rules);
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

            OnPolicyChanged(enforcer, PolicyOperation.PolicyUpdate, section, policyType, oldRule, newRule);
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

            await OnPolicyAsyncChanged(enforcer, PolicyOperation.PolicyUpdate, section, policyType, oldRule, newRule);
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

            OnPoliciesChanged(enforcer, PolicyOperation.PolicyUpdate, section, policyType, oldRules, newRules);
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

            await OnPoliciesAsyncChanged(enforcer, PolicyOperation.PolicyUpdate, section, policyType, oldRules,
                newRules);
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

            OnPoliciesChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, new[] { rule });
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

            await OnPolicyAsyncChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, rule);
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

            OnPoliciesChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, rules);
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

            await OnPoliciesAsyncChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, rules);
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
            IEnumerable<IPolicyValues> effectPolices =
                enforcer.PolicyManager.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);

            if (effectPolices is null)
            {
                return false;
            }

            OnPoliciesChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, effectPolices);
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
                enforcer.Logger?.LogInformation("Enforcer Cache, Cleared all enforce cache.");
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
                enforcer.Logger?.LogInformation("Enforcer Cache, Cleared all enforce cache.");
#endif
            }

            if (enforcer.AutoNotifyWatcher && enforcer.Watcher is not null)
            {
                await enforcer.Watcher.UpdateAsync();
            }
        }
    }
}
