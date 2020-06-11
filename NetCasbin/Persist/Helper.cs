using System;
using System.Linq;

namespace NetCasbin.Persist
{
    public class Helper
    {
        public delegate void LoadPolicyLineHandler<T, U>(T t, U u);

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

            var key = tokens[0];
            var sec = key.Substring(0, 1);

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
