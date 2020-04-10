using System;
using System.Collections.Generic;

namespace WerewolfServer.Game
{
    public class GameRoom
    {
        public Random Random = new Random();
        
        public int Night {get;set;}

        public List<Player> Players {get;set;} = new List<Player>();

        public GameRoom()
        {
            Reset();
        }

        public void Reset()
        {
            Players.Clear();
            Night = 0;
        }

        public void AddPlayer(Player p)
        {
            if (p == null) {
                throw new InvalidOperationException("Cannot insert null player to a gameroom");
            }
            p.Character.Game = this;
            this.Players.Add(p);
        }

        public void StartNight()
        {
            foreach (var p in Players)
                p.Character.OnBeforeNight();

            foreach (var p in Players)
                p.Character.Night.Reset();

            foreach (var p in Players)
                p.Character.OnNightStart();
        }

        public void ProcessNight()
        {
            foreach (var p in Players)
                if (p.Character.Night.Action.ShouldAct)
                    p.Character.PreAction();

            foreach (var p in Players)
                if (p.Character.Night.Action.ShouldAct)
                    p.Character.DoAction();

            foreach (var p in Players)
                if (p.Character.Night.Action.ShouldAct)
                    p.Character.PostAction();

            foreach (var p in Players)
                p.Character.Night.CalculateResults();

            foreach (var p in Players)
                p.Character.OnNightEnd();
        }
    }
}
