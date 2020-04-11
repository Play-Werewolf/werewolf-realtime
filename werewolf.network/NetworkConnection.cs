using System;
using System.Collections.Generic;
using System.Linq;
using Fleck;

namespace WerewolfServer.Network
{

    public class NetworkConnection
    {
        NetworkManager Manager;
        NetworkSession Session;
        IWebSocketConnection Socket;

        Dictionary<string, Action<NetworkMessage>> handlers;

        public NetworkConnection(NetworkManager manager, IWebSocketConnection connection)
        {
            Console.WriteLine("A new network connection was opened");
            Manager = manager;
            Socket = connection;

            handlers = new Dictionary<string, Action<NetworkMessage>>()
            {
                ["echo"] = Echo,
                ["authenticate"] = Authenticate,
                ["authenticate_anonymous"] = AuthenticateAnonymous,
                ["restore_session"] = RestoreSession,
            };

            Socket.OnMessage = (msg) =>
            {
                var message = new NetworkMessage(msg);
                if (handlers.ContainsKey(message.Type))
                {
                    handlers[message.Type](message);
                }
            };
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

        public void Send(string messageType, params string[] args)
        {
            Socket.Send(new NetworkMessage(messageType, args).Compile());
        }

        void Login(NetworkSession session)
        {
            Session = session;
            session.Connection = this;
            Manager.Sessions.AddSession(session);
        }

        void Echo(NetworkMessage message)
        {
            Console.WriteLine("Echo message");
            Socket.Send(new NetworkMessage("echo_reply", message.Args.ToArray()).Compile());
        }

        void AuthenticateAnonymous(NetworkMessage message)
        {
            if (Session != null)
            {
                SendSessionStatus();
                return;
            }

            if (message.Args.Length != 1)
            {
                SendAuthenticationError();
                return;
            }

            var nickname = message.Args[0];

            Login(new NetworkSession(this, new LoginState
            {
                IsAnonymous=true,
                IsAuthenticated=true,
                Nickname=nickname,
                UserID=""
            }));

            SendSessionStatus();
        }

        void Authenticate(NetworkMessage message)
        {
            SendAuthenticationError("Not implemented");
        }

        void RestoreSession(NetworkMessage message)
        {
            if (message.Args.Length != 1)
            {
                SendAuthenticationError("Invalid number of arguments");
                return;
            }

            if (Session != null)
            {
                SendAuthenticationError("Connection is already bound to session");
                return;
            }

            var sessionId = message.Args[0];
            var session = Manager.Sessions.GetSession(sessionId);
            Console.WriteLine(">>> {0}", session);
            if (session == null)
            {
                SendAuthenticationError("Session not found");
                return;
            }

            if (session.Connection != null)
            {
                SendAuthenticationError("Session is already bound");
                return;
            }

            Login(session);
            SendSessionStatus();
        }

        void SendSessionStatus()
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

        void SendAuthenticationError(string reason = null)
        {
            Send(messageType: "session_status", "authentication_error", reason);
        }
    }
}
