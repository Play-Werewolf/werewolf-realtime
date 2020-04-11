using System;
using System.Collections.Generic;
using System.Text;
using Fleck;

using WerewolfServer.Management;

namespace WerewolfServer.Network
{
    public class NetworkManager
    {
        SessionManager<NetworkSession> Sessions;
        List<NetworkConnection> Connections;
        RoomManager Rooms;
        WebSocketServer Server;

        public NetworkManager()
        {
            Connections = new List<NetworkConnection>();
            Sessions = new SessionManager<NetworkSession>();
            Rooms = new RoomManager();
            Server = new WebSocketServer("ws://0.0.0.0");
        }

        public void Start()
        {
            Server.Start(HandleSocket);
        }

        public void HandleSocket(IWebSocketConnection socket)
        {
            NetworkConnection conn = null;

            socket.OnOpen = () =>
            {
                conn = new NetworkConnection(this, socket);
                Connections.Add(conn);
            };

            socket.OnClose = () =>
            {
                if (Connections.Contains(conn))
                    Connections.Remove(conn);
            };
        }
    }
}
