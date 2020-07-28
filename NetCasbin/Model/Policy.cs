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

        public void BuildRoleLinks(IRoleManager rm)
        {
            if (Model.ContainsKey(PermConstants.Section.RoleSection))
            {
                foreach (Assertion assertion in Model[PermConstants.Section.RoleSection].Values)
                {
                    assertion.BuildRoleLinks(rm);
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
            return Model[sec][ptype] != null && Model[sec][ptype].Contains(rule);
        }

        public bool AddPolicy(string sec, string ptype, List<string> rule)
        {
            if (HasPolicy(sec, ptype, rule))
            {
                return false;
            }

            Assertion assertion = Model[sec][ptype];
            return assertion.AddPolicy(rule);
        }

        public bool RemovePolicy(string sec, string ptype, List<string> rule)
        {
            if (!HasPolicy(sec, ptype, rule))
            {
                return true;
            }

            Assertion assertion = Model[sec][ptype];
            return assertion.RemovePolicy(rule);
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

        public List<string> GetValuesForFieldInPolicy(string sec, string ptype, int fieldIndex)
        {
            var values = Model[sec][ptype].Policy
                .Select(rule => rule[fieldIndex])
                .ToList();

            Utility.ArrayRemoveDuplicates(values);
            return values;
        }
    }
}
