using System;
using System.Linq;
using System.Collections.Generic;

using WerewolfServer.Platform;

namespace WerewolfServer.Game
{
    // Interface for sending game updates
    public interface IGameUpdater
    {
        void SendGameUpdate(GameRoom game, IEnumerable<Player> targets = null);
        void SendStateUpdate(GameRoom game, IEnumerable<Player> targets = null);
        void SendPlayerUpdate(Player player, IEnumerable<Player> targets = null);
        void SendTimerUpdate(float time, IEnumerable<Player> targets = null);
        void SendMessage(Message message, IEnumerable<Player> targets = null);
    }

    public class GameRoom
    {
        public DateTime LastTimer { get; set; }
        public int CurrentNight { get; set; }

        public List<Player> Players { get; set; } = new List<Player>();
        public Player PlayerOnStand { get; set; }
        public List<Player> ReadyPlayers { get; set; } = new List<Player>();
        public List<string> RolesBank { get; set; } = new List<string>() { "werewolf", "villager" }; // TODO: Make this a bit more custom
        public NightPlayOrder[] NightPlayOrders { get; set; }

        public TimeProvider Time { get; set; } = new TimeProvider(); // TODO: Inject dependency?
        public GameState State { get; set; }
        public GameConfig Config { get; set; }

        public string Id { get; private set; }

        IGameUpdater _Updater;

        public GameRoom(string id, IGameUpdater updater)
        {
            Id = id;
            Reset();
            _Updater = updater;
        }

        public GameRoom(IGameUpdater updater) : this("", updater) { }

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

            Config = new GameConfig();
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
                .Distinct()
                .ToArray();
        }

        public void Timer()
        {
            var now = Time.Now;
            var newState = State.TriggerTimer((float)(now - LastTimer).TotalSeconds);
            if (newState != State || State.RequestUpdate)
            {
                State = newState;
                SendStateUpdate();
            }

            LastTimer = now;
        }

        public void SendStateUpdate()
        {
            _Updater?.SendStateUpdate(this);
        }

        public void SendPlayerUpdate(IEnumerable<Player> players = null)
        {
            if (players == null)
            {
                players = Players;
            }

            foreach (var p in players)
            {
                _Updater.SendPlayerUpdate(p);
            }
        }

        public void AddPlayer(Player p)
        {
            if (!(State is LobbyState) && !(State is GameOverState))
                throw new InvalidOperationException("Cannot join room in the middle of a game");

            if (p == null)
            {
                throw new InvalidOperationException("Cannot insert null player to a gameroom");
            }
            p.Game = this;
            this.Players.Add(p);

            _Updater?.SendGameUpdate(this);
            _Updater?.SendStateUpdate(this, new[] { p }); // Send state to new player
        }

        public Player GetPlayer(string Id)
        {
            return Players.FirstOrDefault(x => x.Id == Id);
        }

        public bool IsAdmin(Player p) // KISS so this is a simple implementation. We might wish to change this later :/
        {
            return p == Players?[0];
        }

        public void PlayerReconnected(Player p)
        {
            if (!Players.Contains(p))
                return;

            _Updater?.SendGameUpdate(this, new[] { p });
            _Updater?.SendStateUpdate(this, new[] { p });
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

            _Updater?.SendGameUpdate(this);
        }

        public void SetRolesList(string[] roles)
        {
            RolesBank = roles.Where(role => RoleGenerator_.Generators.ContainsKey(role)).ToList();
            _Updater?.SendGameUpdate(this);
        }

        public void StartNight()
        {
            CurrentNight++;
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
                if (p.Character.ShouldAct())
                    p.Character.PreAction();

            foreach (var p in Players.Prioritized())
                if (p.Character.ShouldAct())
                    p.Character.DoAction();

            foreach (var p in Players.Prioritized())
                if (p.Character.ShouldAct())
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

                callouts.Add("We found " + p.Name + ", dead in their house tonight");
                callouts.Add(p.Character.FormatDeathMessage());
                callouts.Add("Rest in peace, " + p.Name);
            }

            return callouts.ToArray();
        }

        public void PlayerReady(Player player)
        {
            if (!(State is LobbyState))
                return;

            if (ReadyPlayers.Contains(player))
                return;

            ReadyPlayers.Add(player);

            SendStateUpdate();
        }

        public void PlayerNotReady(Player player)
        {
            if (!(State is LobbyState))
                return;

            if (!this.ReadyPlayers.Contains(player))
                return;

            this.ReadyPlayers.Remove(player);

            SendStateUpdate();
        }

        public bool IsGameOver()
        {
            var statistics = GameStats.CreateFrom(this);
            bool endGame = false;
            bool keepPlaying = false;

            foreach (var player in Players)
            {
                player.Result = player.Character.Victory.GetResult(statistics, player);

                endGame |= (player.Result == GameResult.WinAndEndGame || player.Result == GameResult.LoseAndEndGame);
                keepPlaying |= (player.Result == GameResult.Unknown);
            }

            return endGame || !keepPlaying;
        }
    }
}
