﻿using System.Collections.Generic;
using System.Linq;
using Casbin.Model;

namespace Casbin.Persist;

public static class PolicyStoreExtension
{
    public static bool TryLoadPolicyLine(this IPolicyStore store, string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return false;
        }

        if (line.StartsWith("/") || line.StartsWith("#"))
        {
            return false;
        }

        string[] tokens = line.Split(',').Select(x => x.Trim()).ToArray();
        return store.TryLoadPolicyLine(tokens);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static bool TryLoadPolicyLine(this IPolicyStore store, IReadOnlyList<string> lineTokens)
    {
        string type = lineTokens[0];
        string section = type.Substring(0, 1);
        IPolicyValues values = Policy.ValuesFrom(lineTokens.Skip(1).ToList());
        return store.AddPolicy(section, type, values);
    }
}
