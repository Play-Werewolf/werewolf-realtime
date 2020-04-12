using System;

namespace WerewolfServer.Game
{
    public class GameState
    {
        protected GameRoom Game { get; set; }

        public GameState(GameRoom game)
        {
            Game = game;
        }

        public GameState NextState {get; private set;}

        public GameState TriggerTimer(float timeDelta)
        {
            NextState = null;
            OnTimer(timeDelta);

            if (NextState != null)
            {
                if (NextState != this)
                    OnEnd();
                
                if (NextState != this)
                    NextState.OnStart();

                return NextState;
            }

            return this;
        }

        protected void ChangeState(GameState newState)
        {
            NextState = newState;
        }

        protected void KeepState()
        {
            NextState = this;
        }

        public virtual void OnTimer(float timeDelta) { }
        public virtual void OnStart() { }
        public virtual void OnEnd() { }
    }
}
