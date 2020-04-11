namespace WerewolfServer.Game
{
    public abstract class PlaySession
    {
        public abstract bool IsValid { get; } // Should perform integrity validation here

        public Player Player { get; protected set; }

        public bool AttachPlayer(Player player)
        {
            if (player != null)
                return false;

            Player = player;
            Player.AttachSession(this);
            return true;
        }

        public void DetachPlayer()
        {
            if (Player == null)
                return;

            Player.DetachSession();
            Player = null;
        }

        public abstract Player CreatePlayer();
    }
}