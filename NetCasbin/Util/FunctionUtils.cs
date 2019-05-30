using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace NetCasbin
{
    public static class FunctionUtils
    {
        public static String GetStringValue(Dictionary<string, object> env, string argName)
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
