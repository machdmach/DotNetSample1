using System.Net;
using System.Net.Sockets;

namespace Libx
{
    public class IPAddressRange
    {
        private readonly AddressFamily addressFamily;
        private readonly byte[] lowerBytes;
        private readonly byte[] upperBytes;

        //===================================================================================================
        public IPAddressRange(IPAddress lower, IPAddress upper)
        {
            // Assert that lower.AddressFamily == upper.AddressFamily

            addressFamily = lower.AddressFamily;
            lowerBytes = lower.GetAddressBytes();
            upperBytes = upper.GetAddressBytes();
        }
        //===================================================================================================
        public bool IsInRange(IPAddress address)
        {
            if (address.AddressFamily != addressFamily)
            {
                return false;
            }

            byte[] addressBytes = address.GetAddressBytes();

            bool lowerBoundary = true, upperBoundary = true;

            for (int i = 0; i < lowerBytes.Length &&
                (lowerBoundary || upperBoundary); i++)
            {
                if ((lowerBoundary && addressBytes[i] < lowerBytes[i]) ||
                    (upperBoundary && addressBytes[i] > upperBytes[i]))
                {
                    return false;
                }

                lowerBoundary &= (addressBytes[i] == lowerBytes[i]);
                upperBoundary &= (addressBytes[i] == upperBytes[i]);
            }

            return true;
        }
        //===================================================================================================
        public static void Test1()
        {
            string ipString = "1.1.2.";// System.Web.HttpContext.Current.Request.UserHostAddress;
            byte[] ipBytes = System.Net.IPAddress.Parse(ipString).GetAddressBytes();
            int ip = System.BitConverter.ToInt32(ipBytes, 0);

            // your network ip range
            string ipStringFrom = "192.168.1.0";
            byte[] ipBytesFrom = System.Net.IPAddress.Parse(ipStringFrom).GetAddressBytes();
            int ipFrom = System.BitConverter.ToInt32(ipBytesFrom, 0);

            string ipStringTo = "192.168.1.255";
            byte[] ipBytesTo = System.Net.IPAddress.Parse(ipStringTo).GetAddressBytes();
            int ipTo = System.BitConverter.ToInt32(ipBytesFrom, 0);

            bool clientIsOnLAN = ipFrom >= ip && ip <= ipTo;
        }
    }
}