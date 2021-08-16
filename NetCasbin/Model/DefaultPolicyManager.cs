using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Persist;

namespace Casbin.Model
{
    public class DefaultPolicyManager : IPolicyManager
    {
        private ISingleAdapter _singleAdapter;
        private IBatchAdapter _batchAdapter;
        private IEpochAdapter _epochAdapter;

        public DefaultPolicyManager(IPolicy policy, IReadOnlyAdapter adapter = null)
        {
            Policy = policy;
            if (adapter is not null)
            {
                Adapter = adapter;
            }
        }

        public virtual bool IsSynchronized => false;
        public bool HasAdapter => Adapter is null;
        public bool AutoSave { get; set; }
        public IReadOnlyAdapter Adapter
        {
            get => _epochAdapter;
            set
            {
                _singleAdapter = value as ISingleAdapter;
                _batchAdapter = value as IBatchAdapter;
                _epochAdapter = value as IEpochAdapter;
            }
        }

        public IPolicy Policy { get; set; }

        public static IPolicyManager Create()
        {
            return new DefaultPolicyManager(DefaultPolicy.Create());
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
            if (TryStartRead() is false)
            {
                return null;
            }

            try
            {
                return Policy.GetPolicy(section, policyType);
            }
            finally
            {
                EndRead();
            }
        }

        public IEnumerable<IEnumerable<string>> GetFilteredPolicy(string section, string policyType, int fieldIndex, params string[] fieldValues)
        {
            if (TryStartRead() is false)
            {
                return null;
            }

            try
            {
                return Policy.GetFilteredPolicy(section, policyType, fieldIndex, fieldValues);
            }
            finally
            {
                EndRead();
            }
        }

        public IEnumerable<string> GetValuesForFieldInPolicy(string section, string policyType, int fieldIndex)
        {
            if (TryStartRead() is false)
            {
                return null;
            }

            try
            {
                return Policy.GetValuesForFieldInPolicy(section, policyType, fieldIndex);
            }
            finally
            {
                EndRead();
            }
        }

        public IEnumerable<string> GetValuesForFieldInPolicyAllTypes(string section, int fieldIndex)
        {
            if (TryStartRead() is false)
            {
                return null;
            }

            try
            {
                return Policy.GetValuesForFieldInPolicyAllTypes(section, fieldIndex);
            }
            finally
            {
                EndRead();
            }
        }

        public bool HasPolicy(string section, string policyType, IEnumerable<string> rule)
        {
            StartRead();
            try
            {
                return Policy.HasPolicy(section, policyType, rule);
            }
            finally
            {
                EndRead();
            }
        }

        public bool HasPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            StartRead();
            try
            {
                return Policy.HasPolicies(section, policyType, rules);
            }
            finally
            {
                EndRead();
            }
        }

        public bool AddPolicy(string section, string policyType, IEnumerable<string> rule)
        {
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                IEnumerable<string> ruleArray = rule as string[] ?? rule.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.AddPolicy(section, policyType, ruleArray);
                }

                _singleAdapter?.AddPolicy(section, policyType, ruleArray);

                return Policy.AddPolicy(section, policyType, ruleArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public bool AddPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                var rulesArray = rules as IEnumerable<string>[] ?? rules.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.AddPolicies(section, policyType, rulesArray);
                }

                _batchAdapter?.AddPolicies(section, policyType, rulesArray);

                return Policy.AddPolicies(section, policyType, rulesArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public bool RemovePolicy(string section, string policyType, IEnumerable<string> rule)
        {
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                string[] ruleArray = rule as string[] ?? rule.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.RemovePolicy(section, policyType, ruleArray);
                }

                _singleAdapter?.RemovePolicy(section, policyType, ruleArray);

                return Policy.RemovePolicy(section, policyType, ruleArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public bool RemovePolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                var rulesArray = rules as IEnumerable<string>[] ?? rules.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.RemovePolicies(section, policyType, rulesArray);
                }

                _batchAdapter?.RemovePolicies(section, policyType, rulesArray);

                return Policy.RemovePolicies(section, policyType, rulesArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public IEnumerable<IEnumerable<string>> RemoveFilteredPolicy(string section, string policyType, int fieldIndex, params string[] fieldValues)
        {
            if (TryStartWrite() is false)
            {
                return null;
            }

            try
            {
                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);
                }

                _batchAdapter?.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);

                return Policy.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);
            }
            finally
            {
                EndWrite();
            }
        }

        public async Task<bool> AddPolicyAsync(string section, string policyType, IEnumerable<string> rule)
        {
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                IEnumerable<string> ruleArray = rule as string[] ?? rule.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.AddPolicy(section, policyType, ruleArray);
                }

                if (_singleAdapter is not null)
                {
                    await _singleAdapter.AddPolicyAsync(section, policyType, ruleArray);
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
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                var rulesArray = rules as IEnumerable<string>[] ?? rules.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.AddPolicies(section, policyType, rulesArray);
                }

                if (_batchAdapter is not null)
                {
                    await _batchAdapter.AddPoliciesAsync(section, policyType, rulesArray);
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
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                string[] ruleArray = rule as string[] ?? rule.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.RemovePolicy(section, policyType, ruleArray);
                }

                if (_singleAdapter is not null)
                {
                    await _singleAdapter.RemovePolicyAsync(section, policyType, ruleArray);
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
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                var rulesArray = rules as IEnumerable<string>[] ?? rules.ToArray();

                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.RemovePolicies(section, policyType, rulesArray);
                }

                if (_batchAdapter is not null)
                {
                    await _batchAdapter.RemovePoliciesAsync(section, policyType, rulesArray);
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
            if (TryStartWrite() is false)
            {
                return null;
            }

            try
            {
                if (HasAdapter is false || AutoSave is false)
                {
                    return Policy.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);
                }

                if (_batchAdapter is not null)
                {
                    await _batchAdapter.RemoveFilteredPolicyAsync(section, policyType, fieldIndex, fieldValues);
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
            StartRead();
            try
            {
                Policy.ClearPolicy();
            }
            finally
            {
                EndRead();
            }
        }

        public bool LoadPolicy() => throw new NotImplementedException();
        public bool SavePolicy() => throw new NotImplementedException();
        public bool SavePolicyAsync() => throw new NotImplementedException();
    }
}
