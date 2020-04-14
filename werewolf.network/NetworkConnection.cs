using System.Collections.Generic;
using System.Collections.Concurrent;
using Fleck;

using WerewolfServer.Game;

namespace WerewolfServer.Network
{

    public class NetworkConnection
    {
        public static ConcurrentQueue<(NetworkConnection, NetworkMessage)> Messages { get; set; } = new ConcurrentQueue<(NetworkConnection, NetworkMessage)>();

        public NetworkManager Manager { get; private set; }
        public NetworkSession Session { get; private set; }
        public IWebSocketConnection Socket { get; private set; }

        public Dictionary<string, BaseCommand> handlers = new Dictionary<string, BaseCommand>();

        public bool IsInRoom => Session?.Player?.Game != null;

        void RegisterCommand<T>() where T: BaseCommand, new()
        {
            var cmd = new T();
            handlers[cmd.CommandType] = cmd;
        }

        void Init()
        {

            Socket.OnMessage = (msg) =>
            {
                var message = new NetworkMessage(msg);
                if (handlers.ContainsKey(message.Type))
                {
                    lock (Messages)
                    {
                        Messages.Enqueue((this, message));
                    }
                }
            };
        }

        public NetworkConnection(NetworkManager manager, IWebSocketConnection connection)
        {
            Manager = manager;
            Socket = connection;

            Init();

            RegisterCommand<PingCommand>();
            RegisterCommand<AuthenticateAnonymous>();
            RegisterCommand<Authenticate>();
            RegisterCommand<RestoreSession>();

            RegisterCommand<CreateRoomCommand>();
            RegisterCommand<JoinRoomCommand>();
            RegisterCommand<LeaveRoomCommand>();

            RegisterCommand<ReadyCommand>();
            RegisterCommand<NotReadyCommand>();
        }

        public override string ToString()
        {
            return Socket.ConnectionInfo.ClientIpAddress + ":" + Socket.ConnectionInfo.ClientPort;
        }

        public void Disconnect()
        {
            if (Session != null)
            {
                Session.Disconnect();
            }
        }

        public void Send(GameMessage message)
        {
            Socket.Send(message.Compile()); // TODO: Make sure this is not blocking so we can dump messages as fast as possible
        }

        public void Send(string messageType, params string[] args)
        {
            Send(new NetworkMessage(messageType, args));
        }

        public void Login(NetworkSession session)
        {
            Session = session;
            session.Connection = this;
            Manager.Sessions.AddSession(session);
        }

        public void SendSessionStatus()
        {
            if (Session == null)
            {
                Send(
                    messageType: "session_status",
                    "not_authenticated"
                );
            }
            else
            {
                Send(
                    messageType: "session_status",
                    "authenticated",
                    Session.Id,
                    Session.LoginState.IsAuthenticated.ToString(),
                    Session.LoginState.IsAnonymous.ToString(),
                    Session.LoginState.Nickname,
                    Session.LoginState.UserID
                );
            }
        }

        public void SendAuthenticationError(string reason = null)
        {
            Send(messageType: "session_status", "authentication_error", reason);
        }

        public void SendRoomStatus()
        {
            if (!IsInRoom)
            {
                Send(messageType: "room_status", "not_connected");
                return;
            }

            Send(
                messageType: "room_status",
                "connected",
                Session.Player.Game.Id
            );
        }
    }
}
