namespace WerewolfServer.Game
{
    public class Villager : Character
    {
        public Villager(string name) : base(name) {}

        public override FortuneTellerResult FortuneTellerResult
            => FortuneTellerResult.Good;
    }
}
