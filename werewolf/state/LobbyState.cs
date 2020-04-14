using System;
using System.Linq;
using System.Collections.Generic;

namespace WerewolfServer.Game
{
    public class LobbyState : GameState
    {
        public LobbyState(GameRoom game) : base(game) { }

        public override void OnStart()
        {
            if (Game.ReadyPlayers == null)
                Game.ReadyPlayers = new List<Player>();

            Game.ReadyPlayers.Clear();

            Game.Players.RemoveAll(player => !player.IsOnline);
        }

        public override void OnTimer(float timeDelta)
        {
            if (Game.ReadyPlayers.Count == Game.Players.Count
                && Game.Players.Count >= Game.Config.MinPlayers
                && Game.Players.Count <= Game.RolesBank.Count)
            {
                ChangeState(new GameInitState(Game));
            }
        }

        public override void OnEnd()
        {
            try
            {
                RoleGenerator_ x = new RoleGenerator_();
                var characters = x.GenerateTown(Game.RolesBank.ToArray());
                Game.InitGame(characters);
            }
            catch (Exception)
            {
                KeepState();
                Game.ReadyPlayers.Remove(Game.Players[0]);
                // TODO: Send state update for failure
            }
        }

        public override object Serialize()
        {
            return new
            {
                State = GetType().Name,
                ReadyPlayers = Game.ReadyPlayers.Select(p => p.Id).ToArray(),
            };
        }
    }
}
