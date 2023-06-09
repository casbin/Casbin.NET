using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper.Configuration;
using CsvHelper;
using NetCasbin.Model;

namespace NetCasbin.Persist
{
    public static class Helper
    {
        [Obsolete("This item will remove at the next mainline version")]
        public delegate void LoadPolicyLineHandler<T, TU>(T t, TU u);

        [Obsolete("please use the extension method TryLoadPolicyLine of Model")]
        public static void LoadPolicyLine(string line, Model.Model model)
        {
            model.TryLoadPolicyLine(line);
        }

        public static bool TryLoadPolicyLine(this Model.Model model, string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return false;
            }

            if (line[0] == '#')
            {
                return false;
            }

            CsvParser parser = new(new StringReader(line), new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                BadDataFound = null
            });
            while (parser.Read())
            {
                string[] tokens = parser.Record;
                return model.TryLoadPolicyLine(tokens);
            };
            return false;
        }

        public static bool TryLoadPolicyLine(this Model.Model model, IReadOnlyList<string> lineTokens)
        {
            string type = lineTokens[0];
            string sec = type.Substring(0, 1);
            if (model.TryGetExistAssertion(sec, type, out Assertion assertion) is false)
            {
                return false;
            }

            var tokens = lineTokens.Skip(1).ToList();
            if (assertion.Tokens is not null && assertion.Tokens.Count != tokens.Count)
            {
                return false;
            }
            return assertion.TryAddPolicy(tokens);
        }
    }
}
