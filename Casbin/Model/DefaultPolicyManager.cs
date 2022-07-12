using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Persist;

namespace Casbin.Model
{
    public class DefaultPolicyManager : IPolicyManager
    {
        protected IBatchAdapter BatchAdapter;
        protected IEpochAdapter EpochAdapter;
        protected IFilteredAdapter FilteredAdapter;
        protected ISingleAdapter SingleAdapter;

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
        public bool HasAdapter => Adapter is not null;
        public bool IsFiltered => FilteredAdapter is not null && FilteredAdapter.IsFiltered;
        public Dictionary<string, Dictionary<string, Assertion>> Sections => PolicyStore.Sections;
        public bool AutoSave { get; set; } = true;
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
                if (HasAdapter is false)
                {
                    return false;
                }

                if (EpochAdapter is null)
                {
                    return false;
                }

                PolicyStore.ClearPolicy();
                EpochAdapter.LoadPolicy(PolicyStore);
                return true;
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
                if (FilteredAdapter is null)
                {
                    return false;
                }

                FilteredAdapter.LoadFilteredPolicy(PolicyStore, filter);
                return true;
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
                if (HasAdapter is false)
                {
                    return false;
                }

                if (EpochAdapter is null)
                {
                    throw new InvalidOperationException("Cannot save policy when use a readonly adapter");
                }

                if (IsFiltered)
                {
                    throw new InvalidOperationException("Cannot save filtered policies");
                }

                EpochAdapter.SavePolicy(PolicyStore);
                return true;
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
                if (HasAdapter is false)
                {
                    return false;
                }

                if (EpochAdapter is null)
                {
                    return false;
                }

                PolicyStore.ClearPolicy();
                await EpochAdapter.LoadPolicyAsync(PolicyStore);
                return true;
            }
            finally
            {
                EndWrite();
            }

            ;
        }

        public virtual async Task<bool> LoadFilteredPolicyAsync(Filter filter)
        {
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                if (FilteredAdapter is null)
                {
                    return false;
                }

                await FilteredAdapter.LoadFilteredPolicyAsync(PolicyStore, filter);
                return true;
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
                if (EpochAdapter is null)
                {
                    return false;
                }

                await EpochAdapter.SavePolicyAsync(PolicyStore);
                return true;
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

        public IEnumerable<IEnumerable<string>> GetFilteredPolicy(string section, string policyType, int fieldIndex,
            params string[] fieldValues)
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

        public bool HasAllPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            StartRead();
            try
            {
                return PolicyStore.HasAllPolicies(section, policyType, rules);
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

        public bool UpdatePolicy(string section, string policyType, IEnumerable<string> oldRule, IEnumerable<string> newRule)
        {
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                IEnumerable<string> oldRuleArray = oldRule as string[] ?? oldRule.ToArray();
                IEnumerable<string> newRuleArray = newRule as string[] ?? newRule.ToArray();
                if (HasAdapter is false || AutoSave is false)
                {
                    return PolicyStore.UpdatePolicy(section, policyType, oldRuleArray, newRuleArray);
                }

                SingleAdapter?.UpdatePolicy(section, policyType, oldRuleArray, newRuleArray);
                return PolicyStore.UpdatePolicy(section, policyType, oldRuleArray, newRuleArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public bool UpdatePolicies(string section, string policyType, IEnumerable<IEnumerable<string>> oldRules, IEnumerable<IEnumerable<string>> newRules)
        {
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                var oldRulesArray = oldRules as IEnumerable<string>[] ?? oldRules.ToArray();
                var newRulesArray = newRules as IEnumerable<string>[] ?? newRules.ToArray();
                if (HasAdapter is false || AutoSave is false)
                {
                    return PolicyStore.UpdatePolicies(section, policyType, oldRulesArray, newRulesArray);
                }

                BatchAdapter?.UpdatePolicies(section, policyType, oldRulesArray, newRulesArray);
                return PolicyStore.UpdatePolicies(section, policyType, oldRulesArray, newRulesArray);
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

        public IEnumerable<IEnumerable<string>> RemoveFilteredPolicy(string section, string policyType, int fieldIndex,
            params string[] fieldValues)
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

        public virtual async Task<bool> AddPoliciesAsync(string section, string policyType,
            IEnumerable<IEnumerable<string>> rules)
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

        public virtual async Task<bool> UpdatePolicyAsync(string section, string policyType, IEnumerable<string> oldRule, IEnumerable<string> newRule)
        {
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                IEnumerable<string> oldRuleArray = oldRule as string[] ?? oldRule.ToArray();
                IEnumerable<string> newRuleArray = newRule as string[] ?? newRule.ToArray();
                if (HasAdapter is false || AutoSave is false)
                {
                    return PolicyStore.UpdatePolicy(section, policyType, oldRuleArray, newRuleArray);
                }

                if (SingleAdapter is not null)
                {
                    await SingleAdapter.UpdatePolicyAsync(section, policyType, oldRuleArray, newRuleArray);
                }

                return PolicyStore.UpdatePolicy(section, policyType, oldRuleArray, newRuleArray);
            }
            finally
            {
                EndWrite();
            }
        }

        public virtual async Task<bool> UpdatePoliciesAsync(string section, string policyType,
            IEnumerable<IEnumerable<string>> oldRules, IEnumerable<IEnumerable<string>> newRules)
        {
            if (TryStartWrite() is false)
            {
                return false;
            }

            try
            {
                var oldRulesArray = oldRules as IEnumerable<string>[] ?? oldRules.ToArray();
                var newRulesArray = newRules as IEnumerable<string>[] ?? newRules.ToArray();
                if (HasAdapter is false || AutoSave is false)
                {
                    return PolicyStore.UpdatePolicies(section, policyType, oldRulesArray, newRulesArray);
                }

                if (BatchAdapter is not null)
                {
                    await BatchAdapter.UpdatePoliciesAsync(section, policyType, oldRulesArray, newRulesArray);
                }

                return PolicyStore.UpdatePolicies(section, policyType, oldRulesArray, newRulesArray);
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

        public virtual async Task<bool> RemovePoliciesAsync(string section, string policyType,
            IEnumerable<IEnumerable<string>> rules)
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

        public virtual async Task<IEnumerable<IEnumerable<string>>> RemoveFilteredPolicyAsync(string section,
            string policyType, int fieldIndex, params string[] fieldValues)
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

        public static IPolicyManager Create() => new DefaultPolicyManager(DefaultPolicyStore.Create());
    }
}
