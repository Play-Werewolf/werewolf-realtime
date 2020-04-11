using System;
using Fleck;

using WerewolfServer.Management;
using WerewolfServer.Game;

namespace WerewolfServer.Network
{
    class Program
    {
        static void Main(string[] args)
        {
            SessionManager sessions = new SessionManager();
            RoomManager rooms = new RoomManager();

            var server = new WebSocketServer("ws://0.0.0.0", true);
            server.Start(sockt =>
            {

            });
        }
    }
}
