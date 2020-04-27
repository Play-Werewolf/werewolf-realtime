using System;

namespace WerewolfServer.Game
{
    public class GameConfig
    {
        public int MinPlayers { get; set; } = 1; // TODO: Change to 5 or 6 for production
        public bool ConcurrentNight { get; set; } = false;

        public TimeSpan GameInitDelay { get; set; } = new TimeSpan(0, 0, 10);
    }
}