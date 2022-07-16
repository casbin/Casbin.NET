using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Casbin.Persist;

namespace Casbin.Model
{
    public class ReaderWriterPolicyManager : DefaultPolicyManager
    {
        private readonly ReaderWriterLockSlim _lockSlim = new();
        private readonly ReaderWriterPolicyManagerOptions _options;

        // ReSharper disable once MemberCanBePrivate.Global
        public ReaderWriterPolicyManager(IPolicyStore policyStore, IReadOnlyAdapter adapter = null)
            : base(policyStore, adapter)
        {
            _options = new ReaderWriterPolicyManagerOptions();
        }

        public ReaderWriterPolicyManager(IPolicyStore policyStore, ReaderWriterPolicyManagerOptions options,
            IAdapter adapter = null)
            : base(policyStore, adapter)
        {
            _options = options;
        }

        public override bool IsSynchronized => true;

        public static new IPolicyManager Create()
        {
            return new ReaderWriterPolicyManager(DefaultPolicyStore.Create());
        }

        public override void StartRead()
        {
            _lockSlim.EnterReadLock();
        }

        public override void StartWrite()
        {
            _lockSlim.EnterWriteLock();
        }

        public override void EndRead()
        {
            _lockSlim.ExitReadLock();
        }

        public override void EndWrite()
        {
            _lockSlim.ExitWriteLock();
        }

        public override bool TryStartRead()
        {
            return _lockSlim.TryEnterReadLock(_options.WaitTimeOut);
        }

        public override bool TryStartWrite()
        {
            return _lockSlim.TryEnterWriteLock(_options.WaitTimeOut);
        }

        public override Task<bool> LoadPolicyAsync()
        {
            return Task.Run(() =>
            {
                if (TryStartWrite() is false)
                {
                    return Task.FromResult(false);
                }

                try
                {
                    if (HasAdapter is false)
                    {
                        return Task.FromResult(false);
                    }

                    if (EpochAdapter is null)
                    {
                        return Task.FromResult(false);
                    }

                    PolicyStore.ClearPolicy();
                    EpochAdapter.LoadPolicyAsync(PolicyStore).Wait();
                    return Task.FromResult(true);
                }
                finally
                {
                    EndWrite();
                }
            });
        }

        public override Task<bool> LoadFilteredPolicyAsync(Filter filter)
        {
            return Task.Run(() =>
            {
                if (TryStartWrite() is false)
                {
                    return Task.FromResult(false);
                }

                try
                {
                    if (FilteredAdapter is not null)
                    {
                        FilteredAdapter.LoadFilteredPolicyAsync(PolicyStore, filter).Wait();
                        return Task.FromResult(true);
                    }

                    return Task.FromResult(false);
                }
                finally
                {
                    EndWrite();
                }
            });
        }

        public override Task<bool> SavePolicyAsync()
        {
            return Task.Run(() =>
            {
                if (TryStartRead() is false)
                {
                    return Task.FromResult(false);
                }

                try
                {
                    if (EpochAdapter is not null)
                    {
                        EpochAdapter.SavePolicyAsync(PolicyStore).Wait();
                        return Task.FromResult(true);
                    }

                    return Task.FromResult(false);
                }
                finally
                {
                    EndRead();
                }
            });
        }

        public override Task<bool> AddPolicyAsync(string section, string policyType, IEnumerable<string> rule)
        {
            return Task.Run(() =>
            {
                if (TryStartWrite() is false)
                {
                    return Task.FromResult(false);
                }

                try
                {
                    IEnumerable<string> ruleArray = rule as string[] ?? rule.ToArray();
                    if (HasAdapter is false || AutoSave is false)
                    {
                        return Task.FromResult(PolicyStore.AddPolicy(section, policyType, ruleArray));
                    }

                    if (SingleAdapter is not null)
                    {
                        SingleAdapter.AddPolicyAsync(section, policyType, ruleArray).Wait();
                    }

                    return Task.FromResult(PolicyStore.AddPolicy(section, policyType, ruleArray));
                }
                finally
                {
                    EndWrite();
                }
            });
        }

        public override Task<bool> AddPoliciesAsync(string section, string policyType,
            IEnumerable<IEnumerable<string>> rules)
        {
            return Task.Run(() =>
            {
                if (TryStartWrite() is false)
                {
                    return Task.FromResult(false);
                }

                try
                {
                    var rulesArray = rules as IEnumerable<string>[] ?? rules.ToArray();
                    if (HasAdapter is false || AutoSave is false)
                    {
                        return Task.FromResult(PolicyStore.AddPolicies(section, policyType, rulesArray));
                    }

                    if (BatchAdapter is not null)
                    {
                        BatchAdapter.AddPoliciesAsync(section, policyType, rulesArray).Wait();
                    }

                    return Task.FromResult(PolicyStore.AddPolicies(section, policyType, rulesArray));
                }
                finally
                {
                    EndWrite();
                }
            });
        }

