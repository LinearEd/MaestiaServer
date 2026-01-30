using System;
using System.Runtime.InteropServices;

namespace DevServer.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public class DPKUL_CHAR_RQ_UNKNOWN_182 : Packet
    {
        public DPKUL_CHAR_RQ_UNKNOWN_182()
        {
            Length = Convert.ToUInt16(Marshal.SizeOf(this));

            Type = PacketType.DPKUL_CHAR_RQ_UNKNOWN_182;
        }

        // Unknown packet structure - add fields as needed
    }
}
