using System;
using System.Linq;
using Xunit;

using WerewolfServer.Game;

namespace werewolf.tests
{
    public class BaseTests
    {
        GameRoom game;
        Character c;
        Werewolf ww;
        AlphaWerewolf aww;
        Healer h;
        
        private void Init()
        {
            game = new GameRoom();
            c = new Character("c");
            ww = new Werewolf("ww");
            aww = new AlphaWerewolf("aww");
            h = new Healer("h");

            game.Reset();
            game.AddPlayer(new Player(c));
            game.AddPlayer(new Player(ww));
            game.AddPlayer(new Player(aww));
            game.AddPlayer(new Player(h));
        }

        [Fact]
        public void TestWerewolfKill()
        {
            Init();

            game.StartNight();

            ww.SetAction(new UnaryAction(h));
            aww.SetAction(new UnaryAction(c));
            
            game.ProcessNight();

            Assert.True(ww.Alive);
            Assert.True(aww.Alive);
            Assert.True(h.Alive);
            Assert.False(c.Alive);

            Assert.Contains(c.Night.Messages, m => m.Content == "You have died!");
            Assert.Contains(c.Night.Messages, m => m.Content == "You were attacked by a werewolf");
        }

        [Fact]
        public void TestHealing()
        {
            Init();

            game.StartNight();

            ww.SetAction(new UnaryAction(h));
            aww.SetAction(new UnaryAction(c));
            h.SetAction(new UnaryAction(c));
            
            game.ProcessNight();

            Assert.True(ww.Alive);
            Assert.True(aww.Alive);
            Assert.True(h.Alive);
            Assert.True(c.Alive);

            Assert.Contains(c.Night.Messages, m => m.Content == "You were attacked but someone nursed you back to life");
        }

        [Fact]
        public void TestNoAttack()
        {
            Init();
            
            game.StartNight();

            game.ProcessNight();

            Assert.True(ww.Alive);
            Assert.True(aww.Alive);
            Assert.True(h.Alive);
            Assert.True(c.Alive);

            Assert.Equal(0, (int)c.Night.Messages.Count);
        }
    }
}
