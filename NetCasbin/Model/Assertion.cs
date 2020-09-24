using System;
using System.Collections.Generic;
using System.Linq;
using NetCasbin.Rbac;
using NetCasbin.Util;

namespace NetCasbin.Model
{
    /// <summary>
    /// Represents an expression in a section of the model.
    /// For example: r = sub, obj, act
    /// </summary>
    public class Assertion
    {
        public string Key { set; get; }

        public string Value { set; get; }

        public IDictionary<string, int> Tokens { set; get; }

        public IRoleManager RoleManager { get; private set; }

        public List<List<string>> Policy { set; get; }

        private HashSet<string> PolicyStringSet { get; }

        public Assertion()
        {
            Policy = new List<List<string>>();
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

        public void BuildRoleLinks(IRoleManager roleManager)
        {
            RoleManager = roleManager;
            int count = Value.ToCharArray().Count(x => x == '_');
            foreach (var rule in Policy)
            {
                if (count < 2)
                {
                    throw new Exception("the number of \"_\" in role definition should be at least 2");
                }
                if (rule.Count < count)
                {
                    throw new Exception("grouping policy elements do not meet role definition");
                }

                if (count == 2)
                {
                    roleManager.AddLink(rule[0], rule[1]);
                }
                else if (count == 3)
                {
                    roleManager.AddLink(rule[0], rule[1], rule[2]);
                }
                else if (count == 4)
                {
                    roleManager.AddLink(rule[0], rule[1], rule[2], rule[3]);
                }
            }
        }

        internal bool Contains(IEnumerable<string> rule)
        {
            return PolicyStringSet.Contains(Utility.RuleToString(rule));
        }

        internal bool TryAddPolicy(List<string> rule)
        {
            if (Contains(rule))
            {
                return false;
            }
            Policy.Add(rule);
            PolicyStringSet.Add(Utility.RuleToString(rule));
            return true;
        }

        internal bool TryRemovePolicy(List<string> rule)
        {
            if (!Contains(rule))
            {
                return false;
            }
            for (int i = 0; i < Policy.Count; i++)
            {
                var ruleInPolicy = Policy[i];
                if (!Utility.ArrayEquals(rule, ruleInPolicy))
                {
                    continue;
                }
                Policy.RemoveAt(i);
                PolicyStringSet.Remove(Utility.RuleToString(rule));
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
