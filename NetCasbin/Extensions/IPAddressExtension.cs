using System;
using System.Net;
using System.Net.Sockets;

namespace Casbin.Extensions
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
                byte cidrByte = (byte)(nowCidrMaskCount == 0 ? 0x00 : 0xFF);

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
    }
}
