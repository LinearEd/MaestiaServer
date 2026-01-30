using System;
using System.Runtime.InteropServices;

namespace DevServer.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public class DPK_UNKNOWN_67_33 : Packet
    {
        public DPK_UNKNOWN_67_33()
        {
            Length = Convert.ToUInt16(Marshal.SizeOf(this));
            Type = PacketType.DPK_UNKNOWN_67_33;
        }
        
        public int Unknown;
    }
}
