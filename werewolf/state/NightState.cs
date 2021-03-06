using System;
using System.Linq;

using WerewolfServer.Platform;

namespace WerewolfServer.Game
{
    public class GoodNightState : TimedState
    {
        public override GameState NextGamestate
        {
            get
            {
                if (Index + 1 >= Game.NightPlayOrders.Length)
                {
                    return new DayTransitionState(Game);
                }

                return new SeparatedNightState(Game, Index + 1);
            }
        }

        int Index;
        public GoodNightState(GameRoom game, int index) : base(game, 3)
        {
            Index = index;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", base.ToString(), Index);
        }
    }

    public class SeparatedNightState : GameState
    {
        int Index;
        float Timer = 0;
        float MaxTime, MinTime;

        public SeparatedNightState(GameRoom game, int index = 0) : base(game)
        {
            if (index >= game.NightPlayOrders.Length) // TODO: This should not stay the way it is (night without plays should be optimized out or not occur)
            {
                ChangeState(new DayTransitionState(game));
                return;
            }

            Index = index;
            MaxTime = game.NightPlayOrders[index].GetPlayTime();
            MinTime = (float)Rand.Random.NextDouble() * MaxTime;
        }

        public override void OnTimer(float timeDelta)
        {
            Timer += timeDelta;
            if (Timer > MaxTime || ( // TODO: Test
                Game.Players.Count(p =>
                    p.Character.NightOrder == Game.NightPlayOrders[Index] &&
                    !p.Character.Night.HasPlayed
                ) == 0) && Timer > MinTime)
            {
                ChangeState(new GoodNightState(Game, Index));
                return;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", base.ToString(), Index);
        }

        public override object Serialize()
        {
            return new
            {
                State = GetType().Name,
                Index = Index,
                NightOrder = Game.NightPlayOrders[Index],
                ActivePlayers = Game.Players
                    .Where(p => p.Character.NightOrder == Game.NightPlayOrders[Index])
                    .Select(p => p.Id),
            };
        }
    }

    public class ConcurrentNightState : TimedState
    {
        public override GameState NextGamestate => new DayTransitionState(Game);
        public ConcurrentNightState(GameRoom game) : base(game, 60) { }
    }
}
