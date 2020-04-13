using System;
using System.Linq;
using System.Collections.Generic;

using WerewolfServer.Platform;

namespace WerewolfServer.Game
{
    public class GameRoom
    {
        public DateTime LastTimer { get; set; }
        public int CurrentNight { get; set; }

        public List<Player> Players { get; set; } = new List<Player>();
        public List<Player> ReadyPlayers { get; set; } = new List<Player>();
        public List<string> RolesBank { get; set; } = new List<string>();
        public NightPlayOrder[] NightPlayOrders { get; set; }

        public TimeProvider Time { get; set; } = new TimeProvider(); // TODO: Inject dependency?
        public GameState State { get; set; }
        public GameConfig Config { get; set; }

        public string Id { get; private set; }

        public GameRoom(string id)
        {
            Id = id;
            Reset();
            Config = new GameConfig();
        }

        public GameRoom() : this("") { }

        public override string ToString()
        {
            string s = Id + ") ";
            foreach (var player in Players)
            {
                s += player.Name + ", ";
            }
            return s;
        }

        public void Reset()
        {
            Players.Clear();
            CurrentNight = 0;
            LastTimer = Time.Now;
            State = new LobbyState(this);
        }

        public void InitGame(Character[] characters)
        {
            for (var i = 0; i < characters.Length; i++)
            {
                Players[i].AttachCharacter(characters[i]);
            }

            NightPlayOrders = Players
                .Select(p => p.Character.NightOrder)
                .Where(o => o != NightPlayOrder.NoPriority)
                .ToArray();
        }

        public void Timer()
        {
            var now = Time.Now;
            var newState = State.TriggerTimer((float)(now - LastTimer).TotalSeconds);
            if (newState != State)
            {
                State = newState; // TODO: Send state update to clients
            }

            LastTimer = now;
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

        public void RemovePlayer(Player player)
        {
            if (player == null)
            {
                throw new InvalidOperationException("Cannot remove null player from a gameroom");
            }
            player.Game = null;

            if (this.ReadyPlayers.Contains(player))
                this.ReadyPlayers.Remove(player);

            if (this.Players.Contains(player))
                this.Players.Remove(player);
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

        public void PlayerReady(Player player)
        {
            if (this.ReadyPlayers.Contains(player))
                return;

            this.ReadyPlayers.Add(player);
        }

        public void PlayerNotReady(Player player)
        {
            if (!this.ReadyPlayers.Contains(player))
                return;

            this.ReadyPlayers.Remove(player);
        }
    }
}
