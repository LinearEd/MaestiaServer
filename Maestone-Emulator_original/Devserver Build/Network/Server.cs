using System;
using System.Net;
using System.Reflection;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using DevServer.Packets;
using DevServer.Handlers;

namespace DevServer.Network
{
    public class Server : TcpListener
    {
        public HandlerType HandlerType { get; }

        private readonly Dictionary<PacketType, Action<Packet, Client>> _packetHandlers;

        public Server(int serverPort, HandlerType handlerType) : base(IPAddress.Any, serverPort)
        {
            HandlerType = handlerType;
            _packetHandlers = new Dictionary<PacketType, Action<Packet, Client>>();
        }

        public void Initialize()
        {
            Start();
            BeginAcceptTcpClient(OnAccept, this);

            // Register existing packet handlers
            var handleNewConnectionMethod = typeof(PacketHandlers).GetMethod("HandleNewConnection");
            AddPacketHandler(handleNewConnectionMethod, PacketType.SPK_NEW_CONNECTION);
        }


        private void OnAccept(IAsyncResult asyncResult)
        {
            try
            {
                var tcpClient = EndAcceptTcpClient(asyncResult);
                Log.WriteInfo($"[{HandlerType}] Accepted client connection from {tcpClient.Client.RemoteEndPoint}.");
                var client = new Client(tcpClient, this);
            }
            catch (Exception ex)
            {
                Log.WriteError($"[{HandlerType}] Error accepting client: {ex.Message}");
            }
            finally
            {
                BeginAcceptTcpClient(OnAccept, this);
            }
        }

        public void AddPacketHandler(MethodInfo methodInfo, PacketType packetType)
        {
            var packetHandler = (Action<Packet, Client>)methodInfo.CreateDelegate(typeof(Action<Packet, Client>));
            if (packetHandler != null)
            {
                _packetHandlers[packetType] = packetHandler;
            }
            else
            {
                Log.WriteError($"[{HandlerType}] Failed to create delegate for {packetType}");
            }
        }

        public void Handle(Client packetSender, byte[] packetData)
        {
            if (packetData.Length < 4)
            {
                Log.WriteError($"[{HandlerType}] Packet too short. Data: {BitConverter.ToString(packetData)}");
                return;
            }

            var packetLength = BitConverter.ToUInt16(packetData, 0) & 0x1FFF;
            var packetMainId = packetData[2];
            var packetSubId = packetData[3];
            var packetRawType = BitConverter.ToUInt16(packetData, 2);

            Log.WriteInfo($"[{HandlerType}] Received packet. Length: {packetLength}, MainId: {packetMainId}, SubId: {packetSubId}, RawType: {packetRawType}");

            if (!Enum.IsDefined(typeof(PacketType), packetRawType))
            {
                Log.WriteWarning($"[{HandlerType}] Received unknown packet type. ( Length: {packetLength}, MainId: {packetMainId}, SubId: {packetSubId} )");
                Log.WriteWarning($"Raw packet data: {BitConverter.ToString(packetData)}");
                return;
            }

            if (!Enum.TryParse(packetRawType.ToString(), out PacketType packetType))
            {
                Log.WriteError($"[{HandlerType}] Failed to parse packet type. ( Length: {packetLength}, MainId: {packetMainId}, SubId: {packetSubId} )");
                return;
            }

            var packetStruct = Type.GetType($"{typeof(Packet).Namespace}.{packetType}");

            if (packetStruct == null)
            {
                Log.WriteWarning($"[{HandlerType}] Packet {packetType} is unmapped.");
                return;
            }

            var fixedLength = Marshal.SizeOf(packetStruct);

            if (packetData.Length > fixedLength)
                Array.Resize(ref packetData, fixedLength);

            var arrayHandle = GCHandle.Alloc(packetData, GCHandleType.Pinned);

            try
            {
                var clientPacket = Marshal.PtrToStructure(arrayHandle.AddrOfPinnedObject(), packetStruct) as Packet;

                if (clientPacket == null)
                {
                    Log.WriteError($"[{HandlerType}] Failed to convert packet data to structure. PacketType: {packetType}");
                    return;
                }

                if (!_packetHandlers.TryGetValue(packetType, out var packetHandler))
                {
                    Log.WriteWarning($"[{HandlerType}] Packet {packetType} is unhandled.");
                    return;
                }

                Log.WriteSuccess($"[{HandlerType}] Received {packetHandler.Method.Name}.");
                packetHandler(clientPacket, packetSender);
            }
            catch (Exception ex)
            {
                Log.WriteError($"[{HandlerType}] Error handling packet: {ex.Message}");
            }
            finally
            {
                arrayHandle.Free();
            }
        }
    }
}