        public override Task<bool> UpdatePolicyAsync(string section, string policyType, IEnumerable<string> oldRule, IEnumerable<string> newRule)
        {
            return Task.Run(() =>
            {
                if (TryStartWrite() is false)
                {
                    return Task.FromResult(false);
                }

                try
                {
                    IEnumerable<string> oldRuleArray = oldRule as string[] ?? oldRule.ToArray();
                    IEnumerable<string> newRuleArray = newRule as string[] ?? newRule.ToArray();
                    if (HasAdapter is false || AutoSave is false)
                    {
                        return Task.FromResult(
                            PolicyStore.UpdatePolicy(section, policyType, oldRuleArray, newRuleArray));
                    }

                    if (SingleAdapter is not null)
                    {
                        SingleAdapter.UpdatePolicyAsync(section, policyType, oldRuleArray, newRuleArray).Wait();
                    }

                    return Task.FromResult(PolicyStore.UpdatePolicy(section, policyType, oldRuleArray, newRuleArray));
                }
                finally
                {
                    EndWrite();
                }
            });
        }

        public override Task<bool> UpdatePoliciesAsync(string section, string policyType,
            IEnumerable<IEnumerable<string>> oldRules, IEnumerable<IEnumerable<string>> newRules)
        {
            return Task.Run(() =>
            {
                if (TryStartWrite() is false)
                {
                    return Task.FromResult(false);
                }

                try
                {
                    var oldRulesArray = oldRules as IEnumerable<string>[] ?? oldRules.ToArray();
                    var newRulesArray = newRules as IEnumerable<string>[] ?? newRules.ToArray();
                    if (HasAdapter is false || AutoSave is false)
                    {
                        return Task.FromResult(PolicyStore.UpdatePolicies(section, policyType, oldRulesArray, newRulesArray));
                    }

                    if (BatchAdapter is not null)
                    {
                        BatchAdapter.UpdatePoliciesAsync(section, policyType, oldRulesArray, newRulesArray).Wait();
                    }

                    return Task.FromResult(PolicyStore.UpdatePolicies(section, policyType, oldRulesArray, newRulesArray));
                }
                finally
                {
                    EndWrite();
                }
            });
        }

        public override Task<bool> RemovePolicyAsync(string section, string policyType, IEnumerable<string> rule)
        {
            return Task.Run(() =>
            {
                if (TryStartWrite() is false)
                {
                    return Task.FromResult(false);
                }

                try
                {
                    string[] ruleArray = rule as string[] ?? rule.ToArray();
                    if (HasAdapter is false || AutoSave is false)
                    {
                        return Task.FromResult(PolicyStore.RemovePolicy(section, policyType, ruleArray));
                    }

                    if (SingleAdapter is not null)
                    {
                        SingleAdapter.RemovePolicyAsync(section, policyType, ruleArray).Wait();
                    }

                    return Task.FromResult(PolicyStore.RemovePolicy(section, policyType, ruleArray));
                }
                finally
                {
                    EndWrite();
                }
            });
        }

        public override Task<bool> RemovePoliciesAsync(string section, string policyType,
            IEnumerable<IEnumerable<string>> rules)
        {
            return Task.Run(() =>
            {
                if (TryStartWrite() is false)
                {
                    return Task.FromResult(false);
                }

                try
                {
                    var rulesArray = rules as IEnumerable<string>[] ?? rules.ToArray();
                    if (HasAdapter is false || AutoSave is false)
                    {
                        return Task.FromResult(PolicyStore.RemovePolicies(section, policyType, rulesArray));
                    }

                    if (BatchAdapter is not null)
                    {
                        BatchAdapter.RemovePoliciesAsync(section, policyType, rulesArray).Wait();
                    }

                    return Task.FromResult(PolicyStore.RemovePolicies(section, policyType, rulesArray));
                }
                finally
                {
                    EndWrite();
                }
            });
        }

        public override Task<IEnumerable<IEnumerable<string>>> RemoveFilteredPolicyAsync(string section,
            string policyType, int fieldIndex, params string[] fieldValues)
        {
            return Task.Run(() =>
            {
                if (TryStartWrite() is false)
                {
                    return Task.FromResult<IEnumerable<IEnumerable<string>>>(null);
                }

                try
                {
                    if (HasAdapter is false || AutoSave is false)
                    {
                        return Task.FromResult(PolicyStore.RemoveFilteredPolicy(section, policyType, fieldIndex,
                            fieldValues));
                    }

                    if (BatchAdapter is not null)
                    {
                        BatchAdapter.RemoveFilteredPolicyAsync(section, policyType, fieldIndex, fieldValues).Wait();
                    }

                    return Task.FromResult(PolicyStore.RemoveFilteredPolicy(section, policyType, fieldIndex,
                        fieldValues));
                }
                finally
                {
                    EndWrite();
                }
            });
        }
    }
}
