using System;
using System.Collections.Generic;

namespace NetCasbin.Util
{
    public static class FunctionUtility
    {
        public static string GetStringValue(Dictionary<string, object> env, string argName)
        {
            //var indexAccessMethod = typeof(Dictionary<string, object>).GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);

            string result = null;
            if (env.ContainsKey(argName))
            {
                var value = env[argName];
                result = value.ToString();
            }
            return result;
        }
    }
}
