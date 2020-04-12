namespace WerewolfServer.Game
{
    public class GameInitState : TimedState
    {
        public override GameState NextGamestate => new RolesLotState(Game); // TODO
        public GameInitState(GameRoom game) : base(game, 10) { }
    }

    public class RolesLotState : TimedState
    {
        public override GameState NextGamestate => null; // TODO
        public RolesLotState(GameRoom game) : base(game, 10) { }

        public override void OnStart()
        {
            // TODO: Shuffle roles for game
        }
    }

    public class NightTransitionState : TimedState
    {
        public override GameState NextGamestate => null; // TODO
        public NightTransitionState(GameRoom game) : base(game, 5) { }
    }

    public class DayTransitionState : TimedState
    {
        public override GameState NextGamestate => null; // TODO
        public DayTransitionState(GameRoom game) : base(game, 5) { }
    }

    public class ExecutionState : TimedState
    {
        public override GameState NextGamestate => new NightTransitionState(Game);
        public ExecutionState(GameRoom game) : base(game, 10) { }
    }
}
