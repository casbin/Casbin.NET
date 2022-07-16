using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if !NET452
using Microsoft.Extensions.Logging;
#endif

namespace Casbin
{
    internal static class InternalEnforcerExtension
    {
        internal static IEnumerable<IEnumerable<string>> InternalGetPolicy(this IEnforcer enforcer, string section, string policyType)
        {
            return enforcer.PolicyManager.GetPolicy(section, policyType);
        }

        internal static IEnumerable<IEnumerable<string>> InternalGetFilteredPolicy(this IEnforcer enforcer,
            string section, string policyType, int fieldIndex,
            params string[] fieldValues)
        {
            return enforcer.PolicyManager.GetFilteredPolicy(section, policyType, fieldIndex, fieldValues);
        }

        internal static IEnumerable<string> InternalGetValuesForFieldInPolicy(this IEnforcer enforcer, string section, string policyType, int fieldIndex)
        {
            return enforcer.PolicyManager.GetValuesForFieldInPolicy(section, policyType, fieldIndex);
        }

        internal static IEnumerable<string> InternalGetValuesForFieldInPolicyAllTypes(this IEnforcer enforcer, string section, int fieldIndex)
        {
            return enforcer.PolicyManager.GetValuesForFieldInPolicyAllTypes(section, fieldIndex);
        }

        internal static bool InternalHasPolicy(this IEnforcer enforcer, string section, string policyType,
            IEnumerable<string> rule)
        {
            return enforcer.PolicyManager.HasPolicy(section, policyType, rule);
        }

