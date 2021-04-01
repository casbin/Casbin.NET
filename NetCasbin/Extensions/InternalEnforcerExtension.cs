using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if !NET45
using Microsoft.Extensions.Logging;
#endif

namespace Casbin.Extensions
{
    internal static class InternalEnforcerExtension
    {
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
            if (enforcer.Model.HasPolicy(section, policyType, ruleArray))
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    enforcer.Adapter.AddPolicy(section, policyType, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleAdded = enforcer.Model.AddPolicy(section, policyType, ruleArray);

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
            if (enforcer.Model.HasPolicy(section, policyType, ruleArray))
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    await enforcer.Adapter.AddPolicyAsync(section, policyType, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleAdded = enforcer.Model.AddPolicy(section, policyType, ruleArray);

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

            if (enforcer.Model.HasPolicies(section, policyType, ruleArray))
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    enforcer.Adapter.AddPolicies(section, policyType, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleAdded = enforcer.Model.AddPolicies(section, policyType, ruleArray);

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

            if (enforcer.Model.HasPolicies(section, policyType, rulesArray))
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    await enforcer.Adapter.AddPoliciesAsync(section, policyType, rulesArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleAdded = enforcer.Model.AddPolicies(section, policyType, rulesArray);

            if (ruleAdded is false)
            {
                return false;
            }

            await OnPoliciesAsyncChanged(enforcer, PolicyOperation.PolicyAdd, section, policyType, rulesArray);
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
            if (enforcer.Model.HasPolicy(section, policyType, ruleArray) is false)
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    enforcer.Adapter.RemovePolicy(section, policyType, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = enforcer.Model.RemovePolicy(section, policyType, ruleArray);

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
            if (enforcer.Model.HasPolicy(section, policyType, ruleArray) is false)
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    await enforcer.Adapter.RemovePolicyAsync(section, policyType, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = enforcer.Model.RemovePolicy(section, policyType, ruleArray);

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

            if (enforcer.Model.HasPolicies(section, policyType, rulesArray) is false)
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    enforcer.Adapter.RemovePolicies(section, policyType, rulesArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = enforcer.Model.RemovePolicies(section, policyType, rulesArray);

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

            if (enforcer.Model.HasPolicies(section, policyType, rulesArray) is false)
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    await enforcer.Adapter.RemovePoliciesAsync(section, policyType, rulesArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = enforcer.Model.RemovePolicies(section, policyType, rulesArray);

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
            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    enforcer.Adapter.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            var effectPolices = enforcer.Model.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);

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
            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    await enforcer.Adapter.RemoveFilteredPolicyAsync(section, policyType, fieldIndex, fieldValues);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            var effectPolicies = enforcer.Model.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);

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
                enforcer.Model.BuildIncrementalRoleLink(enforcer.RoleManager, policyOperation,
                    section, policyType, rule);
                enforcer.ExpressionHandler.SetGFunctions();
            }

            NotifyPolicyChanged(enforcer);
        }

        private static async Task OnPolicyAsyncChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<string> rule)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLink(enforcer.RoleManager, policyOperation,
                    section, policyType, rule);
                enforcer.ExpressionHandler.SetGFunctions();
            }
            await NotifyPolicyChangedAsync(enforcer);
        }

        private static void OnPoliciesChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLinks(enforcer.RoleManager, policyOperation,
                    section, policyType, rules);
                enforcer.ExpressionHandler.SetGFunctions();
            }
            NotifyPolicyChanged(enforcer);
        }

        private static async Task OnPoliciesAsyncChanged(IEnforcer enforcer, PolicyOperation policyOperation,
            string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            if (section.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLinks(enforcer.RoleManager, policyOperation,
                    section, policyType, rules);
                enforcer.ExpressionHandler.SetGFunctions();
            }
            await NotifyPolicyChangedAsync(enforcer);
        }

        private static void NotifyPolicyChanged(IEnforcer enforcer)
        {
            if (enforcer.AutoCleanEnforceCache)
            {
                enforcer.EnforceCache?.Clear();
#if !NET45
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
#if !NET45
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
