using System;
using System.Net;
using System.Net.Sockets;

namespace NetCasbin.Extensions
{
    public static class IPAddressExtension
    {
        public static bool Match(this IPAddress matchIpAddress, IPAddress ipAddress, byte matchCidr)
        {
            if (matchIpAddress.AddressFamily != ipAddress.AddressFamily)
            {
                return false;
            }

            byte bytesLength = matchIpAddress.AddressFamily switch
            {
                AddressFamily.InterNetwork => 4, //IPv4 Length
                AddressFamily.InterNetworkV6 => 16, //IPv6 Length
                _ => throw new NotSupportedException("Unable support other address family."),
            };

            var ipBytes1 = matchIpAddress.GetAddressBytes().AsSpan();
            var ipBytes2 = ipAddress.GetAddressBytes().AsSpan();
            byte nowCidrMaskCount = Convert.ToByte(matchCidr / 8);

            for (int i = 0; i < bytesLength; i++)
            {
                byte cidrByte = (byte) (nowCidrMaskCount == 0 ? 0x00 : 0xFF);

                if ((ipBytes1[i] & cidrByte) != (ipBytes2[i] & cidrByte))
                {
                    return false;
                }

                if (nowCidrMaskCount is not 0)
                {
                    nowCidrMaskCount--;
                }
            }

            return true;
        }


        /// <summary>
        /// Gets mask IP address
        /// </summary>
        /// <param name="networkPrefixLength"></param>
        /// <returns></returns>
        public static IPAddress GetNetworkMask(int networkPrefixLength)
        {
            var bytes = new byte[4];
            for (int i = 0; i < networkPrefixLength; i++)
            {
                int index = i / 8;
                int mod = i % 8;
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
            for (int i = 0; i < 4; i++)
            {
                addressBytes[i] = Convert.ToByte(addressBytes[i] & maskBytes[i]);
            }
            return new IPAddress(addressBytes);
        }
    }
}
