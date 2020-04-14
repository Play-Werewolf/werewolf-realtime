using System;
using System.Linq;
using System.Collections.Generic;

namespace WerewolfServer.Game
{
    public class GameInitState : TimedState
    {
        public override GameState NextGamestate => new RolesLotState(Game);
        public GameInitState(GameRoom game) : base(game, 5) { }

        public override void OnStart()
        {
            foreach (var p in Game.Players)
            {
                p.Result = GameResult.Unknown;
            }
        }
    }

    public class RolesLotState : TimedState
    {
        public override GameState NextGamestate => new NightTransitionState(Game);
        public RolesLotState(GameRoom game) : base(game, 10) { }
    }

    public class NightTransitionState : TimedState
    {
        public override GameState NextGamestate => Game.Config.ConcurrentNight ? (GameState)new ConcurrentNightState(Game) : new SeparatedNightState(Game);
        public NightTransitionState(GameRoom game) : base(game, 5) { }

        public override void OnStart()
        {
            Game.StartNight();
        }
    }

    public class DayTransitionState : TimedState
    {
        public override GameState NextGamestate => new DeathAnnounceState(Game);
        public DayTransitionState(GameRoom game) : base(game, 5) { }

        public override void OnStart()
        {
            Game.ProcessNight();
        }
    }

    public class ExecutionState : TimedState
    {
        public override GameState NextGamestate => new NightTransitionState(Game);
        public ExecutionState(GameRoom game) : base(game, 7) { }

        public override void OnEnd()
        {
            Game.PlayerOnStand.Character.Die();
            Game.PlayerOnStand.Character.OnExecuted();
            Game.PlayerOnStand = null;

            if (Game.IsGameOver())
            {
                ChangeState(new GameOverState(Game));
            }
        }

        public override object Serialize()
        {
            return new
            {
                State = GetType().Name,
                PlayerOnStand = Game.PlayerOnStand.Id,
            };
        }
    }

    public class DeathAnnounceState : GameState
    {
        List<string> Callouts;
        string CurrentCall = null;
        float Timer = -1;

        public DeathAnnounceState(GameRoom game) : base(game)
        {
            Callouts = game.MakeDeathLog().ToList();
        }

        public override void OnTimer(float timeDelta)
        {
            Timer -= timeDelta;
            if (Timer > 0)
                return;


            if (Callouts.Count == 0)
            {
                if (Game.IsGameOver())
                    ChangeState(new GameOverState(Game));
                else
                    ChangeState(new DiscussionState(Game));

                return;
            }

            CurrentCall = Callouts[0];
            Callouts.RemoveAt(0);
            Timer = 0.5f * CurrentCall.Split(new char[] { ' ' }).Length;

            Console.WriteLine(">>> " + CurrentCall);
        }

        public override object Serialize()
        {
            return new
            {
                State = GetType().Name,
                Callout = CurrentCall
            };
        }
    }

    public class GameOverState : TimedState
    {
        public override GameState NextGamestate => new LobbyState(Game);

        public GameOverState(GameRoom game) : base(game, 30) { }
    }
}
