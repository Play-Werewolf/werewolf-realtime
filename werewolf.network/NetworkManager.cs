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

        public void DoCleanup()
        {
            CleanupSessions();
            Rooms.CleanupStaleGames();
        }

        void CleanupSessions()
        {
            DateTime threshold = DateTime.Now - new TimeSpan(0, 0, 10); // TODO: Fix
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
                ses.Close();

                Sessions.RemoveSession(ses);
            }
        }

        public void WorkSingleThreadedly() // TODO: Consider porting to multi-threaded architecture to increase room capacity per WW instance
        {
            TimeSpan delta = new TimeSpan(0, 0, 0, 0, 300);
            TimeSpan cleanupDelta = new TimeSpan(0, 0, 0, 5, 0);
            DateTime nextTimer = DateTime.Now + delta;
            DateTime nextCleanup = DateTime.Now + cleanupDelta;

            while (true)
            {
                while (NetworkConnection.Messages.TryDequeue(out (NetworkConnection conn, NetworkMessage msg) result))
                {
                    result.conn.handlers[result.msg.Type].Init(result.conn, result.msg).ProcessCommand();
                }

                if (DateTime.Now > nextTimer)
                {
                    DateTime timerStart = DateTime.Now;
                    nextTimer = DateTime.Now + delta;
                    foreach (var game in Rooms.Games.Values)
                    {
                        game.Timer();
                    }
                    // Console.WriteLine("Timer in {0}ms", (DateTime.Now - timerStart).TotalMilliseconds.ToString("F7"));
                }

                if (DateTime.Now > nextCleanup)
                {
                    nextCleanup = DateTime.Now + cleanupDelta;
                    DoCleanup();
                }
            }
        }
    }
}
