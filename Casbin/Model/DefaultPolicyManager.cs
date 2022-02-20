using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Persist;

namespace Casbin.Model
{
    public class DefaultPolicyManager : IPolicyManager
    {
        protected ISingleAdapter SingleAdapter;
        protected IBatchAdapter BatchAdapter;
        protected IEpochAdapter EpochAdapter;
        protected IFilteredAdapter FilteredAdapter;

        // ReSharper disable once MemberCanBeProtected.Global
        public DefaultPolicyManager(IPolicyStore policyStore, IReadOnlyAdapter adapter = null)
        {
            PolicyStore = policyStore;
            if (adapter is not null)
            {
                Adapter = adapter;
            }
        }

        public virtual bool IsSynchronized => false;
        public bool HasAdapter => Adapter is null;
        public Dictionary<string, Dictionary<string, Assertion>> Sections => PolicyStore.Sections;
        public bool AutoSave { get; set; }
        public IPolicyStore PolicyStore { get; set; }

        public IReadOnlyAdapter Adapter
        {
            get => EpochAdapter;
            set
            {
                SingleAdapter = value as ISingleAdapter;
                BatchAdapter = value as IBatchAdapter;
                EpochAdapter = value as IEpochAdapter;
                FilteredAdapter = value as IFilteredAdapter;
            }
        }

        public static IPolicyManager Create()
        {
            return new DefaultPolicyManager(DefaultPolicyStore.Create());
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

        public bool LoadPolicy()
        {
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                if (EpochAdapter is not null)
                {
                    EpochAdapter.LoadPolicy(PolicyStore);
                    return true;
                }
                return false;
            }
            finally
            {
                EndWrite();
            }
        }

        public bool LoadFilteredPolicy(Filter filter)
        {
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                if (FilteredAdapter is not null)
                {
                    FilteredAdapter.LoadFilteredPolicy(PolicyStore, filter);
                    return true;
                }
                return false;
            }
            finally
            {
                EndWrite();
            }
        }

        public bool SavePolicy()
        {
            if (TryStartRead() is false)
            {
                return false;
            }

            try
            {
                if (EpochAdapter is not null)
                {
                    EpochAdapter.SavePolicy(PolicyStore);
                    return true;
                }
                return false;
            }
            finally
            {
                EndRead();
            }
        }

