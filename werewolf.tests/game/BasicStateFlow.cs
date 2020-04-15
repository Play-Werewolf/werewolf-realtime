using Xunit;
using Xunit.Abstractions;
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

        private readonly ITestOutputHelper output;

        public StateTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private void Init()
        {
            game = new GameRoom("", null);
            villager1 = new Player(new Villager(), new MockSession());
            villager2 = new Player(new Villager(), new MockSession());
            aww = new Player(new Werewolf(), new MockSession());
            ww = new Player(new AlphaWerewolf(), new MockSession());
            healer = new Player(new Healer(), new MockSession());

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

            Assert.Equal("Werewolves", game.NightPlayOrders[0].ToString());
            Assert.Equal("Healer", game.NightPlayOrders[1].ToString());

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

         [Fact]
         public void TestConcurrentNightToDayTransitionState()
         {
             TestNightTransitionToConcurrentNightState();

            aww.Character.SetAction(new UnaryAction(villager1));
            healer.Character.SetAction(new UnaryAction(villager1));

            game.Time.AddOffset(new TimeSpan(0, 0, 59));
            game.Timer();

            game.Time.AddOffset(new TimeSpan(0, 0, 3));
            game.Timer();

            Assert.True(game.State is DayTransitionState);
        }

        [Fact]
        public void TestDayTransitionToDeathAnnounceState()
        {
            TestSeparatedNightToDayTransitionState();

            game.Time.AddOffset(new TimeSpan(0, 0, 10));
            game.Timer();

            
            Assert.True(game.State is DeathAnnounceState);
        }

        [Fact]
        public void TestDeathAnnounceToGameOverState()
        {
            TestDayTransitionToDeathAnnounceState();

            aww.Character.Die();
            ww.Character.Die();

            game.Time.AddOffset(new TimeSpan(0, 0, 10));
            game.Timer();

            Assert.True(game.State is GameOverState);
        }

        [Fact]
        public void TestDeathAnnounceToDiscussionState()
        {
            TestDayTransitionToDeathAnnounceState();

            Assert.True(game.Players.Where(p => { return p.Character.Alive; }).ToArray().Length == 5);

            game.Time.AddOffset(new TimeSpan(0, 0, 10));
            game.Timer();
            Assert.True(game.State is DiscussionState);
        }

        [Fact]
        public void TestDiscussionToNightTransitionState()
        {
            TestDeathAnnounceToDiscussionState();

            game.Time.AddOffset(new TimeSpan(0, 0, 151));
            game.Timer();


            Assert.True(game.State is NightTransitionState);
        }

        [Fact]
        public void TestDiscussionToTrialState()
        {
            TestDeathAnnounceToDiscussionState();

            healer.VoteToKill(aww);
            villager1.VoteToKill(aww);
            villager2.VoteToKill(aww);

            game.Time.AddOffset(new TimeSpan(0, 0, 60));
            game.Timer();

            Assert.True(game.State is TrialState);

        }

        [Fact]
        public void TestTrialToVotingState()
        {
            TestDiscussionToTrialState();

            game.Time.AddOffset(new TimeSpan(0, 0, 31));
            game.Timer();

            Assert.True(game.State is VotingState);

        }

        [Fact]
        public void TestVotingToGuiltyVoteShowState()
        {
            TestTrialToVotingState();

            healer.VoteGuilty();
            villager1.VoteGuilty();
            villager2.VoteGuilty();
            ww.VoteInnocent();

            game.Time.AddOffset(new TimeSpan(0, 0, 31));
            game.Timer();

            Assert.True(game.State is VoteShowState);

        }

        [Fact]
        public void TestVotingToInnocentVoteShowState()
        {
            TestTrialToVotingState();

            healer.VoteInnocent();
            villager1.VoteInnocent();
            villager2.VoteInnocent();
            ww.VoteInnocent();

            game.Time.AddOffset(new TimeSpan(0, 0, 31));
            game.Timer();

            Assert.True(game.State is VoteShowState);

        }

        [Fact]
        public void TestVotingToDiscussionState()
        {
            TestVotingToInnocentVoteShowState();

            game.Time.AddOffset(new TimeSpan(0, 0, 11));
            game.Timer();

            Assert.True(game.State is DiscussionState);

        }

        [Fact]
        public void TestVotingToExecutionState()
        {
            TestVotingToGuiltyVoteShowState();

            game.Time.AddOffset(new TimeSpan(0, 0, 11));
            game.Timer();

            Assert.True(game.State is ExecutionState);

        }

        [Fact]
        public void TestExecutionToNightTransitionState()
        {
            TestVotingToExecutionState();

            game.Time.AddOffset(new TimeSpan(0, 0, 31));
            game.Timer();

            Assert.True(game.State is NightTransitionState);

        }

        [Fact]
        public void TestExecutionToGameOverState()
        {
            TestVotingToExecutionState();

            ww.Character.Die();
            game.Time.AddOffset(new TimeSpan(0, 0, 31));
            game.Timer();

            Assert.True(game.State is GameOverState);

        }

        [Fact]
        public void TestGameOverToLobbyState()
        {
            TestExecutionToGameOverState();

            game.Time.AddOffset(new TimeSpan(0, 0, 31));
            game.Timer();

            Assert.True(game.State is LobbyState);

        }

    }
}