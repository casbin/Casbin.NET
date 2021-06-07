using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NetCasbin.Model;

namespace NetCasbin.Util
{
    public static class Utility
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        /// <summary>
        /// Removes the comments starting with # in the text.
        /// </summary>
        /// <param name="s">a line in the model.</param>
        /// <returns>The line without comments.</returns>
        public static string RemoveComments(string s)
        {
            int pos = s.IndexOf("#", StringComparison.Ordinal);
            if (pos == -1)
            {
                return s;
            }
            return s.Substring(0, pos).Trim();
        }

        /// <summary>
        /// escapeAssertion escapes the dots in the assertion, because the expression evaluation doesn't support such variable names.
        /// </summary>
        /// <param name="s">The value of the matcher and effect assertions.</param>
        /// <returns>The escaped value.</returns>
        public static string EscapeAssertion(string s)
        {
            // 替换第一个点
            if (s.StartsWith(PermConstants.DefaultRequestType) || s.StartsWith(PermConstants.DefaultPolicyType))
            {
                s = s.ReplaceFirst(@".", "_");
            }
            const string regex = "(\\|| |=|\\)|\\(|&|<|>|,|\\+|-|!|\\*|\\/)(r|p)\\.";
            var p = new Regex(regex);
            var matches = p.Matches(s);
            var sb = new StringBuilder(s);

            for (int i = 0, j = matches.Count; i < j; i++)
            {
                var match = matches[i];
                string replace = match.Groups[0].Value.Replace(".", "_");
                if (replace.Trim().Length > 0)
                {
                    sb.Replace(match.Value, replace, match.Index, match.Length);
                }
            }
            return sb.ToString();
        }

        public static string RuleToString(IEnumerable<string> rule)
        {
            return string.Join(PermConstants.PolicySeparatorString, rule);
        }

        public static bool ArrayEquals(List<string> a, List<string> b)
        {
            if (a == null)
            {
                a = new List<string>();
            }
            if (b == null)
            {
                b = new List<string>();
            }
            if (a.Count != b.Count)
            {
                return false;
            }

            for (int i = 0; i < a.Count; i++)
            {
                if (!a[i].Equals(b[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Array2DEquals(List<List<string>> a, List<List<string>> b)
        {
            if (a == null)
            {
                a = new List<List<string>>();
            }
            if (b == null)
            {
                b = new List<List<string>>();
            }
            if (a.Count != b.Count)
            {
                return false;
            }

            for (int i = 0; i < a.Count; i++)
            {
                if (!ArrayEquals(a[i], b[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ArrayRemoveDuplicates(List<string> s)
        {
            return true;
        }

        /// <summary>
        /// SetEquals determines whether two string sets are identical.
        /// </summary>
        /// <param name="a">The first set.</param>
        /// <param name="b">The second set.</param>
        /// <returns>Whether a equals to b.</returns>
        public static bool SetEquals(List<string> a, List<string> b)
        {
            if (a == null)
            {
                a = new List<string>();
            }
            if (b == null)
            {
                b = new List<string>();
            }
            if (a.Count != b.Count)
            {
                return false;
            }

            a.Sort();
            b.Sort();

            for (int i = 0; i < a.Count; i++)
            {
                if (!a[i].Equals(b[i]))
                {
                    return false;
                }
            }
            return true;
        }

        internal static void LogPrint(string v)
        {
            throw new NotImplementedException();
        }

        private static readonly Regex s_evalRegex = new Regex(@"\beval\((?<rule>[^)]*)\)");

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
        /// <param name="rule"></param>
        /// <returns></returns>
        internal static string ReplaceEval(string expressStringWithEvalRule, string rule)
        {
            return s_evalRegex.Replace(expressStringWithEvalRule, $"({rule})");
        }

        /// <summary>
        /// Replace eval function with the value of its eval rule
        /// </summary>
        /// <param name="expressStringWithEvalRule"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        public static string ReplaceEval(string expressStringWithEvalRule, IDictionary<string, string> rules)
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
