using System.Collections.Generic;
using Casbin.Model;

namespace Casbin.Persist
{
    public class WatcherMessage: IWatcherMessage
    {
        public PolicyOperation Operation { get; }
        public string Section { get; }
        public string PolicyType { get; }
        public int FieldIndex { get; }
        public IPolicyValues Values { get; }
        public IPolicyValues NewValues { get; }
        public IReadOnlyList<IPolicyValues> ValuesList { get; }
        public IReadOnlyList<IPolicyValues> NewValuesList { get; }

        private WatcherMessage(PolicyOperation operation, string section, string policyType,
            IPolicyValues values = null, IPolicyValues newValues = null, int fieldIndex = -1,
            IReadOnlyList<IPolicyValues> valuesList = null, IReadOnlyList<IPolicyValues> newValuesList = null)
        {
            Operation = operation;
            Section = section;
            PolicyType = policyType;
            FieldIndex = fieldIndex;
            Values = values;
            NewValues = newValues;
            ValuesList = valuesList;
            NewValuesList = newValuesList;
        }

        public static WatcherMessage CreateAddPolicyMessage(string section, string policyType, IPolicyValues rule)
        {
            return new WatcherMessage(PolicyOperation.AddPolicy, section, policyType, rule);
        }

        public static WatcherMessage CreateUpdatePolicyMessage(string section, string policyType,
            IPolicyValues oldRule, IPolicyValues newRule)
        {
            return new WatcherMessage(PolicyOperation.UpdatePolicy, section, policyType, oldRule, newRule);
        }

        public static WatcherMessage CreateRemovePolicyMessage(string section, string policyType, IPolicyValues rule)
        {
            return new WatcherMessage(PolicyOperation.RemovePolicy, section, policyType, rule);
        }

        public static WatcherMessage CreateRemoveFilteredPolicyMessage(string section, string policyType,
            int fieldIndex, IReadOnlyList<IPolicyValues> fieldRules)
        {
            return new WatcherMessage(PolicyOperation.RemoveFilteredPolicy, section, policyType, fieldIndex:fieldIndex,
                valuesList: fieldRules);
        }

        public static WatcherMessage CreateAddPoliciesMessage(string section, string policyType,
            IReadOnlyList<IPolicyValues> rules)
        {
            return new WatcherMessage(PolicyOperation.AddPolicies, section, policyType, valuesList:rules);
        }

        public static WatcherMessage CreateUpdatePoliciesMessage(string section, string policyType,
            IReadOnlyList<IPolicyValues> oldRules, IReadOnlyList<IPolicyValues> newRules)
        {
            return new WatcherMessage(PolicyOperation.UpdatePolicies, section, policyType,
                valuesList:oldRules, newValuesList:newRules);
        }

        public static WatcherMessage CreateRemovePoliciesMessage(string section, string policyType,
            IReadOnlyList<IPolicyValues> rules)
        {
            return new WatcherMessage(PolicyOperation.RemovePolicies, section, policyType, valuesList:rules);
        }

        public static WatcherMessage CreateSavePolicyMessage()
        {
            return new WatcherMessage(PolicyOperation.SavePolicy, "", "");
        }

        public static WatcherMessage CreateCustomMessage(string section, string policyType,
            IPolicyValues values = null, IPolicyValues newValues = null, int fieldIndex = -1,
            IReadOnlyList<IPolicyValues> valuesList = null, IReadOnlyList<IPolicyValues> newValuesList = null)
        {
            return new WatcherMessage(PolicyOperation.Custom, section, policyType, values, newValues, fieldIndex,
                valuesList, newValuesList);
        }
    }
}


