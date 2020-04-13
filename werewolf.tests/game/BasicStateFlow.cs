using Xunit;
using System;
using System.Linq;
using WerewolfServer.Game;

namespace WerewolfServer.Tests
{
    public class StateTests
    {
        GameRoom game;
        Player villager1;
        Player villager2;
        Player aww;
        Player ww;
        Player healer;
        
        private void Init()
        {
            game = new GameRoom();
            villager1 = new Player(new Villager());
            villager2 = new Player(new Villager());
            aww = new Player(new Werewolf());
            ww = new Player(new AlphaWerewolf());
            healer = new Player(new Healer());

            game.Reset();
            game.AddPlayer(villager1);
            game.AddPlayer(villager2);
            game.AddPlayer(aww);
            game.AddPlayer(ww);
            game.AddPlayer(healer);
        }

        [Fact]
        public void TestLobbyToInitState()
        {
            Init();

            Assert.True(game.State is LobbyState);

            game.PlayerReady(villager1);
            game.PlayerReady(villager2);
            game.PlayerReady(aww);
            game.PlayerReady(ww);
            game.PlayerReady(healer);

            game.RolesBank.Add("villager");
            game.RolesBank.Add("villager");
            game.RolesBank.Add("werewolf");
            game.RolesBank.Add("alpha_werewolf");
            game.RolesBank.Add("healer");

            game.Timer();
            Assert.True(game.State is GameInitState);

        }

        [Fact]
        public void TestInitToRolesState()
        {
            TestLobbyToInitState();

            Assert.False(villager1.Character == null);
            Assert.False(villager2.Character == null);
            Assert.False(ww.Character == null);
            Assert.False(aww.Character == null);
            Assert.False(healer.Character == null);

            game.Time.AddOffset(new TimeSpan(0, 0, 5));
            game.Timer();

            Assert.True(game.State is RolesLotState);

        }

        [Fact]
        public void TestRolesToNightTransitionState()
        {
            TestInitToRolesState();
            villager1 = game.Players.Where(p => p.Character is Villager).First();
            villager2 = game.Players.Where(p => p.Character is Villager).ElementAt(1);
            aww = game.Players.Where(p => p.Character is Werewolf).First();
            ww = game.Players.Where(p => p.Character is AlphaWerewolf).First();
            healer = game.Players.Where(p => p.Character is Healer).First();

            game.Time.AddOffset(new TimeSpan(0, 0, 10));
            game.Timer();

            Assert.True(game.State is NightTransitionState);
        }

        [Fact]
        public void TestNightTransitionToSeparatedNightState()
        {
            TestRolesToNightTransitionState();

            game.Time.AddOffset(new TimeSpan(0, 0, 5));
            game.Timer();

            Assert.True(game.State is SeparatedNightState);
        }

        [Fact]
        public void TestNightTransitionToConcurrentNightState()
        {
            TestRolesToNightTransitionState();
            game.Config.ConcurrentNight = true;

            game.Time.AddOffset(new TimeSpan(0, 0, 5));
            game.Timer();

            Assert.True(game.State is ConcurrentNightState);
        }

        [Fact]
        public void TestSeparatedNightToDayTransitionState()
        {
            TestNightTransitionToSeparatedNightState();

            Assert.Equal(game.NightPlayOrders[0].ToString(), "Werewolves");
            Assert.Equal(game.NightPlayOrders[1].ToString(), "Healer");

            aww.Character.SetAction(new UnaryAction(villager1));
            game.Time.AddOffset(new TimeSpan(0, 0, 59));
            game.Timer();

            game.Time.AddOffset(new TimeSpan(0, 0, 3));
            game.Timer();

            healer.Character.SetAction(new UnaryAction(villager1));
            game.Time.AddOffset(new TimeSpan(0, 0, 20));
            game.Timer();

            game.Time.AddOffset(new TimeSpan(0, 0, 3));
            game.Timer();

            Assert.True(game.State is DayTransitionState);
        }

        // [Fact]
        // public void TestConcurrentNightToDayTransitionState()
        // {
        //     TestNightTransitionToConcurrentNightState();

        //     Assert.Equal(game.NightPlayOrders[2].ToString(), "Healer");
        //     Assert.Equal(game.NightPlayOrders[0].ToString(), "Werewolves");

        //     aww.Character.SetAction(new UnaryAction(villager1));
        //     healer.Character.SetAction(new UnaryAction(villager1));

        //     game.Time.AddOffset(new TimeSpan(0, 0, 59));
        //     game.Timer();

        //     Assert.True(game.State is DayTransitionState);
        // }

    }
}