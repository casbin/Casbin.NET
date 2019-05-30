using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCasbin.Persist
{
    public class Helper
    {

        public delegate void LoadPolicyLineHandler<T, U>(T t, U u);

        public static void LoadPolicyLine(String line, Model model)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            if (line[0] == '#')
            {
                return;
            }

            string[] tokens = line.Split(',').Select(x => x.Trim()).ToArray();

            string key = tokens[0];
            string sec = key.Substring(0, 1);

            if (model.Model.ContainsKey(sec))
            {
                var item = model.Model[sec];
                var policy = item[key];
                if (policy == null)
                {
                    return;
                }
                 
                policy.Policy.Add(tokens.Skip(1).ToList());
            }
        }
    }
}
