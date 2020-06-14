using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCasbin
{
    /// <summary>
    /// InternalEnforcer = CoreEnforcer + Internal API.
    /// </summary>
    public class InternalEnforcer : CoreEnforcer
    {
        /// <summary>
        /// adds a rule to the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected bool AddPolicy(string sec, string ptype, List<string> rule)
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

            var ruleAdded = model.AddPolicy(sec, ptype, rule);
            return ruleAdded;
        }

        /// <summary>
        /// adds a rule to the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected async Task<bool> AddPolicyAsync(string sec, string ptype, List<string> rule)
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
                    if (!(watcher is null))
                    {
                        await watcher?.UpdateAsync();
                    }
                }
            }

            var ruleAdded = model.AddPolicy(sec, ptype, rule);
            return ruleAdded;
        }

        /// <summary>
        /// removes a rule from the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected bool RemovePolicy(string sec, string ptype, List<string> rule)
        {
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

            var ruleRemoved = model.RemovePolicy(sec, ptype, rule);
            return ruleRemoved;
        }

        /// <summary>
        /// removes a rule from the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected async Task<bool> RemovePolicyAsync(string sec, string ptype, List<string> rule)
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
                    if (!(watcher is null))
                    {
                        await watcher?.UpdateAsync();
                    }
                }
            }

            var ruleRemoved = model.RemovePolicy(sec, ptype, rule);
            return ruleRemoved;
        }

        /// <summary>
        ///  removes rules based on field filters from the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        protected bool RemoveFilteredPolicy(string sec, string ptype, int fieldIndex, params string[] fieldValues)
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

            var ruleRemoved = model.RemoveFilteredPolicy(sec, ptype, fieldIndex, fieldValues);
            return ruleRemoved;
        }

        /// <summary>
        ///  removes rules based on field filters from the current policy.
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="ptype"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        protected async Task<bool> RemoveFilteredPolicyAsync(string sec, string ptype, int fieldIndex, params string[] fieldValues)
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
                    if (!(watcher is null))
                    {
                        await watcher?.UpdateAsync();
                    }
                }
            }

            var ruleRemoved = model.RemoveFilteredPolicy(sec, ptype, fieldIndex, fieldValues);
            return ruleRemoved;
        }
    }
}
