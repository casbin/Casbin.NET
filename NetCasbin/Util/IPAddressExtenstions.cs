using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NetCasbin
{
    public static class IPAddressExtenstions
    {
        /// <summary>
        /// Get Mask IPAddress
        /// </summary>
        /// <param name="networkPrefixLength"></param>
        /// <returns></returns>
        public static IPAddress GetNetworkMask(int networkPrefixLength)
        {
            byte[] bytes = new byte[4];
            for (var i = 0; i < networkPrefixLength; i++)
            {
                var index = i / 8;
                var mod = i % 8;
                if (mod == 0)
                {
                    bytes[index] = 1;
                }
                else
                {
                    bytes[index] += Convert.ToByte(1 << mod);
                }
            }
            var mask = new IPAddress(bytes);
            return mask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static IPAddress Mask(this IPAddress address, IPAddress mask)
        {
            var addressBytes = address.GetAddressBytes();
            var maskBytes = mask.GetAddressBytes();
            for (var i = 0; i < 4; i++)
            {
                addressBytes[i] = Convert.ToByte(addressBytes[i] & maskBytes[i]);
            }
            return new IPAddress(addressBytes);
        }
    }
}
