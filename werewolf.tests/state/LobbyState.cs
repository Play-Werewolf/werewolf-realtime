using Xunit;

using WerewolfServer.Game;

namespace WerewolfServer.Tests
{
    public partial class StateTests
    {
        public GameRoom game;
        public Player a;
        public Player b;

        void Init()
        {
            game = new GameRoom();
            game.Config.MinPlayers = 2;
            a = new Player();
            b = new Player();

            Assert.True(game.State is LobbyState);
        }

        [Fact]
        public void TestUserJoinLobby()
        {
            Init();

            game.HandleCommand(new GameCommand
            {
                Type = CommandType.UserJoin,
                Sender = a,
            });

            game.HandleCommand(new GameCommand
            {
                Type = CommandType.UserJoin,
                Sender = b,
            });

            Assert.Equal(2, (int)game.Players.Count);
        }

        [Fact]
        public void TestUserLeaveLobby()
        {
            TestUserJoinLobby();
            game.HandleCommand(new GameCommand
            {
                Type = CommandType.UserLeave,
                Sender = a,
            });

            Assert.Equal(1, (int)game.Players.Count);
        }

        [Fact]
        public void TestDoubleJoin()
        {
            TestUserJoinLobby();
            game.HandleCommand(new GameCommand
            {
                Type = CommandType.UserJoin,
                Sender = a,
            });

            Assert.Equal(2, (int)game.Players.Count);
        }

        [Fact]
        public void TestUserReady()
        {
            TestUserJoinLobby();
            game.HandleCommand(new GameCommand
            {
                Type = CommandType.UserReady,
                Sender = a,
            });

            Assert.Equal(1, (int)(game.State as LobbyState).ReadyPlayers.Count);
        }

        [Fact]
        public void TestLoggedOutUserReady()
        {
            TestUserLeaveLobby();
            game.HandleCommand(new GameCommand
            {
                Type = CommandType.UserReady,
                Sender = a,
            });

            Assert.Equal(0, (int)(game.State as LobbyState).ReadyPlayers.Count);
        }

        [Fact]
        public void TestStartGame()
        {
            TestUserReady();
            game.HandleCommand(new GameCommand
            {
                Type = CommandType.UserReady,
                Sender = b,
            });

            Assert.True(game.State is GameInitState);
        }
    }
}