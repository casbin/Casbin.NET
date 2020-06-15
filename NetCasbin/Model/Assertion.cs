using NetCasbin.Rbac;
using System;
using System.Collections.Generic;
using System.Linq;
using NetCasbin.Util;

namespace NetCasbin.Model
{
    /// <summary>
    /// 断言
    /// </summary>
    public class Assertion
    {
        public string Key { set; get; }

        public string Value { set; get; }

        public string[] Tokens { set; get; }

        public IRoleManager RoleManager { get; private set; }

        public List<List<string>> Policy { set; get; }

        internal HashSet<string> PolicyStringSet { get; }

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
                PolicyStringSet.Add(Utility.ArrayToString(rule));
            }
        }

        public void BuildRoleLinks(IRoleManager roleManager)
        {
            RoleManager = roleManager;
            var count =  Value.ToCharArray().Count(x => x == '_');
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
    }
}
