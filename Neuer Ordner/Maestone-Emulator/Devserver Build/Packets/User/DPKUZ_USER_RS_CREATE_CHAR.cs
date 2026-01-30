using System;
using System.Runtime.InteropServices;

namespace DevServer.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public class DPKUZ_USER_RS_CREATE_CHAR : Packet
    {
        public DPKUZ_USER_RS_CREATE_CHAR()
        {
            Length = Convert.ToUInt16(Marshal.SizeOf(this));

            Type = PacketType.DPKUZ_USER_RS_CREATE_CHAR;
        }

        public int ErrorCode;
    }
}
