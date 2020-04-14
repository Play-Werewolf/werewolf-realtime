using System;
using System.Collections.Generic;
using System.Text;
using Fleck;

using WerewolfServer.Game;

namespace WerewolfServer.Network
{
    public struct LoginState
    {
        public bool IsAnonymous;
        public bool IsAuthenticated;
        public string Nickname;
        public string UserID;
    }

    public class NetworkSession : PlaySession
    {
        public LoginState LoginState;

        public string AccountInfo { get; set; }

        public NetworkConnection Connection { get; set; }
        public DateTime DisconnectionTime { get; set; }

        public override bool IsValid => throw new NotImplementedException();
        public override bool IsOnline => Connection != null;


        public override Player CreatePlayer()
        {
            return new Player(Id, LoginState.Nickname, "TODO:implement avatars");
        }

        public override void EmitMessage(GameMessage msg)
        {
            Connection?.Socket?.Send(msg.Compile());
        }

        public void InitPlayer()
        {
            if (Player != null)
                KillPlayer();

            AttachPlayer(CreatePlayer());

            Console.WriteLine("Init player called. Player: " + Player);
        }

        public NetworkSession(NetworkConnection connection, LoginState state)
        {
            Connection = connection;
            LoginState = state;
        }

        public void Disconnect()
        {
            Connection = null;
            DisconnectionTime = DateTime.Now;
        }

        public void Close()
        {
            KillPlayer();
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} ({2})", Id, LoginState.Nickname, Connection == null ? "stale" : "alive");
        }
    }
}
