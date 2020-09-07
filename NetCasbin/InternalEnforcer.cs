using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            if (adapter != null && autoSave)
            {
                bool adapterAdded;
                try
                {
                    adapter.AddPolicy(sec, ptype, rule);
                    adapterAdded = true;
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                    adapterAdded = false;
                }

                if (adapterAdded)
                {
                    watcher?.Update();
                }
            }

            bool ruleAdded = model.AddPolicy(sec, ptype, rule);
            return ruleAdded;
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

            if (adapter != null && autoSave)
            {
                bool adapterAdded;
                try
                {
                    await adapter.AddPolicyAsync(sec, ptype, rule);
                    adapterAdded = true;
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                    adapterAdded = false;
                }

                if (adapterAdded)
                {
                    if (watcher is not null)
                    {
                        await watcher.UpdateAsync();
                    }
                }
            }

            bool ruleAdded = model.AddPolicy(sec, ptype, rule);
            return ruleAdded;
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

            if (adapter != null && autoSave)
            {
                bool adapterAdded;
                try
                {
                    adapter.AddPolicies(sec, ptype, ruleArray);
                    adapterAdded = true;
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                    adapterAdded = false;
                }

                if (adapterAdded)
                {
                    watcher?.Update();
                }
            }

            bool ruleAdded = model.AddPolicies(sec, ptype, ruleArray);
            return ruleAdded;
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

            if (adapter != null && autoSave)
            {
                bool adapterAdded;
                try
                {
                    await adapter.AddPoliciesAsync(sec, ptype, ruleArray);
                    adapterAdded = true;
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                    adapterAdded = false;
                }

                if (adapterAdded)
                {
                    if (watcher is not null)
                    {
                        await watcher.UpdateAsync();
                    }
                }
            }

            bool ruleAdded = model.AddPolicies(sec, ptype, ruleArray);
            return ruleAdded;
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
            if (!model.HasPolicy(sec, ptype, rule))
            {
                return true;
            }

            if (adapter != null && autoSave)
            {
                bool adapterRemoved;
                try
                {
                    adapter.RemovePolicy(sec, ptype, rule);
                    adapterRemoved = true;
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                    adapterRemoved = false;
                }

                if (adapterRemoved)
                {
                    watcher?.Update();
                }
            }

            bool ruleRemoved = model.RemovePolicy(sec, ptype, rule);
            return ruleRemoved;
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
            if (adapter != null && autoSave)
            {
                bool adapterRemoved;
                try
                {
                    await adapter.RemovePolicyAsync(sec, ptype, rule);
                    adapterRemoved = true;
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                    adapterRemoved = false;
                }

                if (adapterRemoved)
                {
                    if (watcher is not null)
                    {
                        await watcher.UpdateAsync();
                    }
                }
            }

            bool ruleRemoved = model.RemovePolicy(sec, ptype, rule);
            return ruleRemoved;
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

            if (!model.HasPolicies(sec, ptype, ruleArray))
            {
                return true;
            }

            if (adapter != null && autoSave)
            {
                bool adapterRemoved;
                try
                {
                    adapter.RemovePolicies(sec, ptype, ruleArray);
                    adapterRemoved = true;
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                    adapterRemoved = false;
                }

                if (adapterRemoved)
                {
                    watcher?.Update();
                }
            }

            bool ruleRemoved = model.RemovePolicies(sec, ptype, ruleArray);
            return ruleRemoved;
        }

        /// <summary>
        /// Removes rules from the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        protected async Task<bool> InternalRemovePoliciesAsync(string sec, string ptype, IEnumerable<List<string>> rules)
        {
            var ruleArray = rules as List<string>[] ?? rules.ToArray();

            if (!model.HasPolicies(sec, ptype, ruleArray))
            {
                return true;
            }

            if (adapter != null && autoSave)
            {
                bool adapterRemoved;
                try
                {
                    await adapter.RemovePoliciesAsync(sec, ptype, ruleArray);
                    adapterRemoved = true;
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                    adapterRemoved = false;
                }

                if (adapterRemoved)
                {
                    if (watcher is not null)
                    {
                        await watcher.UpdateAsync();
                    }
                }
            }

            bool ruleRemoved = model.RemovePolicies(sec, ptype, ruleArray);
            return ruleRemoved;
        }

        /// <summary>
        /// Removes rules based on field filters from the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        protected bool InternalRemoveFilteredPolicy(string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            if (adapter != null && autoSave)
            {
                bool adapterRemoved;
                try
                {
                    adapter.RemoveFilteredPolicy(sec, ptype, fieldIndex, fieldValues);
                    adapterRemoved = true;
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                    adapterRemoved = false;
                }

                if (adapterRemoved)
                {
                    watcher?.Update();
                }
            }

            bool ruleRemoved = model.RemoveFilteredPolicy(sec, ptype, fieldIndex, fieldValues);
            return ruleRemoved;
        }

        /// <summary>
        /// Removes rules based on field filters from the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        protected async Task<bool> InternalRemoveFilteredPolicyAsync(string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            if (adapter != null && autoSave)
            {
                bool adapterRemoved;
                try
                {
                    await adapter.RemoveFilteredPolicyAsync(sec, ptype, fieldIndex, fieldValues);
                    adapterRemoved = true;
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                    adapterRemoved = false;
                }

                if (adapterRemoved)
                {
                    if (watcher is not null)
                    {
                        await watcher.UpdateAsync();
                    }
                }
            }

            bool ruleRemoved = model.RemoveFilteredPolicy(sec, ptype, fieldIndex, fieldValues);
            return ruleRemoved;
        }
    }
}
