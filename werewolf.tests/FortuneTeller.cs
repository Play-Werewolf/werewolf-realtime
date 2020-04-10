using Xunit;
using System;

using WerewolfServer.Game;

namespace WerewolfServer.Tests
{
    public class FortuneTellerTests
    {
        GameRoom game;

        Player teller;
        Player alphaWerewolf;
        Player player;

        void Init()
        {
            game = new GameRoom();
            teller = new Player(new FortuneTeller());
            alphaWerewolf = new Player(new AlphaWerewolf());
            game.AddPlayer(teller);
            game.AddPlayer(alphaWerewolf);
        }

        [Fact]
        public void SeesVillagerAsGood()
        {
            Init();
            player = new Player(new Villager());
            game.AddPlayer(player);

            game.StartNight();
            teller.Character.SetAction(new UnaryAction(player));
            game.ProcessNight();

            Assert.Contains(teller.Character.Night.Messages, m => m.Content == "Your target seems innocent");
        }

        [Fact]
        public void SeesWerewolfAsEvil()
        {
            Init();
            player = new Player(new Werewolf());
            game.AddPlayer(player);

            game.StartNight();
            teller.Character.SetAction(new UnaryAction(player));
            game.ProcessNight();

            Assert.Contains(teller.Character.Night.Messages, m => m.Content == "Your target seems evil!");
        }

        [Fact]
        public void SeesAlphaAsGood()
        {
            Init();

            game.StartNight();
            teller.Character.SetAction(new UnaryAction(alphaWerewolf));
            game.ProcessNight();

            Assert.Contains(teller.Character.Night.Messages, m => m.Content == "Your target seems innocent");
        }
    }
}