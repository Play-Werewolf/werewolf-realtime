using WerewolfServer.Game;

namespace WerewolfServer.Tests
{
    class MockSession:PlaySession
    {
        public override Player CreatePlayer()
        {
            return null;
        }

        public override void EmitMessage( GameMessage msg){}

        public override bool IsOnline => true;
        public override bool IsValid => true;

    }
}