using System;
using System.Collections.Generic;

using WerewolfServer.Game;

namespace WerewolfServer.Management
{
    public class SessionManager
    {
        public List<PlaySession> Sessions;

        public void AddSession(PlaySession session)
        {
            if (Sessions.Contains(session))
                return;

            Sessions.Add(session);
        }

        public void RemoveSession(PlaySession session)
        {
            if (!Sessions.Contains(session))
                return;

            if (session.Player != null)
                session.DetachPlayer();
        }

        public void AddSessionToRoom(PlaySession session, GameRoom room)
        {
            if (session.Player != null)
                throw new InvalidOperationException("Session already participates in a game.");

            Player player = session.CreatePlayer();
            session.AttachPlayer(player);
            room.AddPlayer(player);
        }
    }
}
