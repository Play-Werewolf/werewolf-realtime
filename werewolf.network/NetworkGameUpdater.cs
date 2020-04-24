using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using WerewolfServer.Game;

namespace WerewolfServer.Network
{
    public class _Player
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("avatar_url")] public string AvatarURL { get; set; }
        [JsonPropertyName("role")] public string Role { get; set; }
        //[JsonPropertyName("votes_against")] public string VotesAgainst { get; set; }
        //[JsonPropertyName("votes")] public int Votes { get; set; }
        //[JsonPropertyName("trial_vote")] public bool? TrialVote { get; set; }
        //[JsonPropertyName("result")] public string Result { get; set; }

        [JsonPropertyName("alive")] public bool? Alive { get; set; }
        [JsonPropertyName("death_night")] public int? DeathNight { get; set; }

        public _Player(Player player, bool censor)
        {
            Id = player.Id;
            Name = player.Name;
            AvatarURL = player.AvatarURL;
            //VotesAgainst = player.VotesAgainst?.Id;
            //Votes = player.Votes;
            //TrialVote = player.TrialVote;
            Alive = player.Character?.Alive;
            DeathNight = player.Character?.DeathNight;
        }

        public _Player(Player player)
            : this(player, true)
        {
            Role = player.Character?.GetType().Name; // TODO: Is this good?
            //Result = player.Result.ToString();
        }
    }

    public class _Game
    {
        [JsonPropertyName("current_night")] public int CurrentNight { get; set; }
        [JsonPropertyName("players")] public List<_Player> Players { get; set; }
        [JsonPropertyName("ready_players")] public List<string> ReadyPlayers { get; set; }
        [JsonPropertyName("player_on_stand")] public string PlayerOnStand { get; set; }
        [JsonPropertyName("roles_bank")] public List<string> RolesBank { get; set; }

        public _Game(GameRoom game)
        {
            CurrentNight = game.CurrentNight;
            Players = game.Players.Select(p => new _Player(p, censor: true)).ToList();
            ReadyPlayers = game.ReadyPlayers.Select(x => x.Id).ToList();
            PlayerOnStand = game.PlayerOnStand?.Id;
            RolesBank = game.RolesBank;
        }
    }

    class NetworkGameUpdater : IGameUpdater
    {
        public static JsonSerializerOptions options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            IgnoreNullValues = false,
            MaxDepth = 10,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public void SendGameUpdate(GameRoom room, IEnumerable<Player> targets = null)
        {
            var g = new _Game(room);
            var json = JsonSerializer.Serialize(g, g.GetType(), options);
            var m = new NetworkMessage("game_update", new[] { json });

            foreach (var player in targets ?? room.Players)
            {
                player.Session?.EmitMessage(m);
            }
        }

        public void SendPlayerUpdate(Player player, IEnumerable<Player> targets = null)
        {
            var p = new _Player(player);
            var json = JsonSerializer.Serialize(p, p.GetType(), options);
            var m = new NetworkMessage("player_update", new[] { json });

            if (targets == null)
            {
                player.Session?.EmitMessage(m);
                return;
            }

            foreach (var target in targets)
            {
                target.Session?.EmitMessage(m);
            }
        }

        public void SendStateUpdate(GameRoom game, IEnumerable<Player> targets = null)
        {
            var o = game.State.Serialize();
            var json = JsonSerializer.Serialize(o, o.GetType(), options);
            var m = new NetworkMessage("state_update", new[] { json });

            foreach (var player in targets ?? game.Players)
            {
                player.Session?.EmitMessage(m);
            }
        }

        public void SendTimerUpdate(float time, IEnumerable<Player> targets = null)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(Message message, IEnumerable<Player> targets = null)
        {
            if (targets == null)
                return;

            foreach (var player in targets)
            {
                player.Session?.EmitMessage(new NetworkMessage("message", new[] { message.Content }));
            }
        }
    }
}
