using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetCasbin.Model;
#if !NET452
using Microsoft.Extensions.Logging;
#endif

namespace NetCasbin
{
    /// <summary>
    /// InternalEnforcer = CoreEnforcer + Internal API.
    /// </summary>
    public class InternalEnforcer : CoreEnforcer
    {
        /// <summary>
        /// Adds a rule to the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected bool InternalAddPolicy(string sec, string ptype, List<string> rule)
        {
            if (model.HasPolicy(sec, ptype, rule))
            {
                return false;
            }

            if (adapter is not null && autoSave)
            {
                try
                {
                    adapter.AddPolicy(sec, ptype, rule);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleAdded = model.AddPolicy(sec, ptype, rule);

            if (ruleAdded is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                model.BuildIncrementalRoleLink(PolicyOperation.PolicyAdd,
                    sec, ptype, rule);
                ExpressionHandler.SetGFunctions();
            }

            NotifyPolicyChanged();
            return true;
        }

        /// <summary>
        /// Adds a rule to the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected async Task<bool> InternalAddPolicyAsync(string sec, string ptype, List<string> rule)
        {
            if (model.HasPolicy(sec, ptype, rule))
            {
                return false;
            }

            if (adapter is not null && autoSave)
            {
                try
                {
                    await adapter.AddPolicyAsync(sec, ptype, rule);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleAdded = model.AddPolicy(sec, ptype, rule);

            if (ruleAdded is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                model.BuildIncrementalRoleLink(PolicyOperation.PolicyAdd,
                    sec, ptype, rule);
                ExpressionHandler.SetGFunctions();
            }

            await NotifyPolicyChangedAsync();
            return true;
        }

        /// <summary>
        /// Adds rules to the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        protected bool InternalAddPolicies(string sec, string ptype, IEnumerable<List<string>> rules)
        {
            var ruleArray = rules as List<string>[] ?? rules.ToArray();

            if (model.HasPolicies(sec, ptype, ruleArray))
            {
                return false;
            }

            if (adapter is not null && autoSave)
            {
                try
                {
                    adapter.AddPolicies(sec, ptype, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleAdded = model.AddPolicies(sec, ptype, ruleArray);

            if (ruleAdded is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                model.BuildIncrementalRoleLinks(PolicyOperation.PolicyAdd,
                    sec, ptype, ruleArray);
                ExpressionHandler.SetGFunctions();
            }

            NotifyPolicyChanged();
            return true;
        }


        /// <summary>
        /// Adds rules to the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        protected async Task<bool> InternalAddPoliciesAsync(string sec, string ptype, IEnumerable<List<string>> rules)
        {
            var ruleArray = rules as List<string>[] ?? rules.ToArray();

            if (model.HasPolicies(sec, ptype, ruleArray))
            {
                return false;
            }

            if (adapter is not null && autoSave)
            {
                try
                {
                    await adapter.AddPoliciesAsync(sec, ptype, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleAdded = model.AddPolicies(sec, ptype, ruleArray);

            if (ruleAdded is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                model.BuildIncrementalRoleLinks(PolicyOperation.PolicyAdd,
                    sec, ptype, ruleArray);
                ExpressionHandler.SetGFunctions();
            }

            await NotifyPolicyChangedAsync();
            return true;
        }

        /// <summary>
        /// Removes a rule from the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected bool InternalRemovePolicy(string sec, string ptype, List<string> rule)
        {
            if (model.HasPolicy(sec, ptype, rule) is false)
            {
                return false;
            }

            if (adapter is not null && autoSave)
            {
                try
                {
                    adapter.RemovePolicy(sec, ptype, rule);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = model.RemovePolicy(sec, ptype, rule);

            if (ruleRemoved is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                model.BuildIncrementalRoleLink(PolicyOperation.PolicyRemove,
                    sec, ptype, rule);
                ExpressionHandler.SetGFunctions();
            }

            NotifyPolicyChanged();
            return true;
        }

        /// <summary>
        /// Removes a rule from the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected async Task<bool> InternalRemovePolicyAsync(string sec, string ptype, List<string> rule)
        {
            if (model.HasPolicy(sec, ptype, rule) is false)
            {
                return false;
            }

            if (adapter is not null && autoSave)
            {
                try
                {
                    await adapter.RemovePolicyAsync(sec, ptype, rule);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = model.RemovePolicy(sec, ptype, rule);

            if (ruleRemoved is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                model.BuildIncrementalRoleLink(PolicyOperation.PolicyRemove,
                    sec, ptype, rule);
                ExpressionHandler.SetGFunctions();
            }

            await NotifyPolicyChangedAsync();
            return true;
        }

        /// <summary>
        /// Removes rules from the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        protected bool InternalRemovePolicies(string sec, string ptype, IEnumerable<List<string>> rules)
        {
            var ruleArray = rules as List<string>[] ?? rules.ToArray();

            if (model.HasPolicies(sec, ptype, ruleArray) is false)
            {
                return false;
            }

            if (adapter is not null && autoSave)
            {
                try
                {
                    adapter.RemovePolicies(sec, ptype, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = model.RemovePolicies(sec, ptype, ruleArray);

            if (ruleRemoved is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                model.BuildIncrementalRoleLinks(PolicyOperation.PolicyRemove,
                    sec, ptype, ruleArray);
                ExpressionHandler.SetGFunctions();
            }

            NotifyPolicyChanged();
            return true;
        }

        /// <summary>
        /// Removes rules from the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        protected async Task<bool> InternalRemovePoliciesAsync(string sec, string ptype,
            IEnumerable<List<string>> rules)
        {
            var ruleArray = rules as List<string>[] ?? rules.ToArray();

            if (model.HasPolicies(sec, ptype, ruleArray) is false)
            {
                return false;
            }

            if (adapter is not null && autoSave)
            {
                try
                {
                    await adapter.RemovePoliciesAsync(sec, ptype, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = model.RemovePolicies(sec, ptype, ruleArray);

            if (ruleRemoved is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                model.BuildIncrementalRoleLinks(PolicyOperation.PolicyRemove,
                    sec, ptype, ruleArray);
                ExpressionHandler.SetGFunctions();
            }

            await NotifyPolicyChangedAsync();
            return true;
        }

        /// <summary>
        /// Removes rules based on field filters from the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        protected bool InternalRemoveFilteredPolicy(string sec, string ptype, int fieldIndex,
            params string[] fieldValues)
        {
            if (adapter is not null && autoSave)
            {
                try
                {
                    adapter.RemoveFilteredPolicy(sec, ptype, fieldIndex, fieldValues);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = model.RemoveFilteredPolicy(sec, ptype, fieldIndex, fieldValues);

            if (ruleRemoved is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                BuildRoleLinks();
                ExpressionHandler.SetGFunctions();
            }

            NotifyPolicyChanged();
            return true;
        }

        /// <summary>
        /// Removes rules based on field filters from the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        protected async Task<bool> InternalRemoveFilteredPolicyAsync(string sec, string ptype, int fieldIndex,
            params string[] fieldValues)
        {
            if (adapter is not null && autoSave)
            {
                try
                {
                    await adapter.RemoveFilteredPolicyAsync(sec, ptype, fieldIndex, fieldValues);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }
            }

            bool ruleRemoved = model.RemoveFilteredPolicy(sec, ptype, fieldIndex, fieldValues);

            if (ruleRemoved is false)
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RoleSection))
            {
                BuildRoleLinks();
                ExpressionHandler.SetGFunctions();
            }

            await NotifyPolicyChangedAsync();
            return true;
        }

        private void NotifyPolicyChanged()
        {
            if (autoCleanEnforceCache)
            {
                EnforceCache?.Clear();
#if !NET452
                Logger?.LogInformation("Enforcer Cache, Cleared all enforce cache.");
#endif
            }

            if (autoNotifyWatcher)
            {
                watcher?.Update();
            }
        }

        private async Task NotifyPolicyChangedAsync()
        {
            if (autoCleanEnforceCache && EnforceCache is not null)
            {
                await EnforceCache.ClearAsync();
#if !NET452
                Logger?.LogInformation("Enforcer Cache, Cleared all enforce cache.");
#endif
            }

            if (autoNotifyWatcher && watcher is not null)
            {
                await watcher.UpdateAsync();
            }
        }
    }
}
