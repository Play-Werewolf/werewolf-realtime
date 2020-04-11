using System;

namespace WerewolfServer.Game
{
    public class GameInitState : TimedState
    {
        public GameInitState(GameRoom game)
            : base(game, new TimeSpan(0, 0, 10)) { }

        public override GameState OnTimer()
        {
            return new RoleLotState(Game);
        }
    }

    public class RoleLotState : TimedState
    {
        public RoleLotState(GameRoom game)
            : base(game, new TimeSpan(0, 0, 10)) { }

        public override GameState OnTimer()
        {
            // TODO: Return next state
            return this;
        }
    }

    public class DayTransitionState : TimedState
    {
        public DayTransitionState(GameRoom game)
            : base(game, new TimeSpan(0, 0, 5)) { }

        public override GameState OnTimer()
        {
            // TODO: Return next state
            return this;
        }
    }

    public class ExecutionState : TimedState
    {
        public ExecutionState(GameRoom game)
            : base(game, new TimeSpan(0, 0, 10)) { }

        public override GameState OnTimer()
        {
            // TODO: Return next state
            return this;
        }
    }

    public class NightTransitionState : TimedState
    {
        public NightTransitionState(GameRoom game)
            : base(game, new TimeSpan(0, 0, 5)) { }

        public override GameState OnTimer()
        {
            // TODO: Return next state
            return this;
        }
    }
}
