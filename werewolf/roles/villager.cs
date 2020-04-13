using System;

namespace WerewolfServer.Game
{
    public class Villager : Character
    {
        public override FortuneTellerResult FortuneTellerResult => FortuneTellerResult.Good;
        public override Alignment Alignment => Alignment.Good;
        public override NightPlayOrder NightOrder => NightPlayOrder.NoPriority;
        public override VictoryCalculator Victory => new TownVictory();
    }
}
