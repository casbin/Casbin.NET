using System.Collections.Generic;
using Casbin.Model;

namespace Casbin.UnitTests.Extensions
{
    public static class RequestValuesExtension
    {
        public static IEnumerable<string> ToEnumerable(this IRequestValues values)
        {
            string[] res = new string[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                res[i] = values[i];
            }
            return res;
        }
    }
}
