namespace WerewolfServer.Game
{
    public class Healer : Character
    {
        public Healer(string name) : base(name) {}

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
