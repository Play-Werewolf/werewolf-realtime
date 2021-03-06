using System;

namespace WerewolfServer.Game
{
    public class Priest : SingleTargetCharacter
    {
        public int KillsLeft { get; set; } = 1;
        public override bool ShouldPlay => KillsLeft > 0;

        public override FortuneTellerResult FortuneTellerResult => FortuneTellerResult.Good;
        public override Alignment Alignment => Alignment.Good;
        public override NightPlayOrder NightOrder => NightPlayOrder.Priest;
        public override VictoryCalculator Victory => new TownVictory();

        public override void DoAction()
        {
            if (KillsLeft == 0)
                return;

            KillsLeft -= 1;
            Night.Action.FirstTarget.Character.Night.AddAttack(new Attack
            {
                Attacker = this,
                Target = Night.Action.FirstTarget,
                Power = Power.Basic,
                Description = "were slain by a holy priest"
            });

            if (Night.Action.FirstTarget.Character.Alignment == Alignment.Good)
            {
                Night.AddAttack(new Attack
                {
                    Attacker = null,
                    Target = this,
                    Power = Power.Basic,
                    Description = "were punished by a devine power"
                });
            }
        }
    }
}