namespace WerewolfServer.Game
{
    public class Player
    {
        public Character Character;
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
    }
}
