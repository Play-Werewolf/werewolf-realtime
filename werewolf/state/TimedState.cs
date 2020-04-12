namespace WerewolfServer.Game
{
    public abstract class TimedState : GameState
    {

        public abstract GameState NextGamestate { get; }

        float TimeLeft {get;set;}

        public TimedState(GameRoom game, float seconds)
            : base(game)
        {
            TimeLeft = seconds;
        }

        public override void OnTimer(float timeDelta)
        {
            TimeLeft -= timeDelta;

            if (TimeLeft < 0)
            {
                ChangeState(NextGamestate);
            }
        }
    }
}
