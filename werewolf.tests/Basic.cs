using Xunit;

using WerewolfServer.Game;

namespace WerewolfServer.Tests
{
    public class BaseTests
    {
        GameRoom game;
        Player c;
        Player ww;
        Player aww;
        Player h;
        
        private void Init()
        {
            game = new GameRoom();
            c = new Player(new Villager());
            ww = new Player(new Werewolf());
            aww = new Player(new AlphaWerewolf());
            h = new Player(new Healer());

            game.Reset();
            game.AddPlayer(c);
            game.AddPlayer(ww);
            game.AddPlayer(aww);
            game.AddPlayer(h);
        }

        [Fact]
        public void TestWerewolfKill()
        {
            Init();

            game.StartNight();

            ww.Character.SetAction(new UnaryAction(h));
            aww.Character.SetAction(new UnaryAction(c));

            game.ProcessNight();

            Assert.True(ww.Character.Alive);
            Assert.True(aww.Character.Alive);
            Assert.True(h.Character.Alive);
            Assert.False(c.Character.Alive);

            Assert.Contains(c.Character.Night.Messages, m => m.Content == "You have died!");
            Assert.Contains(c.Character.Night.Messages, m => m.Content == "You were attacked by a werewolf");
        }

        [Fact]
        public void TestHealing()
        {
            Init();

            game.StartNight();

            ww.Character.SetAction(new UnaryAction(h));
            aww.Character.SetAction(new UnaryAction(c));
            h.Character.SetAction(new UnaryAction(c));
            
            game.ProcessNight();

            Assert.True(ww.Character.Alive);
            Assert.True(aww.Character.Alive);
            Assert.True(h.Character.Alive);
            Assert.True(c.Character.Alive);

            Assert.Contains(c.Character.Night.Messages, m => m.Content == "You were attacked but someone nursed you back to life");
        }

        [Fact]
        public void TestNoAttack()
        {
            Init();
            
            game.StartNight();

            game.ProcessNight();

            Assert.True(ww.Character.Alive);
            Assert.True(aww.Character.Alive);
            Assert.True(h.Character.Alive);
            Assert.True(c.Character.Alive);

            Assert.Equal(0, (int)c.Character.Night.Messages.Count);
        }

        [Fact]
        public void TestInitialAlphaWerewolfPromotion()
        {
            game = new GameRoom();
            ww = new Player(new Werewolf());
            game.AddPlayer(ww);

            game.StartNight();
            game.ProcessNight();

            Assert.True(ww.Character is AlphaWerewolf); // Test that the character was promoted
            Assert.Contains(ww.Character.Night.Messages, m => m.Content == "You have been promoted to Alpha Werewolf");
        }

        [Fact]
        public void TestNightReset()
        {
            TestHealing();
            
            game.StartNight();
            game.ProcessNight();

            Assert.Equal(0, (int)c.Character.Night.Messages.Count);
        }

        [Fact]
        public void TestAlphaWerewolfPromotionAfterDeath()
        {
            Init();

            game.StartNight();
            aww.Character.Night.AddAttack(new Attack
            {
                Attacker=null,
                Target=aww,
                Power=Power.Unstoppable,
                Description="taken by the grim reaper"
            }, addVisit: false);

            game.ProcessNight();

            Assert.Contains(aww.Character.Night.Messages, m => m.Content == "You were taken by the grim reaper");
            Assert.Contains(aww.Character.Night.Messages, m => m.Content == "You have died!");
            
            game.StartNight();
            Assert.Contains(ww.Character.Night.Messages, m => m.Content == "You have been promoted to Alpha Werewolf");
            Assert.True(ww.Character is AlphaWerewolf);
        }

        [Fact]
        public void TestOnlyOnePromotion()
        {
            game = new GameRoom();

            aww = new Player(new Werewolf());
            ww = new Player(new Werewolf());

            game.AddPlayer(aww);
            game.AddPlayer(ww);

            game.StartNight();

            Assert.NotNull(game.GetAlphaWerewolf());
            Assert.Equal(1, (int)game.GetWerewolves().Count);
        }
    }
}
