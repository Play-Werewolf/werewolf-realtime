using System;

namespace WerewolfServer.Game
{
    public class Healer : SingleTargetCharacter
    {
        public override FortuneTellerResult FortuneTellerResult => FortuneTellerResult.Good;
        public override Alignment Alignment => Alignment.Good;
        public override NightPlayOrder NightOrder => NightPlayOrder.Healer;
        public override VictoryCalculator Victory => new TownVictory();

        public override void DoAction()
        {
            Night.Action.FirstTarget.Character.Night.AddDefense(new Defense
            {
                Defender=this,
                Target=Night.Action.FirstTarget,
                Power=Power.Basic,
                Description="someone nursed you back to life",
            });
        }
    }
}
