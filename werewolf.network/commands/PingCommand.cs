namespace WerewolfServer.Network
{
    public class PingCommand : BaseCommand
    {
        public override string CommandType => "ping";

        public override void OnCommand()
        {
            sender.Send(messageType: "pong");
        }
    }
}
