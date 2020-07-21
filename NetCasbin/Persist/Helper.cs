using System;
using System.Linq;

namespace NetCasbin.Persist
{
    public class Helper
    {
        public delegate void LoadPolicyLineHandler<T, TU>(T t, TU u);

        public static void LoadPolicyLine(string line, Model.Model model)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            if (line[0] == '#')
            {
                return;
            }

            var tokens = line.Split(',').Select(x => x.Trim()).ToArray();

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

                var content = tokens.Skip(1).ToList();
                if (!model.HasPolicy(sec, key, content))
                {
                    policy.Policy.Add(content);
                }
            }
        }


    }
}
