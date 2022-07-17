using System;
using System.Collections.Generic;
using System.Linq;
#if !NET452
using Microsoft.Extensions.Logging;
#endif

namespace Casbin.Model
{
    public class DefaultPolicyStore : IPolicyStore
    {
        private DefaultPolicyStore() => Sections = new Dictionary<string, Dictionary<string, Assertion>>();

#if !NET452
        internal ILogger Logger { get; set; }
#endif
        public Dictionary<string, Dictionary<string, Assertion>> Sections { get; }

        public void ClearPolicy()
        {
            if (Sections.ContainsKey(PermConstants.Section.PolicySection))
            {
                foreach (Assertion assertion in Sections[PermConstants.Section.PolicySection].Values)
                {
                    assertion.ClearPolicy();
                }
            }

            if (Sections.ContainsKey(PermConstants.Section.RoleSection))
            {
                foreach (Assertion assertion in Sections[PermConstants.Section.RoleSection].Values)
                {
                    assertion.ClearPolicy();
                }
            }
        }

        public IEnumerable<IEnumerable<string>> GetPolicy(string section, string policyType)
        {
            return Sections[section][policyType].Policy;
        }

        public IEnumerable<IEnumerable<string>> GetFilteredPolicy(string section, string policyType, int fieldIndex,
            params string[] fieldValues)
        {
            if (fieldValues == null)
            {
                throw new ArgumentNullException(nameof(fieldValues));
            }

            if (fieldValues.Length == 0 || fieldValues.All(string.IsNullOrWhiteSpace))
            {
                return Sections[section][policyType].Policy;
            }

            var result = new List<IEnumerable<string>>();

            foreach (var rule in Sections[section][policyType].Policy)
            {
                // Matched means all the fieldValue equals rule[fieldIndex + i].
                // when fieldValue is empty, this field will skip equals check.
                bool matched = !fieldValues.Where((fieldValue, i) =>
                        !string.IsNullOrWhiteSpace(fieldValue) &&
                        !rule[fieldIndex + i].Equals(fieldValue))
                    .Any();

                if (matched)
                {
                    result.Add(rule);
                }
            }

            return result;
        }

        public bool HasPolicy(string section, string policyType, IEnumerable<string> rule)
        {
            string[] ruleArray = rule as string[] ?? rule.ToArray();
            IPolicyValues values = Policy.ValuesFrom(ruleArray);
            return HasPolicy(section, policyType, values);
        }

        public bool HasPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            List<IPolicyValues> valuesList = new();
            foreach (IEnumerable<string> rule in rules)
            {
                string[] ruleArray = rule as string[] ?? rule.ToArray();
                IPolicyValues values = Policy.ValuesFrom(ruleArray);
                valuesList.Add(values);
            }

            return HasPolicies(section, policyType, valuesList);
        }

        public bool HasAllPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            List<IPolicyValues> valuesList = new();
            foreach (IEnumerable<string> rule in rules)
            {
                string[] ruleArray = rule as string[] ?? rule.ToArray();
                IPolicyValues values = Policy.ValuesFrom(ruleArray);
                valuesList.Add(values);
            }

