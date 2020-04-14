using System;

namespace WerewolfServer.Game
{
    public abstract class PlaySession
    {
        public abstract bool IsValid { get; } // Should perform integrity validation here
        public abstract bool IsOnline { get; }

        public Player Player { get; protected set; }

        // Should be kept secret
        public string Id;

        public PlaySession()
        {
            Id = Guid.NewGuid().ToString();
        }

        public bool AttachPlayer(Player player)
        {
            Player = player;
            Player.AttachSession(this);
            return true;
        }

        public void KillPlayer()
        {
            if (Player == null)
                return;

            Player.DetachSession();
            Player = null;
        }

        public abstract Player CreatePlayer();
        public abstract void EmitMessage(GameMessage msg);
    }
}