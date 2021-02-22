using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Casbin.Extensions
{
    internal static class InternalEnforcerExtension
    {
        /// <summary>
        /// Adds a rule to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        internal static bool InternalAddPolicy(this IEnforcer enforcer, string sec, string ptype, List<string> rule)
        {
            if (enforcer.Model.HasPolicy(sec, ptype, rule))
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    enforcer.Adapter.AddPolicy(sec, ptype, rule);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleAdded = enforcer.Model.AddPolicy(sec, ptype, rule);

            if (ruleAdded is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLink(enforcer.RoleManager, PolicyOperation.PolicyAdd,
                    sec, ptype, rule);
                enforcer.ExpressionHandler.SetGFunctions();
            }

            NotifyPolicyChanged(enforcer);
            return true;
        }

        /// <summary>
        /// Adds a rule to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalAddPolicyAsync(this IEnforcer enforcer, string sec, string ptype, List<string> rule)
        {
            if (enforcer.Model.HasPolicy(sec, ptype, rule))
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    await enforcer.Adapter.AddPolicyAsync(sec, ptype, rule);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleAdded = enforcer.Model.AddPolicy(sec, ptype, rule);

            if (ruleAdded is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLink(enforcer.RoleManager, PolicyOperation.PolicyAdd,
                    sec, ptype, rule);
                enforcer.ExpressionHandler.SetGFunctions();
            }

            await NotifyPolicyChangedAsync(enforcer);
            return true;
        }

        /// <summary>
        /// Adds rules to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        internal static bool InternalAddPolicies(this IEnforcer enforcer, string sec, string ptype, IEnumerable<List<string>> rules)
        {
            var ruleArray = rules as List<string>[] ?? rules.ToArray();

            if (enforcer.Model.HasPolicies(sec, ptype, ruleArray))
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    enforcer.Adapter.AddPolicies(sec, ptype, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleAdded = enforcer.Model.AddPolicies(sec, ptype, ruleArray);

            if (ruleAdded is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLinks(enforcer.RoleManager, PolicyOperation.PolicyAdd,
                    sec, ptype, ruleArray);
                enforcer.ExpressionHandler.SetGFunctions();
            }

            NotifyPolicyChanged(enforcer);
            return true;
        }


        /// <summary>
        /// Adds rules to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalAddPoliciesAsync(this IEnforcer enforcer, string sec, string ptype, IEnumerable<List<string>> rules)
        {
            var ruleArray = rules as List<string>[] ?? rules.ToArray();

            if (enforcer.Model.HasPolicies(sec, ptype, ruleArray))
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    await enforcer.Adapter.AddPoliciesAsync(sec, ptype, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleAdded = enforcer.Model.AddPolicies(sec, ptype, ruleArray);

            if (ruleAdded is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLinks(enforcer.RoleManager, PolicyOperation.PolicyAdd,
                    sec, ptype, ruleArray);
                enforcer.ExpressionHandler.SetGFunctions();
            }

            await NotifyPolicyChangedAsync(enforcer);
            return true;
        }

        /// <summary>
        /// Removes a rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        internal static bool InternalRemovePolicy(this IEnforcer enforcer, string sec, string ptype, List<string> rule)
        {
            if (enforcer.Model.HasPolicy(sec, ptype, rule) is false)
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    enforcer.Adapter.RemovePolicy(sec, ptype, rule);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = enforcer.Model.RemovePolicy(sec, ptype, rule);

            if (ruleRemoved is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLink(enforcer.RoleManager, PolicyOperation.PolicyRemove,
                    sec, ptype, rule);
                enforcer.ExpressionHandler.SetGFunctions();
            }

            NotifyPolicyChanged(enforcer);
            return true;
        }

        /// <summary>
        /// Removes a rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalRemovePolicyAsync(this IEnforcer enforcer, string sec, string ptype, List<string> rule)
        {
            if (enforcer.Model.HasPolicy(sec, ptype, rule) is false)
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    await enforcer.Adapter.RemovePolicyAsync(sec, ptype, rule);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = enforcer.Model.RemovePolicy(sec, ptype, rule);

            if (ruleRemoved is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLink(enforcer.RoleManager, PolicyOperation.PolicyRemove,
                    sec, ptype, rule);
                enforcer.ExpressionHandler.SetGFunctions();
            }

            await NotifyPolicyChangedAsync(enforcer);
            return true;
        }

        /// <summary>
        /// Removes rules from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        internal static bool InternalRemovePolicies(this IEnforcer enforcer, string sec, string ptype, IEnumerable<List<string>> rules)
        {
            var ruleArray = rules as List<string>[] ?? rules.ToArray();

            if (enforcer.Model.HasPolicies(sec, ptype, ruleArray) is false)
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    enforcer.Adapter.RemovePolicies(sec, ptype, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = enforcer.Model.RemovePolicies(sec, ptype, ruleArray);

            if (ruleRemoved is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLinks(enforcer.RoleManager, PolicyOperation.PolicyRemove,
                    sec, ptype, ruleArray);
                enforcer.ExpressionHandler.SetGFunctions();
            }

            NotifyPolicyChanged(enforcer);
            return true;
        }

        /// <summary>
        /// Removes rules from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalRemovePoliciesAsync(this IEnforcer enforcer, string sec, string ptype, IEnumerable<List<string>> rules)
        {
            var ruleArray = rules as List<string>[] ?? rules.ToArray();

            if (enforcer.Model.HasPolicies(sec, ptype, ruleArray) is false)
            {
                return false;
            }

            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    await enforcer.Adapter.RemovePoliciesAsync(sec, ptype, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = enforcer.Model.RemovePolicies(sec, ptype, ruleArray);

            if (ruleRemoved is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.Model.BuildIncrementalRoleLinks(enforcer.RoleManager, PolicyOperation.PolicyRemove,
                    sec, ptype, ruleArray);
                enforcer.ExpressionHandler.SetGFunctions();
            }

            await NotifyPolicyChangedAsync(enforcer);
            return true;
        }

        /// <summary>
        /// Removes rules based on field filters from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        internal static bool InternalRemoveFilteredPolicy(this IEnforcer enforcer, string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    enforcer.Adapter.RemoveFilteredPolicy(sec, ptype, fieldIndex, fieldValues);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = enforcer.Model.RemoveFilteredPolicy(sec, ptype, fieldIndex, fieldValues);

            if (ruleRemoved is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.BuildRoleLinks();
                enforcer.ExpressionHandler.SetGFunctions();
            }

            NotifyPolicyChanged(enforcer);
            return true;
        }

        /// <summary>
        /// Removes rules based on field filters from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalRemoveFilteredPolicyAsync(this IEnforcer enforcer, string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            if (enforcer.Adapter is not null && enforcer.AutoSave)
            {
                try
                {
                    await enforcer.Adapter.RemoveFilteredPolicyAsync(sec, ptype, fieldIndex, fieldValues);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = enforcer.Model.RemoveFilteredPolicy(sec, ptype, fieldIndex, fieldValues);

            if (ruleRemoved is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                enforcer.BuildRoleLinks();
                enforcer.ExpressionHandler.SetGFunctions();
            }

            await NotifyPolicyChangedAsync(enforcer);
            return true;
        }

        private static void NotifyPolicyChanged(IEnforcer enforcer)
        {
            if (enforcer.AutoNotifyWatcher)
            {
                enforcer.Watcher?.Update();
            }
        }

        private static async Task NotifyPolicyChangedAsync(IEnforcer enforcer)
        {
            if (enforcer.AutoNotifyWatcher && enforcer.Watcher is not null)
            {
                await enforcer.Watcher.UpdateAsync();
            }
        }
    }
}
