using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using NetCasbin.Abstractions;
using NetCasbin.Rbac;

namespace NetCasbin.Util
{
    public static class BuiltInFunctions
    {
        private static Regex s_keyMatch4Regex = new Regex(@"\{([^/]+)\}");

        /// <summary>
        /// Determines whether key1 matches the pattern of key2 (similar to 
        /// RESTful path), key2 can contain a *. For example, "/foo/bar" matches "/foo/*".
        /// </summary>
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static bool KeyMatch(string key1, string key2)
        {
            int i = key2.IndexOf('*');

            if (i == -1)
            {
                return key1.Equals(key2);
            }

            if (key1.Length > i)
            {
                return key1.Substring(0, i).Equals(key2.Substring(0, i));
            }
            return key1.Equals(key2.Substring(0, i));
        }

        /// <summary>
        /// Determines whether key1 matches the pattern of key2 (similar to
        /// RESTful path), key2 can contain a*. For example, "/foo/bar" matches
        /// "/foo/*", "/resource1" matches "/:resource"
        /// </summary>
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static bool KeyMatch2(string key1, string key2)
        {
            key2 = key2.Replace("/*", "/.*");

            var regex = new Regex("(.*):[^/]+(.*)");

            while (true)
            {
                if (!key2.Contains("/:"))
                {
                    break;
                }

                key2 = regex.Replace(key2, "$1[^/]+$2");
            }
            return RegexMatch(key1, key2);
        }

        /// <summary>
        /// Determines whether key1 matches the pattern of key2 (similar to
        /// RESTful path), key2 can contain a *. For example, "/foo/bar" matches
        ///  "/foo/*", "/resource1" matches "/{resource}"
        /// </summary>
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static bool KeyMatch3(string key1, string key2)
        {
            key2 = key2.Replace("/*", "/.*");

            var regex = new Regex("(.*)\\{[^/]+\\}(.*)");
            while (true)
            {
                if (!key2.Contains("/{"))
                {
                    break;
                }

                key2 = regex.Replace(key2, "$1[^/]+$2");
            }
            return RegexMatch(key1, key2);
        }

        /// <summary>
        /// Determines whether key1 matches the pattern of key2 (similar to RESTful path), key2 can contain a *.
        /// Besides what KeyMatch3 does, KeyMatch4 can also match repeated patterns:
        /// "/parent/123/child/123" matches "/parent/{id}/child/{id}"
        /// "/parent/123/child/456" does not match "/parent/{id}/child/{id}"
        /// </summary>
        /// But KeyMatch3 will match both.
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static bool KeyMatch4(string key1, string key2)
        {
            key2 = key2.Replace("/*", "/.*");
            var key2Match = s_keyMatch4Regex.Match(key2);

            var tokens = new List<string>();

            while (key2Match.Success)
            {
                key2 = s_keyMatch4Regex.Replace(key2, "([^/]+)");
                tokens.Add(key2Match.Value.Substring(1, key2Match.Value.Length - 2));
                key2Match = key2Match.NextMatch();
            }

            var valueRegex = new Regex($"^{key2}$");
            var key1Match =  valueRegex.Match(key1);

            if (!key1Match.Success)
            {
                return false;
            }

            int key1GroupMatchCount = key1Match.Groups.Count - 1;
            if (key1GroupMatchCount != tokens.Count)
            {
                throw new ArgumentException("KeyMatch4: number of tokens is not equal to number of values");
            }

            var group = key1Match.Groups;
            var valueDictionary = new Dictionary<string, string>();

            for (int i = 0; i < key1GroupMatchCount; i++)
            {
                string token = tokens[i];
                if (!valueDictionary.ContainsKey(token))
                {
                    valueDictionary.Add(token, group[i + 1].Value);
                    continue;
                }
                if (valueDictionary[token] != group[i + 1].Value)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///  Determines whether IP address ip1 matches the pattern of IP address
        ///  ip2, ip2 can be an IP address or a CIDR pattern. For example, "192.168.2.123"
        ///  matches "192.168.2.0/24"
        /// </summary>
        /// <param name="ip1">The first argument.</param>
        /// <param name="ip2">The second argument.</param>
        /// <returns>Whether ip1 matches ip2.</returns>
        public static bool IpMatch(string ip1, string ip2)
        {
            string rgxString = @"^((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))?\/?\d{0,2}(?<!33)$";
            var rgx = new Regex(rgxString);
            if (!rgx.IsMatch(ip1))
            {
                throw new Exception("invalid argument: ip1 in IPMatch() function is not an IP address.");
            }
            if (!rgx.IsMatch(ip2))
            {
                throw new Exception("invalid argument: ip2 in IPMatch() function is not an IP address.");
            }

            var ip1Splits = ip1.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            var address1 = IPAddress.Parse(ip1Splits[0]);

            var ip2Splits = ip2.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            var address2 = IPAddress.Parse(ip2Splits[0]);
            if (ip2Splits.Length == 2)
            {
                int maskLength = int.Parse(ip2Splits[1]);
                var mask = IpAddressExtensions.GetNetworkMask(maskLength);
                address1 = address1.Mask(mask);
            }
            if (address1.Equals(address2))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether key1 matches the pattern of key2 in regular
        /// expression.
        /// </summary>
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static bool RegexMatch(string key1, string key2)
        {
            return Regex.Match(key1, key2).Success;
        }

        delegate bool GCall(string arg1, string arg2, string domain = null);

        /// <summary>
        /// GenerateGFunction is the factory method of the g(_, _) function.
        /// </summary>
        /// <param name="name">The name of the g(_, _) function, can be "g", "g2", ..</param>
        /// <param name="rm">The role manager used by the function.</param>
        /// <returns>The function.</returns>
        internal static AbstractFunction GenerateGFunction(string name, IRoleManager rm)
        {
            bool Call(string arg1, string arg2, string domain = null)
            {
                if (rm == null)
                {
                    return arg1.Equals(arg2);
                }

                bool res;
                if (!string.IsNullOrEmpty(domain))
                {
                    res = rm.HasLink(arg1, arg2, domain);
                    return res;
                }

                res = rm.HasLink(arg1, arg2);
                return res;
            }
            GCall call = Call;
            return new AviatorFunction(name, call);
        }
    }
}
