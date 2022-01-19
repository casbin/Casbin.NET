using System;
using System.Collections.Generic;
using System.Linq;
using NetCasbin.Model;

namespace NetCasbin.Persist
{
    public static class Helper
    {
        [Obsolete("This item will remove at the next mainline version")]
        public delegate void LoadPolicyLineHandler<T, TU>(T t, TU u);

        [Obsolete("please use the extension method TryLoadPolicyLine of Model")]
        public static void LoadPolicyLine(string line, Model.Model model)
        {
            model.TryLoadPolicyLine(line);
        }

        public static bool TryLoadPolicyLine(this Model.Model model, string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return false;
            }

            if (line[0] == '#')
            {
                return false;
            }

            string[] tokens = line.Split(',').Select(x => x.Trim()).ToArray();
            return model.TryLoadPolicyLine(tokens);
        }

        public static bool TryLoadPolicyLine(this Model.Model model, IReadOnlyList<string> lineTokens)
        {
            string type = lineTokens[0];
            string sec = type.Substring(0, 1);
            return model.TryGetExistAssertion(sec, type, out Assertion assertion)
                   && assertion.TryAddPolicy(lineTokens.Skip(1).ToList());
        }
    }
}
