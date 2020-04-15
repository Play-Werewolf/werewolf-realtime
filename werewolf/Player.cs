namespace WerewolfServer.Game
{
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AvatarURL { get; set; } // ?


        public Character Character { get; set; }
        public GameRoom Game { get; set; }
        public PlaySession Session { get; private set; }

        // Returns true if the player has a session (that might persist some time after the player has disconnected)
        public bool IsConnected => Session != null;

        // Returns true if the player is online (will become false the second the user disconnects)
        public bool IsOnline => IsConnected && Session.IsOnline;

        public Player VotesAgainst { get; set; }
        public int Votes { get; set; }

        // True - votes to kill, False - votes innocent, null - abstains
        public bool? TrialVote { get; set; }
        public GameResult Result { get; set; }

        public Player() { }

        public Player(string id, string name, string avatar)
        {
            Id = id;
            Name = name;
            AvatarURL = avatar;
        }

        public Player(Character player)
        {
            Character = player;
            Character.Player = this; // One to one relationship between the player and the character
        }

        public Player(Character player, PlaySession s)
        {
            Character = player;
            Character.Player = this; // One to one relationship between the player and the character
            Session = s;
        }

        public void AttachCharacter(Character character)
        {
            if (Character != null)
                return;

            Character = character;
            Character.Player = this;
        }

        public static implicit operator Character(Player player)
        {
            return player.Character;
        }

        public static implicit operator Player(Character player)
        {
            return player.Player;
        }

        public void AttachSession(PlaySession session)
        {
            Session = session;
        }

        public void DetachSession()
        {
            Session = null;

            if (Game?.State is LobbyState)
            {
                Game.RemovePlayer(this);
            }
        }

        public void ChangeRole(Character c)
        {
#if (DEBUG)
            c.WithName(Character._name);
#endif
            // Copying all fields to the new Character object
            c.DeathNight = Character.DeathNight;

            // For preserving messages
            c.Night = Character.Night;

            c.Player = this;
            Character = c;
        }

        public void VoteToKill(Player p)
        {
            if (!Character.Alive)
                return;

            if (VotesAgainst != null)
                VotesAgainst.Votes--;

            VotesAgainst = p;
            if (VotesAgainst != null)
                VotesAgainst.Votes++;

            if (VotesAgainst.Votes > Game.Players.Count / 2)
            {
                Game.PlayerOnStand = VotesAgainst;
            }
        }

        public void ResetVote()
        {
            if (!Character.Alive)
                return;

            if (VotesAgainst != null)
                VotesAgainst.Votes--;

            VotesAgainst = null;
        }

        public void VoteGuilty()
        {
            if (!Character.Alive)
                return;
            TrialVote = true;
        }

        public void VoteInnocent()
        {
            if (!Character.Alive)
                return;
            TrialVote = false;
        }

        public void UndoTrialVote()
        {
            if (!Character.Alive)
                return;
            TrialVote = null;
        }

        public override string ToString()
        {
            return string.Format("[{1}] {0}", Character, Name);
        }
    }
}
