using System;

namespace WerewolfServer.Game
{
    public class GameInitState : TimedState
    {
        public GameInitState(GameRoom game)
            : base(game, new TimeSpan(0, 0, 10)) { }

        public override GameState OnTimer()
        {
            // TODO: Return next state
            return this;
        }
    }
}
