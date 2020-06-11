using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NetCasbin.Util
{
    public static class Utility
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            var pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        /// <summary>
        /// removeComments removes the comments starting with # in the text.
        /// </summary>
        /// <param name="s">a line in the model.</param>
        /// <returns>the line without comments.</returns>
        public static string RemoveComments(string s)
        {
            var pos = s.IndexOf("#");
            if (pos == -1)
            {
                return s;
            }
            return s.Substring(0, pos).Trim();
        }

        /// <summary>
        /// escapeAssertion escapes the dots in the assertion, because the expression evaluation doesn't support such variable names.
        /// </summary>
        /// <param name="s">the value of the matcher and effect assertions.</param>
        /// <returns>the escaped value.</returns>
        public static string EscapeAssertion(string s)
        {
            // 替换第一个点
            if (s.StartsWith("r") || s.StartsWith("p"))
            {
                s = s.ReplaceFirst(@".", "_");
            }
            var regex = "(\\|| |=|\\)|\\(|&|<|>|,|\\+|-|!|\\*|\\/)(r|p)\\.";
            var p = new Regex(regex);
            var matches = p.Matches(s);
            var sb = new StringBuilder(s);

            for (int i = 0, j = matches.Count; i < j; i++)
            {
                var match = matches[i];
                var replace = match.Groups[0].Value.Replace(".", "_");
                if (replace.Trim().Length > 0)
                {
                    sb.Replace(match.Value, replace, match.Index, match.Length);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// arrayToString gets a printable string for a string array.
        /// </summary>
        /// <param name="v">the array.</param>
        /// <returns>the string joined by the array elements.</returns>
        public static string ArrayToString(string[] v)
        {
            return string.Join(", ", v);
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

            for (var i = 0; i < a.Count; i++)
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

            for (var i = 0; i < a.Count; i++)
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

        public static string ArrayToString(List<string> s)
        {
            return string.Join(", ", s);
        }

        public static string ParamsToString(string[] s)
        {
            return string.Join(", ", s);
        }

        /// <summary>
        /// SetEquals determines whether two string sets are identical.
        /// </summary>
        /// <param name="a">the first set.</param>
        /// <param name="b">the second set.</param>
        /// <returns>whether a equals to b.</returns>
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

            for (var i = 0; i < a.Count; i++)
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
    }
}
