namespace WerewolfServer.Game
{
    public abstract class TimedState : GameState
    {

        public abstract GameState NextGamestate { get; }

        public override object Serialize()
        {
            return new
            {
                State = GetType().Name,
                Timer = TimeLeft,
            };
        }

        protected float TimeLeft {get;set;}

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
