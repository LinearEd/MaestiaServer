using System;
using System.Runtime.InteropServices;

namespace DevServer.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public class SPK_UNKNOWN_67_32 : Packet
    {
        public SPK_UNKNOWN_67_32()
        {
            Length = Convert.ToUInt16(Marshal.SizeOf(this));
            Type = PacketType.SPK_UNKNOWN_67_32;
        }
    }
}
