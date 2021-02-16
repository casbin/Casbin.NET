using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using NetCasbin.Abstractions;
using NetCasbin.Extensions;
using NetCasbin.Rbac;

namespace NetCasbin.Util
{
    public static class BuiltInFunctions
    {
        private static readonly Regex s_keyMatch2Regex = new Regex(@":[^/]+");
        private static readonly Regex s_keyMatch3Regex = new Regex(@"\{[^/]+\}");
        private static readonly Regex s_keyMatch4Regex = new Regex(@"\{([^/]+)\}");
        private delegate bool GFunction(string subject1, string subject2, string domain = null);

        /// <summary>
        /// Determines whether key1 matches the pattern of key2 (similar to RESTful path),
        /// key2 can contain a *. For example, "/foo/bar" matches "/foo/*".
        /// </summary>
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static bool KeyMatch(string key1, string key2)
        {
            int index = key2.IndexOf('*');

            if (index is 0)
            {
                return true;
            }

            if (index < 0)
            {
                return key1.Equals(key2);
            }

            key2 = key2.Substring(0, index);

            return index < key1.Length
                ? key1.Substring(0, index).Equals(key2)
                : key1.Equals(key2);
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
            key2 = s_keyMatch2Regex.Replace(key2, "[^/]+");
            return RegexMatch(key1, $"^{key2}$");
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
            key2 = s_keyMatch3Regex.Replace(key2, "[^/]+");
            return RegexMatch(key1, $"^{key2}$");
        }

        /// <summary>
        /// Determines whether key1 matches the pattern of key2 (similar to RESTful path), key2 can contain a *.
        /// Besides what KeyMatch3 does, KeyMatch4 can also match repeated patterns:
        /// "/parent/123/child/123" matches "/parent/{id}/child/{id}"
        /// "/parent/123/child/456" does not match "/parent/{id}/child/{id}"
        /// But KeyMatch3 will match both.
        /// </summary>
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static bool KeyMatch4(string key1, string key2)
        {
            key2 = key2.Replace("/*", "/.*");

            var tokens = new List<string>();

            key2 = s_keyMatch4Regex.Replace(key2, match =>
            {
                tokens.Add(match.Value.Substring(1, match.Value.Length - 2));
                return "([^/]+)";
            });

            var valueRegex = new Regex($"^{key2}$");
            var key1Match = valueRegex.Match(key1);

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
                if (valueDictionary.ContainsKey(token) is false)
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
        /// Determines whether IP address ip1 matches the pattern of IP address ip2,
        /// ip2 can be an IP address or a CIDR pattern. For example, "192.168.2.123"
        /// matches "192.168.2.0/24"
        /// </summary>
        /// <param name="ip1">The first argument.</param>
        /// <param name="ip2">The second argument.</param>
        /// <returns>Whether ip1 matches ip2.</returns>
        public static bool IPMatch(string ip1, string ip2)
        {
            if (IPAddress.TryParse(ip1, out var ipAddress1) is false)
            {
                throw new ArgumentException($"The argument {nameof(ip1)} is not an IP address.");
            }

            var ipSpan2 = ip2.AsSpan();
            int index = ipSpan2.IndexOf('/');
            IPAddress ipAddress2;

            if (index < 0)
            {
                if (IPAddress.TryParse(ip2, out ipAddress2) is false)
                {
                    throw new ArgumentException($"The argument {nameof(ip2)} is not an IP address.");
                }

                return ipAddress1.Equals(ipAddress2);
            }

            if (IPAddress.TryParse(ipSpan2.Slice(0, index).ToString(), out ipAddress2) is false)
            {
                throw new ArgumentException($"The argument {nameof(ip2)} is not an IP address.");
            }

            if (ipAddress1.AddressFamily != ipAddress2.AddressFamily)
            {
                return false;
            }

            if (byte.TryParse(ipSpan2.Slice(index + 1).ToString(), out byte cidrMask) is false)
            {
                throw new ArgumentException($"The argument {nameof(ip2)} has invalid CIDR mask.");
            }

            return ipAddress2.Match(ipAddress1, cidrMask);
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

        /// <summary>
        /// GenerateGFunction is the factory method of the g(_, _) function.
        /// </summary>
        /// <param name="name">The name of the g(_, _) function, can be "g", "g2", ..</param>
        /// <param name="roleManager">The role manager used by the function.</param>
        /// <returns>The function.</returns>
        internal static Delegate GenerateGFunction(string name, IRoleManager roleManager)
        {
            var resultCache = new Dictionary<string, bool>();

            bool GFunction(string subject1, string subject2, string domain = null)
            {
                bool hasDomain = domain is not null;

                string cacheKey = hasDomain
                    ? string.Join(";", subject1, subject2, domain)
                    : string.Join(";", subject1, subject2);

                if (resultCache.TryGetValue(cacheKey, out bool result))
                {
                    return result;
                }

                if (roleManager == null)
                {
                    result = subject1.Equals(subject2);
                    resultCache[cacheKey] = result;
                    return result;
                }

                if (!string.IsNullOrEmpty(domain))
                {
                    result = roleManager.HasLink(subject1, subject2, domain);
                    resultCache[cacheKey] = result;
                    return result;
                }

                result = roleManager.HasLink(subject1, subject2);
                resultCache[cacheKey] = result;
                return result;
            }
            return (GFunction) GFunction;
        }
    }
}
