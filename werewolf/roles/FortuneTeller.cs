namespace WerewolfServer.Game
{
    public enum FortuneTellerResult
    {
        Good,
        Evil,
        Unknown
    }

    public class FortuneTeller : SingleTargetCharacter
    {
        public FortuneTeller(string name) : base(name) {}
        public override FortuneTellerResult FortuneTellerResult => FortuneTellerResult.Good;

        public override void DoAction()
        {
            Night.Action.FirstTarget.Character.Night.AddVisit(this);
        }

        public override void PostAction()
        {
            SendMessage("Your target seems " + Night.Action.FirstTarget.Character.FortuneTellerResult.GetResultString());
        }
    }

    public static class FortuneTellerExtensions
    {
        public static string GetResultString(this FortuneTellerResult result)
        {
            switch (result)
            {
            case FortuneTellerResult.Evil:
                return "Your target seems evil!";
            case FortuneTellerResult.Good:
                return "Your target seems innocent";
            default:
                return "You could not decide wether your target is good or bad";
            }
        }
    }
}
