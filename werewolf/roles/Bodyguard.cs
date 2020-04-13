using System;

namespace WerewolfServer.Game
{
    // TODO: Test
    public class Bodyguard : SingleTargetCharacter
    {
        public override FortuneTellerResult FortuneTellerResult => FortuneTellerResult.Good;
        public override Alignment Alignment => Alignment.Good;
        public override NightPlayOrder NightOrder => NightPlayOrder.BodyGuard;
        public override VictoryCalculator Victory => new TownVictory();

        public override void DoAction()
        {
            Night.Action.FirstTarget.Character.Night.AddDefense(new Defense
            {
                Defender = this,
                Target = Night.Action.FirstTarget,
                Power = Power.Basic,
                Description = "someone fought off your attacker",
            });
        }

        public override void PostAction()
        {
            var attacks = Night.Action.FirstTarget.Character.Night.Attacks;
            if (attacks.Count > 0)
            {
                foreach (var a in attacks)
                {
                    if (Night.Action.FirstTarget.Character.Night.VisitedBy.Contains(a.Attacker))
                    {
                        Night.AddAttack(new Attack
                        {
                            Attacker = a.Attacker,
                            Target = this,
                            Power = a.Power,
                            Description = a.Description,
                        }, addVisit: false);
                    }
                }
            }
        }
    }
}