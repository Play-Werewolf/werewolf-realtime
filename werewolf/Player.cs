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
        public bool IsConnected => Session != null;

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

        public override string ToString()
        {
            return string.Format("[player] {0}", Character);
        }
    }
}
