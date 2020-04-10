namespace WerewolfServer.Game
{
    public abstract class SingleTargetCharacter : Character
    {
        public override bool ShouldAct()
        {
            return Night.Action != null && Night.Action.ShouldAct && Night.Action is UnaryAction;
        }
    }
}