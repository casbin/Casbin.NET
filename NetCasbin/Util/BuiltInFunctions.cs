using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using NetCasbin.Rabc;

namespace NetCasbin
{
    public static class BuiltInFunctions
    {
        /// <summary>
        /// key1是否匹配key2（类似RESTful路径），key2能包含*
        /// 例如："/foo/bar"匹配"/foo/*"
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public static Boolean KeyMatch(string key1, string key2)
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
        ///  ipMatch determines whether IP address ip1 matches the pattern of IP address        
        ///  ip2, ip2 can be an IP address or a CIDR pattern. For example, "192.168.2.123"
        ///  matches "192.168.2.0/24"
        /// </summary>
        /// <param name="ip1"> the first argument.</param>
        /// <param name="ip2"> the second argument.</param>
        /// <returns>whether ip1 matches ip2.</returns>
        public static bool IPMatch(string ip1, string ip2)
        {
            string rgxString = @"^((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))?\/?\d{0,2}(?<!33)$";
            Regex rgx = new Regex(rgxString);
            if (!rgx.IsMatch(ip1))
            {
                throw new Exception("invalid argument: ip1 in IPMatch() function is not an IP address.");
            }
            if (!rgx.IsMatch(ip2))
            {
                throw new Exception("invalid argument: ip2 in IPMatch() function is not an IP address.");
            }

            var ip1Splits = ip1.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            IPAddress address1 = IPAddress.Parse(ip1Splits[0]);

            var ip2Splits = ip2.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            IPAddress address2 = IPAddress.Parse(ip2Splits[0]);
            if (ip2Splits.Length == 2)
            {
                var maskLength = int.Parse(ip2Splits[1]);
                var mask = IPAddressExtenstions.GetNetworkMask(maskLength);
                address1 = address1.Mask(mask);
            }
            if (address1.Equals(address2))
            {
                return true;
            }
            return false;
        }

        public static bool KeyMatch2(string key1, string key2)
        {
            key2 = key2.Replace("/*", "/.*");

            Regex regex = new Regex("(.*):[^/]+(.*)");

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

        public static bool KeyMatch3(string key1, string key2)
        {
            key2 = key2.Replace("/*", "/.*");

            Regex regex = new Regex("(.*)\\{[^/]+\\}(.*)");
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


        public static Boolean RegexMatch(String key1, String key2)
        {
            return Regex.Match(key1, key2).Success;
        }

        delegate bool GCall(string arg1, string arg2, string domain = null);
        internal static AbstractFunction GenerateGFunction(string name, IRoleManager rm)
        {
            bool Call(string arg1, string arg2, string domain = null)
            {
                if (rm == null)
                {
                    return arg1.Equals(arg2);
                }
                else
                {
                    bool res;
                    if (!String.IsNullOrEmpty(domain))
                    {
                        res = rm.HasLink(arg1, arg2, domain);
                        return res;
                    }

                    res = rm.HasLink(arg1, arg2);
                    return res;
                }
            }
            GCall call = Call;
            return new AviatorFunction(name, call);
        }
    }
}
