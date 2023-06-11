using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Casbin.Model;
using CsvHelper;
using CsvHelper.Configuration;

namespace Casbin.Persist;

public static class PolicyStoreExtension
{
    public static bool TryLoadPolicyLine(this IPolicyStore store, string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return false;
        }

        if (line.StartsWith("#"))
        {
            return false;
        }

        CsvParser parser = new(new StringReader(line),
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                AllowComments = true,
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null
            });
        if (parser.Read() is false)
        {
            return false;
        }

        string[] tokens = parser.Record;
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
