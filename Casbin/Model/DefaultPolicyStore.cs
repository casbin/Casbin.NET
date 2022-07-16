using System;
using System.Collections.Generic;
using System.Linq;
#if !NET452
using Microsoft.Extensions.Logging;
#endif
using Casbin.Util;

namespace Casbin.Model
{
    public class DefaultPolicyStore : IPolicyStore
    {
        public Dictionary<string, Dictionary<string, Assertion>> Sections { get; }

#if !NET452
        internal ILogger Logger { get; set; }
#endif

        private DefaultPolicyStore()
        {
            Sections = new Dictionary<string, Dictionary<string, Assertion>>();
        }

        internal static IPolicyStore Create()
        {
            return new DefaultPolicyStore();
        }

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

        public IEnumerable<IEnumerable<string>> GetFilteredPolicy(string section, string policyType, int fieldIndex, params string[] fieldValues)
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
            var assertion = GetRequiredAssertion(section, policyType);
            return assertion.Contains(rule);
        }

        public bool HasPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            var assertion = GetRequiredAssertion(section, policyType);
            var ruleArray = rules as IEnumerable<string>[] ?? rules.ToArray();
            return ruleArray.Length == 0 || ruleArray.Any(assertion.Contains);
        }

        public bool HasAllPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            var assertion = GetRequiredAssertion(section, policyType);
            var ruleArray = rules as IEnumerable<string>[] ?? rules.ToArray();
            return ruleArray.Length == 0 || ruleArray.All(assertion.Contains);
        }

        public bool AddPolicy(string section, string policyType, IEnumerable<string> rule)
        {
            var assertion = GetRequiredAssertion(section, policyType);
            return assertion.TryAddPolicy(rule);
        }

        public bool AddPolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            if (rules is null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            var assertion = GetRequiredAssertion(section, policyType);
            var ruleArray = rules as IEnumerable<string>[] ?? rules.ToArray();

            if (ruleArray.Length == 0)
            {
                return true;
            }

            foreach (var rule in ruleArray)
            {
                assertion.TryAddPolicy(rule);
            }
            return true;
        }

        public bool UpdatePolicy(string section, string policyType, IEnumerable<string> oldRule, IEnumerable<string> newRule)
        {
            var assertion = GetRequiredAssertion(section, policyType);
            return assertion.TryUpdatePolicy(oldRule, newRule);
        }

        public bool UpdatePolicies(string section, string policyType, IEnumerable<IEnumerable<string>> oldRules, IEnumerable<IEnumerable<string>> newRules)
        {
            if (oldRules is null)
            {
                throw new ArgumentNullException(nameof(oldRules));
            }

            var assertion = GetRequiredAssertion(section, policyType);
            var oldRulesArray = oldRules as IEnumerable<string>[] ?? oldRules.ToArray();
            var newRulesArray = newRules as IEnumerable<string>[] ?? newRules.ToArray();

            if (oldRulesArray.Length != newRulesArray.Length)
            {
                return false;
            }

            for (int i = 0; i < oldRulesArray.Length; i++)
            {
                assertion.TryUpdatePolicy(oldRulesArray[i], newRulesArray[i]);
            }
            return true;
        }

        public bool RemovePolicy(string section, string policyType, IEnumerable<string> rule)
        {
            var assertion = GetRequiredAssertion(section, policyType);
            return assertion.TryRemovePolicy(rule);
        }

        public bool RemovePolicies(string section, string policyType, IEnumerable<IEnumerable<string>> rules)
        {
            if (rules is null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            var assertion = GetRequiredAssertion(section, policyType);
            var ruleArray = rules as IEnumerable<string>[] ?? rules.ToArray();

            if (ruleArray.Length == 0)
            {
                return true;
            }

            foreach (var rule in ruleArray)
            {
                assertion.TryRemovePolicy(rule);
            }
            return true;
        }

        public IEnumerable<IEnumerable<string>> RemoveFilteredPolicy(string section, string policyType, int fieldIndex, params string[] fieldValues)
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
            foreach (var rule in assertion.Policy)
            {
                // Matched means all the fieldValue equals rule[fieldIndex + i].
                // when fieldValue is empty, this field will skip equals check.
                bool matched = !fieldValues.Where((fieldValue, i) =>
                        !string.IsNullOrWhiteSpace(fieldValue) &&
                        !rule[fieldIndex + i].Equals(fieldValue))
                    .Any();

                if (matched)
                {
                    effectPolicies ??= new List<IEnumerable<string>>();
                    effectPolicies.Add(rule);
                    assertion.PolicyStringSet.Remove(Utility.RuleToString(rule));
                }
                else
                {
                    newPolicy.Add(rule);
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

        private static IEnumerable<string> GetValuesForFieldInPolicy(IDictionary<string, Assertion> section, string policyType, int fieldIndex)
        {
            var values = section[policyType].Policy
                .Select(rule => rule[fieldIndex])
                .Distinct().ToList();
            return values;
        }

        public Assertion GetRequiredAssertion(string section, string policyType)
        {
            bool exist = TryGetAssertion(section, policyType, out Assertion assertion);
            if (exist is false)
            {
                throw new ArgumentException($"Can not find the assertion at the {nameof(section)} {section} and {nameof(policyType)} {policyType}.");
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
    }
}
