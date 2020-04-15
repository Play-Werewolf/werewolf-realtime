namespace WerewolfServer.Game
{
    public struct Attack
    {
        public Player Attacker { get; set; }

        public Player Target { get; set; }

        public Power Power { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("Attacker: {0}, Descriptions{1}", this.Attacker, this.Description);
        }
    }

    public struct Defense
    {
        public Player Defender { get; set; }

        public Player Target { get; set; }

        public Power Power { get; set; }

        public string Description;
    }
}
