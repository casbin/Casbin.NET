using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Casbin.Util
{
    internal static class StringUtil
    {
        private static readonly Regex s_evalRegex = new(@"\beval\((?<rule>[^)]*)\)");

        /// <summary>
        /// Removes the comments starting with # in the text.
        /// </summary>
        /// <param name="line">a line in the model.</param>
        /// <returns>The line without comments.</returns>
        internal static string RemoveComments(this string line)
        {
            int pos = line.IndexOf("#", StringComparison.Ordinal);
            if (pos is -1)
            {
                return line;
            }
            return line.Substring(0, pos).Trim();
        }

        /// <summary>
        /// Determines whether matcher contains eval function
        /// </summary>
        /// <param name="expressString"></param>
        /// <returns></returns>
        internal static bool HasEval(string expressString)
        {
            return s_evalRegex.IsMatch(expressString);
        }

        /// <summary>
        /// Tries get rule names of eval function
        /// </summary>
        /// <param name="expressString"></param>
        /// <param name="evalRuleNames"></param>
        /// <returns></returns>
        internal static bool TryGetEvalRuleNames(string expressString, out IEnumerable<string> evalRuleNames)
        {
            MatchCollection matches = s_evalRegex.Matches(expressString);
            int matchCount = matches.Count;
            if (matchCount is 0)
            {
                evalRuleNames = null;
                return false;
            }
            string[] rules = new string[matchCount];
            for (int i = 0; i < matchCount; i++)
            {
                GroupCollection group = matches[i].Groups;
                if (group.Count < 2)
                {
                    evalRuleNames = null;
                    return false;
                }
                rules[i] = group[1].Value;
            }
            evalRuleNames = rules;
            return true;
        }

        /// <summary>
        /// Replace eval function with the value of its eval rule
        /// </summary>
        /// <param name="expressStringWithEvalRule"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        internal static string ReplaceEval(string expressStringWithEvalRule, IDictionary<string, string> rules)
        {
            if (rules is null)
            {
                return expressStringWithEvalRule;
            }

            return s_evalRegex.Replace(expressStringWithEvalRule, match =>
            {
                GroupCollection matchGroups = match.Groups;
                int subMatchCount = matchGroups.Count - 1;
                if (subMatchCount is 0 || rules.TryGetValue(
                    matchGroups[1].Value, out string rule) is false)
                {
                    return match.Value;
                }
                return rule;
            });
        }
    }
}
