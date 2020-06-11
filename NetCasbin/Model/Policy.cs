using NetCasbin.Rbac;
using NetCasbin.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCasbin.Model
{
    public class Policy
    {
        public Dictionary<string, Dictionary<string, Assertion>> Model { set; get; }

        public void BuildRoleLinks(IRoleManager rm)
        {
            if (Model.ContainsKey("g"))
            {
                foreach (var ast in Model["g"].Values)
                {
                    ast.BuildRoleLinks(rm);
                }
            }
        }

        public void ClearPolicy()
        {
            if (Model.ContainsKey("p"))
            {
                foreach (var ast in Model["p"].Values)
                {
                    ast.Policy = new List<List<string>>();
                }
            }

            if (Model.ContainsKey("g"))
            {
                foreach (var ast in Model["p"].Values)
                {
                    ast.Policy = new List<List<string>>();
                }
            }
        }

        public List<List<string>> GetPolicy(string sec, string ptype)
        {
            return Model[sec][ptype].Policy;
        }

        public List<List<string>> GetFilteredPolicy(string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            var res = new List<List<string>>();

            foreach (var rule in Model[sec][ptype].Policy)
            {
                var matched = true;
                for (var i = 0; i < fieldValues.Length; i++)
                {
                    var fieldValue = fieldValues[i];
                    if (!string.IsNullOrEmpty(fieldValue) && !rule[fieldIndex + i].Equals(fieldValue))
                    {
                        matched = false;
                        break;
                    }
                }

                if (matched)
                {
                    res.Add(rule);
                }
            }

            return res;
        }

        public bool HasPolicy(string sec, string ptype, List<string> rule)
        {
            return Model[sec][ptype]!=null && Model[sec][ptype].Policy.Any(x => Utility.ArrayEquals(rule, x));
        }

        public bool AddPolicy(string sec, string ptype, List<string> rule)
        {
            if (!HasPolicy(sec, ptype, rule))
            {
                Model[sec][ptype].Policy.Add(rule);
                return true;
            }
            return false;
        }

        public bool RemovePolicy(string sec, string ptype, List<string> rule)
        {
            for (var i = 0; i < Model[sec][ptype].Policy.Count; i++)
            {
                var r = Model[sec][ptype].Policy[i];
                if (Utility.ArrayEquals(rule, r))
                {
                    Model[sec][ptype].Policy.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveFilteredPolicy(string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            var tmp = new List<List<string>>();
            var res = false;

            foreach (var rule in Model[sec][ptype].Policy)
            {
                var matched = true;
                for (var i = 0; i < fieldValues.Length; i++)
                {
                    var fieldValue = fieldValues[i];
                    if (!string.IsNullOrEmpty(fieldValue) && !rule[fieldIndex + i].Equals(fieldValue))
                    {
                        matched = false;
                        break;
                    }
                }

                if (matched)
                {
                    res = true;
                }
                else
                {
                    tmp.Add(rule);
                }
            }

            Model[sec][ptype].Policy = tmp;
            return res;
        }

        public List<string> GetValuesForFieldInPolicy(string sec, string ptype, int fieldIndex)
        {
            var values = new List<string>();

            foreach (var rule in Model[sec][ptype].Policy)
            {
                values.Add(rule[fieldIndex]);
            }

            Utility.ArrayRemoveDuplicates(values);
            return values;
        }
    }
}