        internal static bool InternalHasPolicies(this IEnforcer enforcer, string section, string policyType,
            IEnumerable<IEnumerable<string>> rules)
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
        internal static bool InternalAddPolicy(this IEnforcer enforcer, string section, string policyType, IEnumerable<string> rule)
        {
            IEnumerable<string> ruleArray = rule as string[] ?? rule.ToArray();

            bool ruleAdded = enforcer.PolicyManager.AddPolicy(section, policyType, ruleArray);

            if (ruleAdded is false)
            {
                return false;
            }

            OnPolicyChanged(enforcer, PolicyOperation.PolicyAdd, section, policyType, ruleArray);
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
        internal static async Task<bool> InternalAddPolicyAsync(this IEnforcer enforcer, string section, string policyType, IEnumerable<string> rule)
        {
            IEnumerable<string> ruleArray = rule as string[] ?? rule.ToArray();
            if (enforcer.PolicyManager.HasPolicy(section, policyType, ruleArray))
            {
                return false;
            }

            bool ruleAdded = await enforcer.PolicyManager.AddPolicyAsync(section, policyType, ruleArray);

            if (ruleAdded is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer, PolicyOperation.PolicyAdd, section, policyType, ruleArray);
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
        internal static bool InternalAddPolicies(this IEnforcer enforcer, string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            var ruleArray = rules as IEnumerable<string>[] ?? rules.ToArray();

            if (enforcer.PolicyManager.HasPolicies(section, policyType, ruleArray))
            {
                return false;
            }

            bool ruleAdded = enforcer.PolicyManager.AddPolicies(section, policyType, ruleArray);

            if (ruleAdded is false)
            {
                return false;
            }

            OnPoliciesChanged(enforcer, PolicyOperation.PolicyAdd, section, policyType, ruleArray);
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
        internal static async Task<bool> InternalAddPoliciesAsync(this IEnforcer enforcer, string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            var rulesArray = rules as IEnumerable<string>[] ?? rules.ToArray();

            if (enforcer.PolicyManager.HasPolicies(section, policyType, rulesArray))
            {
                return false;
            }

            bool ruleAdded = await enforcer.PolicyManager.AddPoliciesAsync(section, policyType, rulesArray);

            if (ruleAdded is false)
            {
                return false;
            }

            await OnPoliciesAsyncChanged(enforcer, PolicyOperation.PolicyAdd, section, policyType, rulesArray);
            return true;
        }

        /// <summary>
        /// Updates a rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldRule"></param>
        /// <param name="newRule"></param>
        /// <returns></returns>
        internal static bool InternalUpdatePolicy(this IEnforcer enforcer, string section, string policyType, IEnumerable<string> oldRule, IEnumerable<string> newRule)
        {
            IEnumerable<string> oldRuleArray = oldRule as string[] ?? oldRule.ToArray();
            if (enforcer.PolicyManager.HasPolicy(section, policyType, oldRuleArray) is false)
            {
                return false;
            }

            IEnumerable<string> newRuleArray = newRule as string[] ?? newRule.ToArray();
            bool ruleUpdated = enforcer.PolicyManager.UpdatePolicy(section, policyType, oldRuleArray, newRuleArray);

            if (ruleUpdated is false)
            {
                return false;
            }

            OnPolicyChanged(enforcer, PolicyOperation.PolicyUpdate, section, policyType, oldRuleArray, newRuleArray);
            return true;
        }

        /// <summary>
        /// Updates a rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldRule"></param>
        /// <param name="newRule"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalUpdatePolicyAsync(this IEnforcer enforcer, string section, string policyType, IEnumerable<string> oldRule, IEnumerable<string> newRule)
        {
            IEnumerable<string> oldRuleArray = oldRule as string[] ?? oldRule.ToArray();
            if (enforcer.PolicyManager.HasPolicy(section, policyType, oldRuleArray) is false)
            {
                return false;
            }

            IEnumerable<string> newRuleArray = newRule as string[] ?? newRule.ToArray();
            bool ruleUpdated = await enforcer.PolicyManager.UpdatePolicyAsync(section, policyType, oldRuleArray, newRuleArray);

            if (ruleUpdated is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer, PolicyOperation.PolicyUpdate, section, policyType, oldRuleArray, newRuleArray);
            return true;
        }

        /// <summary>
        /// Updates rules from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldRules"></param>
        /// <param name="newRules"></param>
        /// <returns></returns>
        internal static bool InternalUpdatePolicies(this IEnforcer enforcer, string section, string policyType,
            IEnumerable<IEnumerable<string>> oldRules, IEnumerable<IEnumerable<string>> newRules)
        {
            var oldRulesArray = oldRules as IEnumerable<string>[] ?? oldRules.ToArray();

            if (enforcer.PolicyManager.HasAllPolicies(section, policyType, oldRulesArray) is false)
            {
                return false;
            }

            var newRulesArray = newRules as IEnumerable<string>[] ?? newRules.ToArray();
            bool ruleUpdated = enforcer.PolicyManager.UpdatePolicies(section, policyType, oldRulesArray, newRulesArray);

            if (ruleUpdated is false)
            {
                return false;
            }

            OnPoliciesChanged(enforcer, PolicyOperation.PolicyUpdate, section, policyType, oldRulesArray, newRulesArray);
            return true;
        }

        /// <summary>
        /// Updates rules from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldRules"></param>
        /// <param name="newRules"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalUpdatePoliciesAsync(this IEnforcer enforcer, string section,
            string policyType, IEnumerable<IEnumerable<string>> oldRules, IEnumerable<IEnumerable<string>> newRules)
        {
            var oldRulesArray = oldRules as IEnumerable<string>[] ?? oldRules.ToArray();

            if (enforcer.PolicyManager.HasAllPolicies(section, policyType, oldRulesArray) is false)
            {
                return false;
            }

            var newRulesArray = newRules as IEnumerable<string>[] ?? newRules.ToArray();
            bool ruleUpdated = await enforcer.PolicyManager.UpdatePoliciesAsync(section, policyType, oldRulesArray, newRulesArray);

            if (ruleUpdated is false)
            {
                return false;
            }

            await OnPoliciesAsyncChanged(enforcer, PolicyOperation.PolicyUpdate, section, policyType, oldRulesArray, newRulesArray);
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
        internal static bool InternalRemovePolicy(this IEnforcer enforcer, string section, string policyType, IEnumerable<string> rule)
        {
            IEnumerable<string> ruleArray = rule as string[] ?? rule.ToArray();
            if (enforcer.PolicyManager.HasPolicy(section, policyType, ruleArray) is false)
            {
                return false;
            }

            bool ruleRemoved = enforcer.PolicyManager.RemovePolicy(section, policyType, ruleArray);

            if (ruleRemoved is false)
            {
                return false;
            }

            OnPoliciesChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, new [] {ruleArray});
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
        internal static async Task<bool> InternalRemovePolicyAsync(this IEnforcer enforcer, string section, string policyType, IEnumerable<string> rule)
        {
            IEnumerable<string> ruleArray = rule as string[] ?? rule.ToArray();
            if (enforcer.PolicyManager.HasPolicy(section, policyType, ruleArray) is false)
            {
                return false;
            }

            bool ruleRemoved = await enforcer.PolicyManager.RemovePolicyAsync(section, policyType, ruleArray);

            if (ruleRemoved is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, ruleArray);
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
        internal static bool InternalRemovePolicies(this IEnforcer enforcer, string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            var rulesArray = rules as IEnumerable<string>[] ?? rules.ToArray();

            if (enforcer.PolicyManager.HasPolicies(section, policyType, rulesArray) is false)
            {
                return false;
            }

            bool ruleRemoved = enforcer.PolicyManager.RemovePolicies(section, policyType, rulesArray);

            if (ruleRemoved is false)
            {
                return false;
            }

            OnPoliciesChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, rulesArray);
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
        internal static async Task<bool> InternalRemovePoliciesAsync(this IEnforcer enforcer, string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            var rulesArray = rules as IEnumerable<string>[] ?? rules.ToArray();

            if (enforcer.PolicyManager.HasPolicies(section, policyType, rulesArray) is false)
            {
                return false;
            }

            bool ruleRemoved = await enforcer.PolicyManager.RemovePoliciesAsync(section, policyType, rulesArray);

            if (ruleRemoved is false)
            {
                return false;
            }

            await OnPoliciesAsyncChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, rulesArray);
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
        internal static bool InternalRemoveFilteredPolicy(this IEnforcer enforcer, string section, string policyType, int fieldIndex, params string[] fieldValues)
        {
            var effectPolices = enforcer.PolicyManager.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);

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
        internal static async Task<bool> InternalRemoveFilteredPolicyAsync(this IEnforcer enforcer, string section, string policyType, int fieldIndex, params string[] fieldValues)
        {
            var effectPolicies = await enforcer.PolicyManager.RemoveFilteredPolicyAsync(section, policyType, fieldIndex, fieldValues);

            if (effectPolicies is null)
            {
                return false;
            }

            await OnPoliciesAsyncChanged(enforcer, PolicyOperation.PolicyRemove, section, policyType, effectPolicies);
            return true;
        }

        private static void OnPolicyChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<string> rule)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLink(policyOperation, section, policyType, rule);
            }

            NotifyPolicyChanged(enforcer);
        }

        private static void OnPolicyChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<string> oldRule, IEnumerable<string> newRule)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLink(policyOperation, section, policyType, oldRule, newRule);
            }

