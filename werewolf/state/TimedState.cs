using System;

namespace WerewolfServer.Game
{
    public class TimedState : GameState
    {
        public DateTime CreatedOn { get; set; }
        public DateTime EndsOn { get; set; }

        public TimedState(GameRoom game, TimeSpan length) : base(game)
        {
            CreatedOn = Game.Time.Now;
            EndsOn = CreatedOn + length;

            RegisterHandler(CommandType.Timer, HandleTimer);
        }

        public GameState HandleTimer(GameCommand command)
        {
            if (Game.Time.Now > EndsOn)
            {
                return OnTimer();
            }
            return this;
        }

        public virtual GameState OnTimer()
        {
            return this;
        }
    }
}
