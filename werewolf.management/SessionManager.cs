using System;
using System.Collections.Generic;

using WerewolfServer.Game;

namespace WerewolfServer.Management
{
    public class SessionManager<TSession> where TSession : PlaySession
    {
        public List<TSession> Sessions;

        public SessionManager()
        {
            Sessions = new List<TSession>();
        }

        public void AddSession(TSession session)
        {
            if (Sessions.Contains(session))
                return;

            Sessions.Add(session);
        }

        public void RemoveSession(TSession session)
        {
            if (!Sessions.Contains(session))
                return;

            if (session.Player != null)
                session.DetachPlayer();

            Sessions.Remove(session);
        }

        public TSession GetSession(string sessionId)
        {
            // Can we optimize this?
            foreach (var ses in Sessions)
            {
                if (ses.Id == sessionId)
                    return ses;
            }
            return null;
        }

        public void AddSessionToRoom(TSession session, GameRoom room)
        {
            if (session.Player != null)
                throw new InvalidOperationException("Session already participates in a game.");

            if (!session.IsValid)
                throw new InvalidOperationException("Session is invalid!");

            Player player = session.CreatePlayer();
            session.AttachPlayer(player);
            room.AddPlayer(player);
        }
    }
}