            return HasAllPolicies(section, policyType, valuesList);
        }

        public bool AddPolicy(string section, string policyType, IEnumerable<string> rule)
        {
            string[] ruleArray = rule as string[] ?? rule.ToArray();
            IPolicyValues values = Policy.ValuesFrom(ruleArray);
            return AddPolicy(section, policyType, values);
        }

        public bool AddPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            List<IPolicyValues> valuesList = new();
            foreach (IEnumerable<string> rule in rules)
            {
                string[] ruleArray = rule as string[] ?? rule.ToArray();
                IPolicyValues values = Policy.ValuesFrom(ruleArray);
                valuesList.Add(values);
            }

            return AddPolicies(section, policyType, valuesList);
        }

        public bool UpdatePolicy(string section, string policyType, IEnumerable<string> oldRule,
            IEnumerable<string> newRule)
        {
            string[] oldRuleArray = oldRule as string[] ?? oldRule.ToArray();
            string[] newRuleArray = newRule as string[] ?? newRule.ToArray();
            IPolicyValues oldValues = Policy.ValuesFrom(oldRuleArray);
            IPolicyValues newValues = Policy.ValuesFrom(newRuleArray);
            return UpdatePolicy(section, policyType, oldValues, newValues);
        }

        public bool UpdatePolicies(string section, string policyType, IEnumerable<IEnumerable<string>> oldRules,
            IEnumerable<IEnumerable<string>> newRules)
        {
            List<IPolicyValues> oldValuesList = new();
            foreach (IEnumerable<string> rule in oldRules)
            {
                string[] ruleArray = rule as string[] ?? rule.ToArray();
                IPolicyValues values = Policy.ValuesFrom(ruleArray);
                oldValuesList.Add(values);
            }

            List<IPolicyValues> newValuesList = new();
            foreach (IEnumerable<string> rule in newRules)
            {
                string[] ruleArray = rule as string[] ?? rule.ToArray();
                IPolicyValues values = Policy.ValuesFrom(ruleArray);
                newValuesList.Add(values);
            }

            return UpdatePolicies(section, policyType, oldValuesList, newValuesList);
        }

        public bool RemovePolicy(string section, string policyType, IEnumerable<string> rule)
        {
            string[] ruleArray = rule as string[] ?? rule.ToArray();
            IPolicyValues values = Policy.ValuesFrom(ruleArray);
            return RemovePolicy(section, policyType, values);
        }

        public bool RemovePolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            List<IPolicyValues> valuesList = new();
            foreach (IEnumerable<string> rule in rules)
            {
                string[] ruleArray = rule as string[] ?? rule.ToArray();
                IPolicyValues values = Policy.ValuesFrom(ruleArray);
                valuesList.Add(values);
            }

            return RemovePolicies(section, policyType, valuesList);
        }

        public IEnumerable<IEnumerable<string>> RemoveFilteredPolicy(string section, string policyType, int fieldIndex,
            params string[] fieldValues)
        {
            if (fieldValues == null)
            {
                throw new ArgumentNullException(nameof(fieldValues));
            }

            if (fieldValues.Length == 0 || fieldValues.All(string.IsNullOrWhiteSpace))
            {
                return null;
            }

            var newPolicy = new List<IPolicyValues>();
            List<IEnumerable<string>> effectPolicies = null;

            Assertion assertion = Sections[section][policyType];
            foreach (IPolicyValues values in assertion.Policy)
            {
                // Matched means all the fieldValue equals rule[fieldIndex + i].
                // when fieldValue is empty, this field will skip equals check.
                bool matched = !fieldValues.Where((fieldValue, i) =>
                        !string.IsNullOrWhiteSpace(fieldValue) &&
                        !values[fieldIndex + i].Equals(fieldValue))
                    .Any();

                if (matched)
                {
                    effectPolicies ??= new List<IEnumerable<string>>();
                    effectPolicies.Add(values);
                    assertion.PolicyStringSet.Remove(values.ToText());
                }
                else
                {
                    newPolicy.Add(values);
                }
            }

            assertion.Policy = newPolicy;
            return effectPolicies;
        }

        public IEnumerable<string> GetValuesForFieldInPolicyAllTypes(string section, int fieldIndex)
        {
            var sectionDictionary = Sections[section];
            var values = new List<string>();
            foreach (string policyType in sectionDictionary.Keys)
            {
                values.AddRange(GetValuesForFieldInPolicy(sectionDictionary, policyType, fieldIndex));
            }

            return values;
        }

        public IEnumerable<string> GetValuesForFieldInPolicy(string section, string policyType, int fieldIndex)
        {
            return GetValuesForFieldInPolicy(Sections[section], policyType, fieldIndex);
        }

        public Assertion GetRequiredAssertion(string section, string policyType)
        {
            bool exist = TryGetAssertion(section, policyType, out Assertion assertion);
            if (exist is false)
            {
                throw new ArgumentException(
                    $"Can not find the assertion at the {nameof(section)} {section} and {nameof(policyType)} {policyType}.");
            }

            return assertion;
        }

        public bool TryGetAssertion(string section, string policyType, out Assertion returnAssertion)
        {
            if (Sections.TryGetValue(section, out Dictionary<string, Assertion> assertions) is false)
            {
                returnAssertion = default;
                return false;
            }

            if (assertions.TryGetValue(policyType, out Assertion assertion) is false)
            {
                returnAssertion = default;
                return false;
            }

            if (assertion is null)
            {
                returnAssertion = default;
                return false;
            }

            returnAssertion = assertion;
            return true;
        }

        public bool AddPolicy(string section, string policyType, IPolicyValues values)
        {
            Assertion assertion = GetRequiredAssertion(section, policyType);
            return assertion.TryAddPolicy(values);
        }

        private bool HasPolicy(string section, string policyType, IPolicyValues values)
        {
            Assertion assertion = GetRequiredAssertion(section, policyType);
            return assertion.Contains(values);
        }


        private bool HasPolicies(string section, string policyType, IEnumerable<IPolicyValues> valuesList)
        {
            Assertion assertion = GetRequiredAssertion(section, policyType);
            List<IPolicyValues> list = valuesList as List<IPolicyValues> ?? valuesList.ToList();
            return list.Count == 0 || list.Any(assertion.Contains);
        }

        private bool HasAllPolicies(string section, string policyType, IEnumerable<IPolicyValues> valuesList)
        {
            Assertion assertion = GetRequiredAssertion(section, policyType);
            List<IPolicyValues> list = valuesList as List<IPolicyValues> ?? valuesList.ToList();
            return list.Count is 0 || list.All(assertion.Contains);
        }

        private bool AddPolicies(string section, string policyType, IEnumerable<IPolicyValues> valuesList)
        {
            if (valuesList is null)
            {
                throw new ArgumentNullException(nameof(valuesList));
            }

            Assertion assertion = GetRequiredAssertion(section, policyType);
            List<IPolicyValues> list = valuesList as List<IPolicyValues> ?? valuesList.ToList();
            if (list.Count is 0)
            {
                return true;
            }

            foreach (IPolicyValues values in list)
            {
                assertion.TryAddPolicy(values);
            }

            return true;
        }

        private bool UpdatePolicy(string section, string policyType, IPolicyValues oldValues, IPolicyValues newValues)
        {
            Assertion assertion = GetRequiredAssertion(section, policyType);
            return assertion.TryUpdatePolicy(oldValues, newValues);
        }

        private bool UpdatePolicies(string section, string policyType, IEnumerable<IPolicyValues> oldValuesList,
            IEnumerable<IPolicyValues> newValuesList)
        {
            if (oldValuesList is null)
            {
                throw new ArgumentNullException(nameof(oldValuesList));
            }

            if (newValuesList is null)
            {
                throw new ArgumentNullException(nameof(newValuesList));
            }

            Assertion assertion = GetRequiredAssertion(section, policyType);
            List<IPolicyValues> oldList = oldValuesList as List<IPolicyValues> ?? oldValuesList.ToList();
            List<IPolicyValues> newList = newValuesList as List<IPolicyValues> ?? newValuesList.ToList();
            if (oldList.Count != newList.Count)
            {
                return false;
            }

            for (int i = 0; i < oldList.Count; i++)
            {
                assertion.TryUpdatePolicy(oldList[i], newList[i]);
            }

            return true;
        }

        private bool RemovePolicy(string section, string policyType, IPolicyValues values)
        {
            Assertion assertion = GetRequiredAssertion(section, policyType);
            return assertion.TryRemovePolicy(values);
        }

        private bool RemovePolicies(string section, string policyType, IEnumerable<IPolicyValues> valuesList)
        {
            if (valuesList is null)
            {
                throw new ArgumentNullException(nameof(valuesList));
            }

            Assertion assertion = GetRequiredAssertion(section, policyType);
            List<IPolicyValues> list = valuesList as List<IPolicyValues> ?? valuesList.ToList();
            if (list.Count is 0)
            {
                return true;
            }

            foreach (IPolicyValues values in list)
            {
                assertion.TryRemovePolicy(values);
            }

            return true;
        }

        internal static IPolicyStore Create() => new DefaultPolicyStore();

        private static IEnumerable<string> GetValuesForFieldInPolicy(IDictionary<string, Assertion> section,
            string policyType, int fieldIndex)
        {
            List<string> values = section[policyType].Policy
                .Select(rule => rule[fieldIndex])
                .Distinct().ToList();
            return values;
        }
    }
}