        public virtual async Task<bool> LoadPolicyAsync()
        {
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                if (EpochAdapter is not null)
                {
                    await EpochAdapter.LoadPolicyAsync(PolicyStore);
                    return true;
                }
                return false;
            }
            finally
            {
                EndWrite();
            };
        }
        public virtual async Task<bool> LoadFilteredPolicyAsync(Filter filter)
        {
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                if (FilteredAdapter is not null)
                {
                    await FilteredAdapter.LoadFilteredPolicyAsync(PolicyStore, filter);
                    return true;
                }
                return false;
            }
            finally
            {
                EndWrite();
            }
        }

        public virtual async Task<bool> SavePolicyAsync()
        {
            if (TryStartRead() is false)
            {
                return false;
            }

            try
            {
                if (EpochAdapter is not null)
                {
                    await EpochAdapter.SavePolicyAsync(PolicyStore);
                    return true;
                }
                return false;
            }
            finally
            {
                EndRead();
            }
        }

        public Assertion GetRequiredAssertion(string section, string type)
        {
            if (TryStartRead() is false)
            {
                return null;
            }

            try
            {
                return PolicyStore.GetRequiredAssertion(section, type);
            }
            finally
            {
                EndRead();
            }
        }

        public bool TryGetAssertion(string section, string policyType, out Assertion returnAssertion)
        {
            if (TryStartRead() is false)
            {
                returnAssertion = null;
                return false;
            }

            try
            {
                return PolicyStore.TryGetAssertion(section, policyType, out returnAssertion);
            }
            finally
            {
                EndRead();
            }
        }

        public IEnumerable<IEnumerable<string>> GetPolicy(string section, string policyType)
        {
            if (TryStartRead() is false)
            {
                return null;
            }

            try
            {
                return PolicyStore.GetPolicy(section, policyType);
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
                return PolicyStore.GetFilteredPolicy(section, policyType, fieldIndex, fieldValues);
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
                return PolicyStore.GetValuesForFieldInPolicy(section, policyType, fieldIndex);
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
                return PolicyStore.GetValuesForFieldInPolicyAllTypes(section, fieldIndex);
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
                return PolicyStore.HasPolicy(section, policyType, rule);
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
                return PolicyStore.HasPolicies(section, policyType, rules);
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
                    return PolicyStore.AddPolicy(section, policyType, ruleArray);
                }
                SingleAdapter?.AddPolicy(section, policyType, ruleArray);
                return PolicyStore.AddPolicy(section, policyType, ruleArray);
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
                    return PolicyStore.AddPolicies(section, policyType, rulesArray);
                }
                BatchAdapter?.AddPolicies(section, policyType, rulesArray);
                return PolicyStore.AddPolicies(section, policyType, rulesArray);
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
                    return PolicyStore.RemovePolicy(section, policyType, ruleArray);
                }
                SingleAdapter?.RemovePolicy(section, policyType, ruleArray);
                return PolicyStore.RemovePolicy(section, policyType, ruleArray);
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
                    return PolicyStore.RemovePolicies(section, policyType, rulesArray);
                }
                BatchAdapter?.RemovePolicies(section, policyType, rulesArray);
                return PolicyStore.RemovePolicies(section, policyType, rulesArray);
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
                    return PolicyStore.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);
                }
                BatchAdapter?.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);
                return PolicyStore.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);
            }
            finally
            {
                EndWrite();
            }
        }

        public virtual async Task<bool> AddPolicyAsync(string section, string policyType, IEnumerable<string> rule)
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
                    return PolicyStore.AddPolicy(section, policyType, ruleArray);
                }

                if (SingleAdapter is not null)
                {
                    await SingleAdapter.AddPolicyAsync(section, policyType, ruleArray);
                }

                return PolicyStore.AddPolicy(section, policyType, ruleArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public virtual async Task<bool> AddPoliciesAsync(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
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
                    return PolicyStore.AddPolicies(section, policyType, rulesArray);
                }

                if (BatchAdapter is not null)
                {
                    await BatchAdapter.AddPoliciesAsync(section, policyType, rulesArray);
                }

                return PolicyStore.AddPolicies(section, policyType, rulesArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public virtual async Task<bool> RemovePolicyAsync(string section, string policyType, IEnumerable<string> rule)
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
                    return PolicyStore.RemovePolicy(section, policyType, ruleArray);
                }

                if (SingleAdapter is not null)
                {
                    await SingleAdapter.RemovePolicyAsync(section, policyType, ruleArray);
                }

                return PolicyStore.RemovePolicy(section, policyType, ruleArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public virtual async Task<bool> RemovePoliciesAsync(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
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
                    return PolicyStore.RemovePolicies(section, policyType, rulesArray);
                }

                if (BatchAdapter is not null)
                {
                    await BatchAdapter.RemovePoliciesAsync(section, policyType, rulesArray);
                }

                return PolicyStore.RemovePolicies(section, policyType, rulesArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public virtual async Task<IEnumerable<IEnumerable<string>>> RemoveFilteredPolicyAsync(string section, string policyType, int fieldIndex, params string[] fieldValues)
        {
            if (TryStartWrite() is false)
            {
                return null;
            }

            try
            {
                if (HasAdapter is false || AutoSave is false)
                {
                    return PolicyStore.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);
                }

                if (BatchAdapter is not null)
                {
                    await BatchAdapter.RemoveFilteredPolicyAsync(section, policyType, fieldIndex, fieldValues);
                }

                return PolicyStore.RemoveFilteredPolicy(section, policyType, fieldIndex, fieldValues);
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
                PolicyStore.ClearPolicy();
            }
            finally
            {
                EndRead();
            }
        }
    }
}
