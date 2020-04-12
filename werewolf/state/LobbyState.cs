using System;
using System.Collections.Generic;

namespace WerewolfServer.Game
{
    public class LobbyState : GameState
    {
        public LobbyState(GameRoom game) : base(game) {}

        public override void OnStart()
        {
            if (Game.ReadyPlayers == null)
                Game.ReadyPlayers = new List<Player>();

            Game.ReadyPlayers.Clear();
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

                for (var i = 0; i < characters.Length; i++)
                {
                    Game.Players[i].AttachCharacter(characters[i]);
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Exception: {0}", ex);
                KeepState();
                // TODO: Send state update for failure
            }
        }
    }
}