            NotifyPolicyChanged(enforcer);
        }

        private static async Task OnPolicyAsyncChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<string> rule)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLink(policyOperation,
                    section, policyType, rule);
            }
            await NotifyPolicyChangedAsync(enforcer);
        }

        private static async Task OnPolicyAsyncChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<string> oldRule, IEnumerable<string> newRule)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLink(policyOperation,
                    section, policyType, oldRule, newRule);
            }
            await NotifyPolicyChangedAsync(enforcer);
        }

        private static void OnPoliciesChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLinks(policyOperation,
                    section, policyType, rules);
            }
            NotifyPolicyChanged(enforcer);
        }

        private static void OnPoliciesChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<IEnumerable<string>> oldRules, IEnumerable<IEnumerable<string>> newRules)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLinks(policyOperation,
                    section, policyType, oldRules, newRules);
            }
            NotifyPolicyChanged(enforcer);
        }

        private static async Task OnPoliciesAsyncChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLinks(policyOperation,
                    section, policyType, rules);
            }
            await NotifyPolicyChangedAsync(enforcer);
        }

        private static async Task OnPoliciesAsyncChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<IEnumerable<string>> oldRules,  IEnumerable<IEnumerable<string>> newRules)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLinks(policyOperation,
                    section, policyType, oldRules, newRules);
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
