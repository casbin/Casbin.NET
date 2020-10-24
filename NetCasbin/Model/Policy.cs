using System;
using System.Collections.Generic;
using System.Linq;
using NetCasbin.Rbac;
using NetCasbin.Util;

namespace NetCasbin.Model
{
    public class Policy
    {
        public Dictionary<string, Dictionary<string, Assertion>> Model { get; }

        protected Policy()
        {
            Model = new Dictionary<string, Dictionary<string, Assertion>>();
        }

        public void BuildRoleLinks(IRoleManager roleManager)
        {
            if (Model.ContainsKey(PermConstants.Section.RoleSection))
            {
                foreach (Assertion assertion in Model[PermConstants.Section.RoleSection].Values)
                {
                    assertion.BuildRoleLinks(roleManager);
                }
            }
        }

        public void RefreshPolicyStringSet()
        {
            foreach (Assertion assertion in Model.Values
                .SelectMany(pair => pair.Values))
            {
                assertion.RefreshPolicyStringSet();
            }
        }

        public void ClearPolicy()
        {
            if (Model.ContainsKey(PermConstants.Section.PolicySection))
            {
                foreach (Assertion assertion in Model[PermConstants.Section.PolicySection].Values)
                {
                    assertion.ClearPolicy();
                }
            }

            if (Model.ContainsKey(PermConstants.Section.RoleSection))
            {
                foreach (Assertion assertion in Model[PermConstants.Section.RoleSection].Values)
                {
                    assertion.ClearPolicy();
                }
            }
        }

        public List<List<string>> GetPolicy(string sec, string ptype)
        {
            return Model[sec][ptype].Policy;
        }

        public List<List<string>> GetFilteredPolicy(string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            if (fieldValues == null)
            {
                throw new ArgumentNullException(nameof(fieldValues));
            }

            if (fieldValues.Length == 0 || fieldValues.All(string.IsNullOrWhiteSpace))
            {
                return Model[sec][ptype].Policy;
            }

            var result = new List<List<string>>();

            foreach (var rule in Model[sec][ptype].Policy)
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

        public bool HasPolicy(string sec, string ptype, List<string> rule)
        {
            var assertion = GetExistAssertion(sec, ptype);
            return assertion.Contains(rule);
        }

        public bool HasPolicies(string sec, string ptype, IEnumerable<List<string>> rules)
        {
            var assertion = GetExistAssertion(sec, ptype);
            var ruleArray = rules as List<string>[] ?? rules.ToArray();
            return ruleArray.Length == 0 || ruleArray.Any(assertion.Contains);
        }

        public bool AddPolicy(string sec, string ptype, List<string> rule)
        {
            var assertion = GetExistAssertion(sec, ptype);
            return assertion.TryAddPolicy(rule);
        }

        public bool AddPolicies(string sec, string ptype, IEnumerable<List<string>> rules)
        {
            if (rules is null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            var assertion = GetExistAssertion(sec, ptype);
            var ruleArray = rules as List<string>[] ?? rules.ToArray();

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

        public bool RemovePolicy(string sec, string ptype, List<string> rule)
        {
            var assertion = GetExistAssertion(sec, ptype);
            return assertion.TryRemovePolicy(rule);
        }

        public bool RemovePolicies(string sec, string ptype, IEnumerable<List<string>> rules)
        {
            if (rules is null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            var assertion = GetExistAssertion(sec, ptype);
            var ruleArray = rules as List<string>[] ?? rules.ToArray();

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

        public bool RemoveFilteredPolicy(string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            if (fieldValues == null)
            {
                throw new ArgumentNullException(nameof(fieldValues));
            }

            if (fieldValues.Length == 0 || fieldValues.All(string.IsNullOrWhiteSpace))
            {
                return true;
            }

            var newPolicy = new List<List<string>>();
            bool deleted = false;

            Assertion assertion = Model[sec][ptype];
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
                    deleted = true;
                }
                else
                {
                    newPolicy.Add(rule);
                }
            }

            assertion.Policy = newPolicy;
            assertion.RefreshPolicyStringSet();
            return deleted;
        }

        public List<string> GetValuesForFieldInPolicyAllTypes(string sec, int fieldIndex)
        {
            var section = Model[sec];
            var values = new List<string>();

            foreach (string policyType in section.Keys)
            {
                values.AddRange(GetValuesForFieldInPolicy(
                    section, policyType, fieldIndex));
            }

            return values;
        }

        public List<string> GetValuesForFieldInPolicy(string sec, string ptype, int fieldIndex)
        {
            return GetValuesForFieldInPolicy(Model[sec], ptype, fieldIndex);
        }

        private static List<string> GetValuesForFieldInPolicy(IDictionary<string, Assertion> section, string ptype, int fieldIndex)
        {
            var values = section[ptype].Policy
                .Select(rule => rule[fieldIndex])
                .ToList();

            Utility.ArrayRemoveDuplicates(values);
            return values;
        }

        private Assertion GetExistAssertion(string section, string policyType)
        {
            bool exist = TryGetExistAssertion(section, policyType, out var assertion);
            if (!exist)
            {
                throw new ArgumentException($"Can not find the assertion at the {nameof(section)} {section} and {nameof(policyType)} {policyType}.");
            }
            return assertion;
        }

        private bool TryGetExistAssertion(string section, string policyType, out Assertion returnAssertion)
        {
            if (Model[section].TryGetValue(policyType, out var assertion))
            {
                if (assertion is null)
                {
                    returnAssertion = default;
                    return false;
                }
                returnAssertion = assertion;
                return true;
            }
            returnAssertion = default;
            return false;
        }
    }
}
