using System;
using System.Runtime.InteropServices;

namespace DevServer.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public class DPKUL_CHAR_RS_NPC : Packet
    {
        public DPKUL_CHAR_RS_NPC()
        {
            Length = Convert.ToUInt16(Marshal.SizeOf(this));
            Type = PacketType.DPKUL_CHAR_RS_NPC;
        }

        public int NpcUniqueId;    // Unique instance ID for this NPC spawn
        public int NpcTemplateId;  // NPC type/template from game data
        public int PositionX;      // X coordinate (encoded)
        public int PositionY;      // Y coordinate (encoded)
        public int PositionZ;      // Z coordinate (encoded)
        public short Rotation;     // Facing direction
        public byte NpcType;       // NPC type (0=normal, 1=quest, etc)
    }
}