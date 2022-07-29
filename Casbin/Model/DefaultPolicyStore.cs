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

        public IEnumerable<IPolicyValues> GetPolicy(string section, string policyType)
        {
            return Sections[section][policyType].Policy;
        }

        public IEnumerable<IPolicyValues> GetFilteredPolicy(string section, string policyType, int fieldIndex,
            IPolicyValues fieldValues)
        {
            if (fieldValues == null)
            {
                throw new ArgumentNullException(nameof(fieldValues));
            }

            if (fieldValues.Count == 0 || fieldValues.All(string.IsNullOrWhiteSpace))
            {
                return Sections[section][policyType].Policy;
            }

            var result = new List<IPolicyValues>();

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

        public IEnumerable<IPolicyValues> RemoveFilteredPolicy(string section, string policyType, int fieldIndex,
            IPolicyValues fieldValues)
        {
            if (fieldValues == null)
            {
                throw new ArgumentNullException(nameof(fieldValues));
            }

            if (fieldValues.Count == 0 || fieldValues.All(string.IsNullOrWhiteSpace))
            {
                return null;
            }

            var newPolicy = new List<IPolicyValues>();
            List<IPolicyValues> effectPolicies = null;

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
                    effectPolicies ??= new List<IPolicyValues>();
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

        public bool HasPolicy(string section, string policyType, IPolicyValues values)
        {
            Assertion assertion = GetRequiredAssertion(section, policyType);
            return assertion.Contains(values);
        }


        public bool HasPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList)
        {
            Assertion assertion = GetRequiredAssertion(section, policyType);
            List<IPolicyValues> list = valuesList as List<IPolicyValues> ?? valuesList.ToList();
            return list.Count == 0 || list.Any(assertion.Contains);
        }

        public bool HasAllPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList)
        {
            Assertion assertion = GetRequiredAssertion(section, policyType);
            List<IPolicyValues> list = valuesList as List<IPolicyValues> ?? valuesList.ToList();
            return list.Count is 0 || list.All(assertion.Contains);
        }

        public bool AddPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList)
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

        public bool UpdatePolicy(string section, string policyType, IPolicyValues oldValues, IPolicyValues newValues)
        {
            Assertion assertion = GetRequiredAssertion(section, policyType);
            return assertion.TryUpdatePolicy(oldValues, newValues);
        }

        public bool UpdatePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> oldValuesList,
            IReadOnlyList<IPolicyValues> newValuesList)
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

        public bool RemovePolicy(string section, string policyType, IPolicyValues values)
        {
            Assertion assertion = GetRequiredAssertion(section, policyType);
            return assertion.TryRemovePolicy(values);
        }

        public bool RemovePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList)
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
            IEnumerable<string> values = section[policyType].Policy
                .Select(rule => rule[fieldIndex])
                .Distinct();
            return values;
        }
    }
}
