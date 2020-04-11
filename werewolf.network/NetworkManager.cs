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
        RoomManager Rooms;
        WebSocketServer Server;

        public NetworkManager()
        {
            Sessions = new SessionManager<NetworkSession>();
            Rooms = new RoomManager();
            Server = new WebSocketServer("ws://0.0.0.0");
            Server.Start(HandleSocket);
        }

        public void HandleSocket(IWebSocketConnection socket)
        {
            NetworkSession ses = null;

            socket.OnOpen = () =>
            {
                ses = new NetworkSession(socket);
                Sessions.AddSession(ses);
            };

            socket.OnClose = () =>
            {
                // TODO: Do not remove session from session manager immediately, and also make
                // some code to recheck session connection.
                Sessions.RemoveSession(ses);
            };
        }
    }
}
