using System;
using System.Collections.Generic;
using System.Linq;
using Casbin.Rbac;

namespace Casbin.Model
{
    /// <summary>
    /// Represents an expression in a section of the model.
    /// For example: r = sub, obj, act
    /// </summary>
    public class Assertion : IReadOnlyAssertion
    {
        private List<IPolicyValues> _policy;

        public Assertion()
        {
            _policy = new List<IPolicyValues>();
            PolicyStringSet = new HashSet<string>();
            RoleManager = new DefaultRoleManager(10);
        }

        public IRoleManager RoleManager { get; internal set; }

        internal HashSet<string> PolicyStringSet { get; }

        public string Key { get; internal set; }

        public string Value { get; internal set; }

        public IReadOnlyDictionary<string, int> Tokens { get; internal set; }

        public IReadOnlyList<IPolicyValues> Policy
        {
            get => _policy;
            internal set => _policy = value as List<IPolicyValues> ?? value.ToList();
        }

        public void RefreshPolicyStringSet()
        {
            PolicyStringSet.Clear();
            foreach (var rule in Policy)
            {
                PolicyStringSet.Add(rule.ToText());
            }
        }

        internal void BuildIncrementalRoleLink(PolicyOperation policyOperation, IEnumerable<string> rule)
        {
            int count = Value.Count(c => c is '_');
            if (count < 2)
            {
                throw new InvalidOperationException("the number of \"_\" in role definition should be at least 2.");
            }

            BuildRoleLink(count, policyOperation, rule);
        }

        internal void BuildIncrementalRoleLink(PolicyOperation policyOperation, IEnumerable<string> oldRule,
            IEnumerable<string> newRule)
        {
            int count = Value.Count(c => c is '_');
            if (count < 2)
            {
                throw new InvalidOperationException("the number of \"_\" in role definition should be at least 2.");
            }

            BuildRoleLink(count, policyOperation, oldRule, newRule);
        }

        internal void BuildIncrementalRoleLinks(PolicyOperation policyOperation, IEnumerable<IEnumerable<string>> rules)
        {
            int count = Value.Count(c => c is '_');
            if (count < 2)
            {
                throw new InvalidOperationException("the number of \"_\" in role definition should be at least 2.");
            }

            foreach (var rule in rules)
            {
                BuildRoleLink(count, policyOperation, rule);
            }
        }

        internal void BuildIncrementalRoleLinks(PolicyOperation policyOperation,
            IEnumerable<IEnumerable<string>> oldRules, IEnumerable<IEnumerable<string>> newRules)
        {
            int count = Value.Count(c => c is '_');
            if (count < 2)
            {
                throw new InvalidOperationException("the number of \"_\" in role definition should be at least 2.");
            }

            IReadOnlyList<IEnumerable<string>> rulesList =
                oldRules as IReadOnlyList<IEnumerable<string>> ?? oldRules.ToList();
            IReadOnlyList<IEnumerable<string>> newRulesList =
                newRules as IReadOnlyList<IEnumerable<string>> ?? newRules.ToList();
            if (rulesList.Count != newRulesList.Count)
            {
                throw new InvalidOperationException(
                    $"the length of oldPolices should be equal to the length of newPolices, but got the length of oldPolices is {rulesList.Count}, the length of newPolices is {newRulesList.Count}.");
            }

            for (int i = 0; i < rulesList.Count; i++)
            {
                BuildRoleLink(count, policyOperation, rulesList[i], newRulesList[i]);
            }
        }

        public void BuildRoleLinks()
        {
            int count = Value.Count(c => c is '_');
            if (count < 2)
            {
                throw new InvalidOperationException("the number of \"_\" in role definition should be at least 2.");
            }

            foreach (IPolicyValues rule in Policy)
            {
                BuildRoleLink(count, PolicyOperation.AddPolicy, rule);
            }
        }

        private void BuildRoleLink(int groupPolicyCount,
            PolicyOperation policyOperation, IEnumerable<string> rule, params IEnumerable<string>[] newRule)
        {
            var roleManager = RoleManager;
            List<string> ruleEnum = rule as List<string> ?? rule.ToList();
            int ruleCount = ruleEnum.Count;

            if (ruleCount < groupPolicyCount)
            {
                throw new InvalidOperationException("Grouping policy elements do not meet role definition.");
            }

            if (ruleCount > groupPolicyCount)
            {
                ruleEnum = ruleEnum.GetRange(0, groupPolicyCount);
            }

            switch (policyOperation)
            {
                case PolicyOperation.AddPolicy:
                case PolicyOperation.AddPolicies:
                    switch (groupPolicyCount)
                    {
                        case 2:
                            roleManager.AddLink(ruleEnum[0], ruleEnum[1]);
                            break;
                        case 3:
                            roleManager.AddLink(ruleEnum[0], ruleEnum[1], ruleEnum[2]);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(groupPolicyCount), groupPolicyCount, null);
                    }

                    break;
                case PolicyOperation.UpdatePolicy:
                case PolicyOperation.UpdatePolicies:
                    if (newRule.Length == 0)
                    {
                        throw new InvalidOperationException("Grouping policy elements do not meet role definition.");
                    }

                    List<string> newRuleEnum = newRule[0] as List<string> ?? newRule[0].ToList();
                    int newRuleCount = newRuleEnum.Count;
                    if (newRuleCount < groupPolicyCount)
                    {
                        throw new InvalidOperationException("Grouping policy elements do not meet role definition.");
                    }

                    if (newRuleCount > groupPolicyCount)
                    {
                        newRuleEnum = newRuleEnum.GetRange(0, groupPolicyCount);
                    }

                    switch (groupPolicyCount)
                    {
                        case 2:
                            roleManager.DeleteLink(ruleEnum[0], ruleEnum[1]);
                            roleManager.AddLink(newRuleEnum[0], newRuleEnum[1]);
                            break;
                        case 3:
                            roleManager.DeleteLink(ruleEnum[0], ruleEnum[1], ruleEnum[2]);
                            roleManager.AddLink(newRuleEnum[0], newRuleEnum[1], newRuleEnum[2]);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(groupPolicyCount), groupPolicyCount, null);
                    }

