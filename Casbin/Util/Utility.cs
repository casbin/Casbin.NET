using System.Collections.Generic;

namespace Casbin.Util
{
    internal static class Utility
    {
        internal static string RuleToString(IEnumerable<string> rule)
        {
            return string.Join(PermConstants.PolicySeparatorString, rule);
        }

        /// <summary>
        /// SetEquals determines whether two string sets are identical.
        /// </summary>
        /// <param name="a">The first set.</param>
        /// <param name="b">The second set.</param>
        /// <returns>Whether a equals to b.</returns>
        internal static bool SetEquals(List<string> a, List<string> b)
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
    }
}
