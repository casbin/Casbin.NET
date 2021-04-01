using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Persist;

namespace Casbin.Model
{
    public class DefaultPolicyManager : IPolicyManager
    {
        private DefaultPolicyManager(IPolicy policy, IAdapter adapter = null)
        {
            Policy = policy;
            if (adapter is not null)
            {
                Adapter = adapter;
            }
        }

        public bool AutoSave { get; set; }
        public IAdapter Adapter { get; private set; }
        public bool HasAdapter => Adapter is null;
        public IPolicy Policy { get; }

        public static IPolicyManager Create()
        {
            return new DefaultPolicyManager(DefaultPolicy.Create());
        }

        public void SetAdapter(IAdapter adapter)
        {
            Adapter = adapter;
        }

        public virtual void StartRead()
        {
        }

        public virtual void StartWrite()
        {
        }

        public virtual void EndRead()
        {
        }

        public virtual void EndWrite()
        {
        }

        public virtual bool TryStartRead()
        {
            return true;
        }

        public virtual bool TryStartWrite()
        {
            return true;
        }


        public IEnumerable<IEnumerable<string>> GetPolicy(string section, string policyType)
        {
            try
            {
                return TryStartRead() ? Policy.GetPolicy(section, policyType) : null;
            }
            finally
            {
                EndRead();
            }
        }

        public IEnumerable<IEnumerable<string>> GetFilteredPolicy(string section, string policyType, int fieldIndex, params string[] fieldValues)
        {
            try
            {
                return TryStartRead() ? Policy.GetFilteredPolicy(section, policyType, fieldIndex, fieldValues) : null;
            }
            finally
            {
                EndRead();
            }
        }

        public IEnumerable<string> GetValuesForFieldInPolicy(string section, string policyType, int fieldIndex)
        {
            try
            {
                return TryStartRead() ? Policy.GetValuesForFieldInPolicy(section, policyType, fieldIndex) : null;
            }
            finally
            {
                EndRead();
            }
        }

        public IEnumerable<string> GetValuesForFieldInPolicyAllTypes(string section, int fieldIndex)
        {
            try
            {
                return TryStartRead() ? Policy.GetValuesForFieldInPolicyAllTypes(section, fieldIndex) : null;
            }
            finally
            {
                EndRead();
            }
        }

        public bool HasPolicy(string section, string policyType, IEnumerable<string> rule)
        {
            try
            {
                StartRead();
                return Policy.HasPolicy(section, policyType, rule);
            }
            finally
            {
                EndRead();
            }
        }

        public bool HasPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            try
            {
                StartRead();
                return Policy.HasPolicies(section, policyType, rules);
            }
            finally
            {
                EndRead();
            }
        }

        public bool AddPolicy(string section, string policyType, IEnumerable<string> rule)
        {
            try
            {
                if (TryStartWrite() is false)
                {
                    return false;
                }

                IEnumerable<string> ruleArray = rule as string[] ?? rule.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.AddPolicy(section, policyType, ruleArray);
                }

                try
                {
                    Adapter.AddPolicy(section, policyType, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }

                return Policy.AddPolicy(section, policyType, ruleArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public bool AddPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            try
            {
                if (TryStartWrite() is false)
                {
                    return false;
                }

                var rulesArray = rules as IEnumerable<string>[] ?? rules.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.AddPolicies(section, policyType, rulesArray);
                }

                try
                {
                    Adapter.AddPolicies(section, policyType, rulesArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }

                return Policy.AddPolicies(section, policyType, rulesArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public bool RemovePolicy(string section, string policyType, IEnumerable<string> rule)
        {
            try
            {
                if (TryStartWrite() is false)
                {
                    return false;
                }

                string[] ruleArray = rule as string[] ?? rule.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.RemovePolicy(section, policyType, ruleArray);
                }

                try
                {
                    Adapter.RemovePolicy(section, policyType, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }

                return Policy.RemovePolicy(section, policyType, ruleArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public bool RemovePolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            try
            {
                if (TryStartWrite() is false)
                {
                    return false;
                }

                var rulesArray = rules as IEnumerable<string>[] ?? rules.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.RemovePolicies(section, policyType, rulesArray);
                }

                try
                {
                    Adapter.RemovePolicies(section, policyType, rulesArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }

                return Policy.RemovePolicies(section, policyType, rulesArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public IEnumerable<IEnumerable<string>> RemoveFilteredPolicy(string section, string policyType, int fieldIndex, params string[] fieldValues)
        {
            try
            {
                if (TryStartWrite() is false)
                {
                    return null;
                }

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);
                }

                try
                {
                    Adapter.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }

                return Policy.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);
            }
            finally
            {
                EndWrite();
            }
        }

        public async Task<bool> AddPolicyAsync(string section, string policyType, IEnumerable<string> rule)
        {
            try
            {
                if (TryStartWrite() is false)
                {
                    return false;
                }

                IEnumerable<string> ruleArray = rule as string[] ?? rule.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.AddPolicy(section, policyType, ruleArray);
                }

                try
                {
                    await Adapter.AddPolicyAsync(section, policyType, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }

                return Policy.AddPolicy(section, policyType, ruleArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public async Task<bool> AddPoliciesAsync(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            try
            {
                if (TryStartWrite() is false)
                {
                    return false;
                }

                var rulesArray = rules as IEnumerable<string>[] ?? rules.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.AddPolicies(section, policyType, rulesArray);
                }

                try
                {
                    await Adapter.AddPoliciesAsync(section, policyType, rulesArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }

                return Policy.AddPolicies(section, policyType, rulesArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public async Task<bool> RemovePolicyAsync(string section, string policyType, IEnumerable<string> rule)
        {
            try
            {
                if (TryStartWrite() is false)
                {
                    return false;
                }

                string[] ruleArray = rule as string[] ?? rule.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.RemovePolicy(section, policyType, ruleArray);
                }

                try
                {
                    await Adapter.RemovePolicyAsync(section, policyType, ruleArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }

                return Policy.RemovePolicy(section, policyType, ruleArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public async Task<bool> RemovePoliciesAsync(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            try
            {
                if (TryStartWrite() is false)
                {
                    return false;
                }

                var rulesArray = rules as IEnumerable<string>[] ?? rules.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.RemovePolicies(section, policyType, rulesArray);
                }

                try
                {
                    await Adapter.RemovePoliciesAsync(section, policyType, rulesArray);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }

                return Policy.RemovePolicies(section, policyType, rulesArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public async Task<IEnumerable<IEnumerable<string>>> RemoveFilteredPolicyAsync(string section, string policyType, int fieldIndex, params string[] fieldValues)
        {
            try
            {
                if (TryStartWrite() is false)
                {
                    return null;
                }

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);
                }

                try
                {
                    await Adapter.RemoveFilteredPolicyAsync(section, policyType, fieldIndex, fieldValues);
                }
                catch (NotImplementedException)
                {
                    // error intentionally ignored
                }

                return Policy.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);
            }
            finally
            {
                EndWrite();
            }
        }

        public void ClearPolicy()
        {
            try
            {
                if (TryStartRead())
                {
                    Policy.ClearPolicy();
                }
            }
            finally
            {
                EndRead();
            }
        }
    }
}
