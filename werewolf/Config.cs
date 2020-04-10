using System;

namespace WerewolfServer.Game
{
    public class GameConfig
    {
        public int MinPlayers { get; set; } = 5;



        public TimeSpan GameInitDelay { get; set; } = new TimeSpan(0, 0, 10);
    }
}