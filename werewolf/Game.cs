using System;
using System.Collections.Generic;

namespace WerewolfServer.Game
{
    public class GameRoom
    {
        public Random Random = new Random();

        public int CurrentNight { get; set; }

        public List<Player> Players { get; set; } = new List<Player>();

        public GameRoom()
        {
            Reset();
        }

        public void Reset()
        {
            Players.Clear();
            CurrentNight = 0;
        }

        public void AddPlayer(Player p)
        {
            if (p == null)
            {
                throw new InvalidOperationException("Cannot insert null player to a gameroom");
            }
            p.Game = this;
            this.Players.Add(p);
        }

        public void StartNight()
        {
            foreach (var p in Players.Prioritized())
                p.Character.OnBeforeNight();

            foreach (var p in Players.Prioritized())
                p.Character.Night.Reset();

            foreach (var p in Players.Prioritized())
                p.Character.OnNightStart();
        }

        public void ProcessNight()
        {
            foreach (var p in Players.Prioritized())
                if (p.Character.Night.Action.ShouldAct)
                    p.Character.PreAction();

            foreach (var p in Players.Prioritized())
                if (p.Character.Night.Action.ShouldAct)
                    p.Character.DoAction();

            foreach (var p in Players.Prioritized())
                if (p.Character.Night.Action.ShouldAct)
                    p.Character.PostAction();

            foreach (var p in Players.Prioritized())
                p.Character.Night.CalculateResults();

            foreach (var p in Players.Prioritized())
                if (p.Character.DeathNight == CurrentNight)
                    p.Character.OnDeath();

            foreach (var p in Players.Prioritized())
                p.Character.OnNightEnd();
        }

        public string[] MakeDeathLog()
        {
            List<string> callouts = new List<string>();

            foreach (var p in Players)
            {
                if (p.Character.DeathNight != CurrentNight)
                    continue;

                callouts.Add("We found " + p + ", dead in their house tonight");
                callouts.Add(p.Character.FormatDeathMessage());
            }

            return callouts.ToArray();
        }
    }
}
