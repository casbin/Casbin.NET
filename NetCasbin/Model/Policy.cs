using NetCasbin.Rabc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCasbin
{
    public class Policy
    {
        public Dictionary<String, Dictionary<String, Assertion>> Model { set; get; }

        public void BuildRoleLinks(IRoleManager rm)
        {
            if (Model.ContainsKey("g"))
            {
                foreach (Assertion ast in Model["g"].Values)
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
                    ast.Policy = new List<List<String>>();
                }
            }

            if (Model.ContainsKey("g"))
            {
                foreach (var ast in Model["p"].Values)
                {
                    ast.Policy = new List<List<String>>();
                }
            }
        }

        public List<List<String>> GetPolicy(String sec, String ptype)
        {
            return Model[sec][ptype].Policy;
        }

        public List<List<String>> GetFilteredPolicy(String sec, String ptype, int fieldIndex, params string[] fieldValues)
        {
            List<List<String>> res = new List<List<string>>();

            foreach (List<String> rule in Model[sec][ptype].Policy)
            {
                Boolean matched = true;
                for (int i = 0; i < fieldValues.Length; i++)
                {
                    String fieldValue = fieldValues[i];
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

        public Boolean HasPolicy(String sec, String ptype, List<String> rule)
        {
            return Model[sec][ptype]!=null && Model[sec][ptype].Policy.Any(x => Util.ArrayEquals(rule, x));
        }

        public Boolean AddPolicy(String sec, String ptype, List<String> rule)
        {
            if (!HasPolicy(sec, ptype, rule))
            {
                Model[sec][ptype].Policy.Add(rule);
                return true;
            }
            return false;
        }

        public Boolean RemovePolicy(String sec, String ptype, List<String> rule)
        {
            for (int i = 0; i < Model[sec][ptype].Policy.Count; i++)
            {
                List<String> r = Model[sec][ptype].Policy[i];
                if (Util.ArrayEquals(rule, r))
                {
                    Model[sec][ptype].Policy.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public Boolean RemoveFilteredPolicy(String sec, String ptype, int fieldIndex, params string[] fieldValues)
        {
            List<List<String>> tmp = new List<List<string>>();
            Boolean res = false;

            foreach (List<String> rule in Model[sec][ptype].Policy)
            {
                Boolean matched = true;
                for (int i = 0; i < fieldValues.Length; i++)
                {
                    String fieldValue = fieldValues[i];
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

        public List<String> GetValuesForFieldInPolicy(String sec, String ptype, int fieldIndex)
        {
            List<String> values = new List<string>();

            foreach (List<String> rule in Model[sec][ptype].Policy)
            {
                values.Add(rule[fieldIndex]);
            }

            Util.ArrayRemoveDuplicates(values);
            return values;
        }
    }
}
