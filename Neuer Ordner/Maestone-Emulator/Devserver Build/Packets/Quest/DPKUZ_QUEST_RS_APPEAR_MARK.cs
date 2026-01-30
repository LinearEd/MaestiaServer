using System;
using System.Runtime.InteropServices;

namespace DevServer.Packets
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public class DPKUZ_QUEST_RS_APPEAR_MARK : Packet
    {
        public DPKUZ_QUEST_RS_APPEAR_MARK()
        {
            Length = Convert.ToUInt16(Marshal.SizeOf(this));
            Type = PacketType.DPKUZ_QUEST_RS_APPEAR_MARK;
        }

        public int NpcId;       // NPC that has the quest mark
        public int QuestId;     // Quest ID
        public byte MarkType;   // 0=none, 1=available, 2=progress, 3=complete, etc.
    }
}
