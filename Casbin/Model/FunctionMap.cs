using System;
using System.Collections.Generic;
using Casbin.Util;

namespace Casbin.Model
{
    internal class FunctionMap
    {
        internal IDictionary<string, Delegate> FunctionDict { get; private set; }

        private void AddFunction(string name, Delegate function)
        {
            FunctionDict.Add(name, function);
        }

        internal static FunctionMap LoadFunctionMap()
        {
            var map = new FunctionMap
            {
                FunctionDict = new Dictionary<string, Delegate>()
            };

            map.AddFunction("keyGet", BuiltInFunctions.KeyGet);
            map.AddFunction("keyGet2", BuiltInFunctions.KeyGet2);
            map.AddFunction("keyGet3", BuiltInFunctions.KeyGet3);
            map.AddFunction("keyMatch", BuiltInFunctions.KeyMatch);
            map.AddFunction("keyMatch2", BuiltInFunctions.KeyMatch2);
            map.AddFunction("keyMatch3", BuiltInFunctions.KeyMatch3);
            map.AddFunction("keyMatch4", BuiltInFunctions.KeyMatch4);
            map.AddFunction("keyMatch5", BuiltInFunctions.KeyMatch5);
            map.AddFunction("regexMatch", BuiltInFunctions.RegexMatch);
            map.AddFunction("ipMatch", BuiltInFunctions.IPMatch);
            map.AddFunction("globMatch", BuiltInFunctions.GlobMatch);
            return map;
        }
    }
}
