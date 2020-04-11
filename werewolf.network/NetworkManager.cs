using System;
using System.Collections.Generic;
using Fleck;

using WerewolfServer.Management;
using WerewolfServer.Platform;

namespace WerewolfServer.Network
{
    public class NetworkManager
    {
        public SessionManager<NetworkSession> Sessions;
        public List<NetworkConnection> Connections;
        public RoomManager Rooms;
        public WebSocketServer Server;
        public TimeProvider TimeProvider { get; set; } = new TimeProvider();

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
                conn.Disconnect();
                if (Connections.Contains(conn))
                    Connections.Remove(conn);
            };
        }

        public void ClearSessions()
        {
#if (DEBUG)
            DateTime threshold = DateTime.Now - new TimeSpan(0, 2, 10);
#else
            DateTime threshold = DateTime.Now - new TimeSpan(0, 2, 0);
#endif
            List<NetworkSession> toDelete = new List<NetworkSession>();
            foreach (var ses in Sessions.Sessions)
            {
                if (ses.Connection == null && ses.DisconnectionTime < threshold)
                {
                    toDelete.Add(ses);
                }
            }

            foreach (var ses in toDelete)
            {
                Sessions.RemoveSession(ses);
            }
        }
    }
}
