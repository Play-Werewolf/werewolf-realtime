using System;

namespace WerewolfServer.Game
{
    public abstract class GameState
    {
        protected GameRoom Game { get; set; }

        public GameState(GameRoom game)
        {
            Game = game;
        }

        public virtual object Serialize()
        {
            return new
            {
                State = GetType().Name,
            };
        }

        public GameState NextState {get; private set;}
        public bool RequestUpdate { get; private set; }

        public GameState TriggerTimer(float timeDelta)
        {
            NextState = null;
            RequestUpdate = false;
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

        protected void SendUpdate()
        {
            RequestUpdate = true;
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
