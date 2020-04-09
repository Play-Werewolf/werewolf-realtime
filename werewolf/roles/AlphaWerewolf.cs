using System;
using System.Linq;
using System.Collections.Generic;

namespace WerewolfServer.Game
{
    public class AlphaWerewolf : SingleTargetCharacter
    {
        Player Attacker;

        public AlphaWerewolf(string name) : base(name) {}

        public override Power BaseDefense => Power.Basic;

        public override void OnNightStart()
        {
            var ww = Game.GetWerewolves();

            if (ww.Count == 0)
            {
                Attacker = this.Player;
            }
            else
            {
                Attacker = ww[Game.Random.Next(ww.Count)];
            }
        }

        public override void SetAction(NightAction action)
        {
            // Setting the attacker action instead of this action
            Attacker.Character.Night.Action = action;
        }

        public override void DoAction()
        {
            Night.Action.FirstTarget.Character.Night.AddAttack(new Attack
            {
                Attacker=this,
                Target=Night.Action.FirstTarget,
                Power=Power.Basic,
                Description="attacked by a werewolf",
            });
        }
    }

    public static class AlphaWerewolfExtensions
    {
        public static AlphaWerewolf GetAlphaWerewolf(this GameRoom game)
        {
            var aww = game.Players
                .Where(p => p.Character.Alive && p.Character is AlphaWerewolf)
                .ToList();

            if (aww.Count > 1)
            {
                throw new InvalidProgramException("Game cannot contain more than one Alpha Werewolf");
            }

            return aww.FirstOrDefault().Character as AlphaWerewolf;
        }

        public static List<Werewolf> GetWerewolves(this GameRoom game)
        {
            return game.Players
                .Where(p => p.Character is Werewolf)
                .Select(p => p.Character as Werewolf)
                .ToList();
        }
    }
}
