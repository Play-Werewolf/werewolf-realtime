using System;
using System.Linq;
using System.Collections.Generic;

using WerewolfServer.Platform;

namespace WerewolfServer.Game
{
    public class AlphaWerewolf : SingleTargetCharacter
    {
        public Player Attacker;

        public override Power BaseDefense => Power.Basic;
        public override FortuneTellerResult FortuneTellerResult => FortuneTellerResult.Good;
        public override Alignment Alignment => Alignment.Evil;
        public override NightPlayOrder NightOrder => NightPlayOrder.Werewolves;
        public override VictoryCalculator Victory => new WerewolfVictory();

        public override void PreAction()
        {
            // Setting the attacker action instead of this action

            var ww = Player.Game.GetWerewolves();

            if (ww.Count == 0)
            {
                Attacker = this.Player;
            }
            else
            {
                Attacker = ww[Rand.Random.Next(ww.Count)];
                Attacker.Character.Night.Action = Night.Action;
                Night.Action = NightAction.Empty;
            }
        }

        public override void DoAction()
        {
            Night.Action.FirstTarget.Character.Night.AddAttack(new Attack
            {
                Attacker=this,
                Target=Night.Action.FirstTarget,
                Power=Power.Basic,
                Description="attacked by a werewolf",
            }, addVisit: true);
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

            if (aww.Count == 0)
            {
                return null;
            }

            return aww.First().Character as AlphaWerewolf;
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
