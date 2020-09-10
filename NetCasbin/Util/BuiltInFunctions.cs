using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using NetCasbin.Abstractions;
using NetCasbin.Rbac;

namespace NetCasbin.Util
{
    public static class BuiltInFunctions
    {
        private static readonly Regex s_keyMatch2Regex = new Regex(@":[^/]+");
        private static readonly Regex s_keyMatch3Regex = new Regex(@"\{[^/]+\}");
        private static readonly Regex s_keyMatch4Regex = new Regex(@"\{([^/]+)\}");

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
            if (IPAddress.TryParse(ip1, out var ip1Address) is false)
            {
                throw new ArgumentException($"The argument {nameof(ip1)} is not an IP address.");
            }

            var ip2Span = ip2.AsSpan();
            int index = ip2Span.IndexOf('/');
            IPAddress ip2Address;
            if (index < 0)
            {
                if (IPAddress.TryParse(ip2, out ip2Address) is false)
                {
                    throw new ArgumentException($"The argument {nameof(ip2)} is not an IP address.");
                }

                return ip1Address.Equals(ip2Address);
            }

            if(IPAddress.TryParse(ip2Span.Slice(0, index).ToString(), out ip2Address) is false)
            {
                throw new ArgumentException($"The argument {nameof(ip2)} is not an IP address.");
            }

            if (ip1Address.AddressFamily != ip2Address.AddressFamily)
            {
                return false;
            }

            if (int.TryParse(ip2Span.Slice(index + 1).ToString(), out int cidrMask) is false)
            {
                throw new ArgumentException($"The argument {nameof(ip2)} has invalid CIDR mask.");
            }

            const int ipv4Length = 4;
            const int ipv6Length = 16;
            // If IPv4 and cidrMask = 24, (ipv4Length * 8 - cidrMask) = 8,
            // -1 = 0xFFFFFFFF, (-1 << (ipv4Length * 8 - cidrMask)) = 0xFFFFFF00
            // IPAddress.NetworkToHostOrder(-1 << (ipv4Length * 8 - cidrMask)) = 0x00FFFFFF
            long cidrMaskNetWorkOrder = ip1Address.AddressFamily switch
            {
                AddressFamily.InterNetwork => IPAddress.NetworkToHostOrder(-1 << (ipv4Length * 8 - cidrMask)),
                AddressFamily.InterNetworkV6 => IPAddress.NetworkToHostOrder((long) -1 << (ipv6Length * 8 - cidrMask)),
                _ => throw new NotSupportedException("Unable support other address family.")
            };

            return (ip1Address.Address & cidrMaskNetWorkOrder) == (ip2Address.Address & cidrMaskNetWorkOrder);
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
