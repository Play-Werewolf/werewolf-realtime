using Xunit;
using System;

using WerewolfServer.Game;

namespace WerewolfServer.Tests
{
    public partial class StateTests
    {
        [Fact]
        public void TestGameInit()
        {
            TestStartGame();

            game.Time.AddOffset(new TimeSpan(0, 0, 10));
            game.DoTimer();

            Assert.True(game.State is RoleLotState);
        }
    }
}