using System;
using System.Collections.Generic;
using System.Linq;
using Casbin.Rbac;
using Casbin.Util;

namespace Casbin.Model
{
    /// <summary>
    /// Represents an expression in a section of the model.
    /// For example: r = sub, obj, act
    /// </summary>
    public class Assertion : IReadOnlyAssertion
    {
        private List<IPolicyValues> _policy;

        public string Key { get; internal set; }

        public string Value { get; internal set; }

        public IReadOnlyDictionary<string, int> Tokens { get; internal set; }

        public IRoleManager RoleManager { get; internal set; }

        public IReadOnlyList<IPolicyValues> Policy
        {
            get => _policy;
            internal set => _policy = value as List<IPolicyValues> ?? value.ToList();
        }

        internal HashSet<string> PolicyStringSet { get; }

        public Assertion()
        {
            _policy = new List<IPolicyValues>();
            PolicyStringSet = new HashSet<string>();
            RoleManager = new DefaultRoleManager(10);
        }

        public void RefreshPolicyStringSet()
        {
            PolicyStringSet.Clear();
            foreach (var rule in Policy)
            {
                PolicyStringSet.Add(Utility.RuleToString(rule));
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

        public void BuildRoleLinks()
        {
            int count = Value.Count(c => c is '_');
            if (count < 2)
            {
                throw new InvalidOperationException("the number of \"_\" in role definition should be at least 2.");
            }

            foreach (IPolicyValues rule in Policy)
            {
                BuildRoleLink(count, PolicyOperation.PolicyAdd, rule);
            }
        }

        private void BuildRoleLink(int groupPolicyCount,
            PolicyOperation policyOperation, IEnumerable<string> rule)
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
                case PolicyOperation.PolicyAdd:
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
                case PolicyOperation.PolicyRemove:
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(policyOperation), policyOperation, null);
            }
        }

        internal bool Contains(IEnumerable<string> rule)
        {
            return PolicyStringSet.Contains(Utility.RuleToString(rule));
        }

        internal bool TryAddPolicy(IEnumerable<string> rule)
        {
            var ruleList = rule as IReadOnlyList<string> ?? rule.ToArray();
            if (Contains(ruleList))
            {
                return false;
            }

            if (TryGetPriorityIndex(out int index))
            {
                return TryAddPolicyByPriority(ruleList, index);
            }

            _policy.Add(Model.Policy.CreateOnlyString(ruleList));
            PolicyStringSet.Add(Utility.RuleToString(ruleList));
            return true;
        }

        internal bool TryRemovePolicy(IEnumerable<string> rule)
        {
            var ruleList = rule as IReadOnlyList<string> ?? rule.ToArray();
            if (Contains(ruleList) is false)
            {
                return false;
            }
            for (int i = 0; i < Policy.Count; i++)
            {
                var ruleInPolicy = Policy[i];
                if (ruleList.DeepEquals(ruleInPolicy) is false)
                {
                    continue;
                }
                _policy.RemoveAt(i);
                PolicyStringSet.Remove(Utility.RuleToString(ruleList));
                break;
            }
            return true;
        }

        internal void ClearPolicy()
        {
            _policy.Clear();
            PolicyStringSet.Clear();
        }

        private bool TryAddPolicyByPriority(IReadOnlyList<string> rule, int priorityIndex)
        {
            if (int.TryParse(rule[priorityIndex], out int priority) is false)
            {
                return false;
            }

            bool LastLessOrEqualPriority(IReadOnlyList<string> p)
            {
                return int.Parse(p[priorityIndex]) <= priority;
            }

            int lastIndex = _policy.FindLastIndex(LastLessOrEqualPriority);
            _policy.Insert(lastIndex + 1, Model.Policy.CreateOnlyString(rule));
            PolicyStringSet.Add(Utility.RuleToString(rule));
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


        internal bool TrySortPoliciesBySubjectHierarchy(Dictionary<string, int> subjectHierarchyMap, Func<string, string, string> nameFormatter)
        {
            if(TryGetSubjectHierarchyDomainIndex(out int domainIndex) is false)
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
                if(domainIndex != -1)
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
