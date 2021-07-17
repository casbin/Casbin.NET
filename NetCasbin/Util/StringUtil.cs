using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Casbin.Util
{
    internal static class StringUtil
    {
        private static readonly Regex s_escapeRegex = new(@"(\|| |=|\)|\(|&|<|>|,|\+|-|!|\*|\/)((r|p)[0-9]*)\.");
        private static readonly Regex s_evalRegex = new(@"\beval\((?<rule>[^)]*)\)");

        /// <summary>
        /// EscapeAssertion escapes the dots in the assertion,
        /// because the expression evaluation doesn't support such variable names.
        /// </summary>
        /// <param name="s">The value of the matcher and effect assertions.</param>
        /// <returns>The escaped value.</returns>
        internal static string EscapeAssertion(string str)
        {
            if (str.StartsWith(PermConstants.DefaultRequestType, StringComparison.Ordinal)
                || str.StartsWith(PermConstants.DefaultPolicyType, StringComparison.Ordinal))
            {
                str = str.ReplaceFirst('.', '_');
            }
            str = s_escapeRegex.Replace(str, m => m.Value.ReplaceFirst('.', '_')) ;
            return str;
        }

        private static string ReplaceFirst(this string str, char oldChar, char newChar)
        {
            int position = str.IndexOf(oldChar.ToString(), StringComparison.Ordinal);
            if (position is -1)
            {
                return str;
            }
            return string.Concat(str.Substring(0, position), newChar, str.Substring(position + 1));
        }

        /// <summary>
        /// Removes the comments starting with # in the text.
        /// </summary>
        /// <param name="s">a line in the model.</param>
        /// <returns>The line without comments.</returns>
        internal static string RemoveComments(this string str)
        {
            int pos = str.IndexOf("#", StringComparison.Ordinal);
            if (pos is -1)
            {
                return str;
            }
            return str.Substring(0, pos).Trim();
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
