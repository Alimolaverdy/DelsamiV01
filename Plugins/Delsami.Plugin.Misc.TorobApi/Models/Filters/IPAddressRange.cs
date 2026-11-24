using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.TorobApi.Models.Filters
{
    public class IPAddressRange
    {
        public IPAddress Start { get; }
        public IPAddress End { get; }

        public IPAddressRange(IPAddress start, IPAddress end)
        {
            Start = start;
            End = end;
        }

        public bool Contains(IPAddress ip)
        {
            var addressBytes = ip.GetAddressBytes();
            return ByteArrayCompare(Start.GetAddressBytes(), addressBytes) <= 0 &&
                   ByteArrayCompare(End.GetAddressBytes(), addressBytes) >= 0;
        }

        private int ByteArrayCompare(byte[] a1, byte[] a2)
        {
            for (int i = 0; i < a1.Length; i++)
            {
                int comparison = a1[i].CompareTo(a2[i]);
                if (comparison != 0) return comparison;
            }
            return 0;
        }
    }
}
