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

        public IWebSocketConnection Socket { get; set; }

        public override bool IsValid => throw new NotImplementedException();

        public override Player CreatePlayer()
        {
            throw new NotImplementedException();
        }

        public NetworkSession(IWebSocketConnection socket)
        {
            Socket = socket;
        }

        public bool ConnectAnonymously(string nickname)
        {
            if (LoginState.IsAuthenticated)
                return true;

            LoginState.IsAnonymous = true;
            LoginState.IsAuthenticated = true;
            LoginState.Nickname = nickname;
            return true;
        }

        public bool ConnectWithCreds()
        {
            return false; // Not implemented
        }
    }
}
