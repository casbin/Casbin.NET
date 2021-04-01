using System;
using System.Collections.Generic;
using System.Linq;
using Casbin.Rbac;
using Casbin.Util;
using Casbin.Extensions;

namespace Casbin.Model
{
    /// <summary>
    /// Represents an expression in a section of the model.
    /// For example: r = sub, obj, act
    /// </summary>
    public class Assertion
    {
        public string Key { get; internal set; }

        public string Value { get; internal set;  }

        public IDictionary<string, int> Tokens { get; internal set;  }

        public IRoleManager RoleManager { get; private set; }

        public List<IReadOnlyList<string>> Policy { get; internal set; }

        internal HashSet<string> PolicyStringSet { get; }

        public Assertion()
        {
            Policy = new List<IReadOnlyList<string>>();
            PolicyStringSet = new HashSet<string>();
            RoleManager = new DefaultRoleManager(0);
        }

        public void RefreshPolicyStringSet()
        {
            PolicyStringSet.Clear();
            foreach (var rule in Policy)
            {
                PolicyStringSet.Add(Utility.RuleToString(rule));
            }
        }

        internal void BuildIncrementalRoleLink(IRoleManager roleManager,
            PolicyOperation policyOperation, IEnumerable<string> rule)
        {
            RoleManager = roleManager;
            int count = Value.Count(c => c is '_');
            if (count < 2)
            {
                throw new InvalidOperationException("the number of \"_\" in role definition should be at least 2.");
            }

            BuildRoleLink(roleManager, count, policyOperation, rule);
        }

        internal void BuildIncrementalRoleLinks(IRoleManager roleManager,
            PolicyOperation policyOperation, IEnumerable<IEnumerable<string>> rules)
        {
            RoleManager = roleManager;
            int count = Value.Count(c => c is '_');
            if (count < 2)
            {
                throw new InvalidOperationException("the number of \"_\" in role definition should be at least 2.");
            }

            foreach (var rule in rules)
            {
                BuildRoleLink(roleManager, count, policyOperation, rule);
            }
        }

        public void BuildRoleLinks(IRoleManager roleManager)
        {
            RoleManager = roleManager;
            int count = Value.Count(c => c is '_');
            if (count < 2)
            {
                throw new InvalidOperationException("the number of \"_\" in role definition should be at least 2.");
            }

            foreach (IEnumerable<string> rule in Policy)
            {
                BuildRoleLink(roleManager, count, PolicyOperation.PolicyAdd, rule);
            }
        }

        private static void BuildRoleLink(IRoleManager roleManager, int groupPolicyCount,
            PolicyOperation policyOperation, IEnumerable<string> rule)
        {
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
                        case 4:
                            roleManager.AddLink(ruleEnum[0], ruleEnum[1],
                                ruleEnum[2], ruleEnum[3]);
                            break;
                        default:
                            roleManager.AddLink(ruleEnum[0], ruleEnum[1],
                                ruleEnum.GetRange(2, groupPolicyCount - 2).ToArray());
                            break;
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
                        case 4:
                            roleManager.DeleteLink(ruleEnum[0], ruleEnum[1],
                                ruleEnum[2], ruleEnum[3]);
                            break;
                        default:
                            roleManager.DeleteLink(ruleEnum[0], ruleEnum[1],
                                ruleEnum.GetRange(2, groupPolicyCount - 2).ToArray());
                            break;
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
            Policy.Add(ruleList);
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
                Policy.RemoveAt(i);
                PolicyStringSet.Remove(Utility.RuleToString(ruleList));
                break;
            }
            return true;
        }

        internal void ClearPolicy()
        {
            Policy.Clear();
            PolicyStringSet.Clear();
        }
    }
}