                    break;
                case PolicyOperation.RemovePolicy:
                case PolicyOperation.RemovePolicies:
                case PolicyOperation.RemoveFilteredPolicy:
                    switch (groupPolicyCount)
                    {
                        case 2:
                            roleManager.DeleteLink(ruleEnum[0], ruleEnum[1]);
                            break;
                        case 3:
                            roleManager.DeleteLink(ruleEnum[0], ruleEnum[1], ruleEnum[2]);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(groupPolicyCount), groupPolicyCount, null);
                    }

                    break;
                case PolicyOperation.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(policyOperation), policyOperation, null);
            }
        }

        internal bool Contains(IPolicyValues values) => PolicyStringSet.Contains(values.ToText());

        internal bool TryAddPolicy(IPolicyValues values)
        {
            if (Contains(values))
            {
                return false;
            }

            if (TryGetPriorityIndex(out int index))
            {
                return TryAddPolicyByPriority(values, index);
            }

            _policy.Add(values);
            PolicyStringSet.Add(values.ToText());
            return true;
        }

        internal bool TryUpdatePolicy(IPolicyValues oldValues, IPolicyValues newValues)
        {
            if (Contains(oldValues) is false)
            {
                return false;
            }

            for (int i = 0; i < Policy.Count; i++)
            {
                IPolicyValues ruleInPolicy = Policy[i];
                if (ruleInPolicy.Equals(oldValues) is false)
                {
                    continue;
                }

                _policy.RemoveAt(i);
                PolicyStringSet.Remove(oldValues.ToText());
                _policy.Insert(i, newValues);
                PolicyStringSet.Add(newValues.ToText());
                return true;
            }

            return true;
        }

        internal bool TryRemovePolicy(IPolicyValues values)
        {
            if (Contains(values) is false)
            {
                return false;
            }

            for (int i = 0; i < Policy.Count; i++)
            {
                var ruleInPolicy = Policy[i];
                if (ruleInPolicy.Equals(values) is false)
                {
                    continue;
                }

                _policy.RemoveAt(i);
                PolicyStringSet.Remove(values.ToText());
                return true;
            }

            return true;
        }

        internal void ClearPolicy()
        {
            _policy.Clear();
            PolicyStringSet.Clear();
        }

        private bool TryAddPolicyByPriority(IPolicyValues values, int priorityIndex)
        {
            if (int.TryParse(values[priorityIndex], out int priority) is false)
            {
                return false;
            }

            bool LastLessOrEqualPriority(IPolicyValues v)
            {
                return int.Parse(v[priorityIndex]) <= priority;
            }

            int lastIndex = _policy.FindLastIndex(LastLessOrEqualPriority);
            _policy.Insert(lastIndex + 1, values);
            PolicyStringSet.Add(values.ToText());
            return true;
        }

        private bool TryGetPriorityIndex(out int index)
        {
            if (Tokens is null)
            {
                index = -1;
                return false;
            }

            return Tokens.TryGetValue("priority", out index);
        }

        internal bool TrySortPoliciesByPriority()
        {
            if (TryGetPriorityIndex(out int priorityIndex) is false)
            {
                return false;
            }

            int PolicyComparison(IPolicyValues p1, IPolicyValues p2)
            {
                string priorityString1 = p1[priorityIndex];
                string priorityString2 = p2[priorityIndex];

                if (int.TryParse(priorityString1, out int priority1) is false
                    || int.TryParse(priorityString2, out int priority2) is false)
                {
                    return string.CompareOrdinal(priorityString1, priorityString2);
                }

                return priority1 - priority2;
            }

            _policy.Sort(PolicyComparison);
            return true;
        }

        private bool TryGetSubjectHierarchyDomainIndex(out int index)
        {
            if (Tokens is null)
            {
                index = -1;
                return false;
            }

            return Tokens.TryGetValue("dom", out index);
        }

        private bool TryGetSubjectHierarchySubjectIndex(out int index)
        {
            if (Tokens is null)
            {
                index = -1;
                return false;
            }

            return Tokens.TryGetValue("sub", out index);
        }


        internal bool TrySortPoliciesBySubjectHierarchy(Dictionary<string, int> subjectHierarchyMap,
            Func<string, string, string> nameFormatter)
        {
            if (TryGetSubjectHierarchyDomainIndex(out int domainIndex) is false)
            {
                domainIndex = -1;
            }

            if (TryGetSubjectHierarchySubjectIndex(out int subjectIndex) is false)
            {
                return false;
            }


            int PolicyComparison(IPolicyValues p1, IPolicyValues p2)
            {
                string domain1 = "", domain2 = "";
                if (domainIndex != -1)
                {
                    domain1 = p1[domainIndex];
                    domain2 = p2[domainIndex];
                }

                string name1 = nameFormatter(domain1, p1[subjectIndex]);
                string name2 = nameFormatter(domain2, p2[subjectIndex]);

                return subjectHierarchyMap[name1] - subjectHierarchyMap[name2];
            }

            _policy.Sort(PolicyComparison);
            return true;
        }
    }
}
