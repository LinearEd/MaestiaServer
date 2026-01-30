using DevServer.Network;
using DevServer.Packets;

namespace DevServer.Handlers
{
    public static class PacketHandlers
    {
        public static void HandleNewConnection(Packet packet, Client client)
        {
            var newConnectionPacket = packet as SPK_NEW_CONNECTION;

            if (newConnectionPacket == null)
            {
                Log.WriteError("Failed to cast packet to SPK_NEW_CONNECTION.");
                return;
            }

            Log.WriteInfo($"New connection: Id={newConnectionPacket.ConnectionId}, UserName={newConnectionPacket.UserName}");

            // Implementiere deine Logik zur Verarbeitung der neuen Verbindung hier.
        }
    }
}
