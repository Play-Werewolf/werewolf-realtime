using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using WerewolfServer.Game;

namespace WerewolfServer.Display
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new GameRoom(new ConsoleJsonGameUpdater());
            
            var p1 = new Player("", "George", "");
            var p2 = new Player("", "Amitmeat", "");
            var p3 = new Player("", "Jerome", "");
            var p4 = new Player("", "Ahmad", "");

            game.Config.MinPlayers = 2;

            Console.WriteLine(game.State);

            game.AddPlayer(p1);
            game.AddPlayer(p2);
            game.AddPlayer(p3);
            game.AddPlayer(p4);

            game.PlayerReady(p1);
            game.PlayerReady(p2);
            game.PlayerReady(p3);
            game.PlayerReady(p4);

            game.RolesBank.Add("werewolf");
            game.RolesBank.Add("villager");
            game.RolesBank.Add("villager");
            game.RolesBank.Add("villager");

            game.Timer();

            Console.WriteLine(game.State);

            Console.WriteLine(p1.Character);
            Console.WriteLine(p2.Character);

            game.Time.AddOffset(new TimeSpan(0, 0, 5));
            game.Timer();

            Console.WriteLine(game.State);

            var ww = game.Players.Where(p => p.Character is Werewolf).First();
            var villager = game.Players.Where(p => p.Character is Villager).First();

            game.Time.AddOffset(new TimeSpan(0, 0, 10));
            game.Timer();

            Console.WriteLine(game.State);

            game.Time.AddOffset(new TimeSpan(0, 0, 5));
            game.Timer();

            Console.WriteLine(game.State);

            foreach (var i in game.NightPlayOrders)

                ww.Character.SetAction(new UnaryAction(villager));

            game.Time.AddOffset(new TimeSpan(0, 0, 29));
            game.Timer();

            Console.WriteLine(game.State);

            game.Time.AddOffset(new TimeSpan(0, 0, 3));
            game.Timer();

            Console.WriteLine(game.State);

            game.Time.AddOffset(new TimeSpan(0, 0, 5));
            game.Timer();

            Console.WriteLine(game.State);

            for (int i = 0; i < 5; i++)
            {
                game.Time.AddOffset(new TimeSpan(0, 0, 10));
                game.Timer();
            }

            Console.WriteLine(game.State);

            foreach (var player in game.Players)
            {
                player.VoteToKill(ww);
            }

            game.Timer();
            Console.WriteLine(game.State);

            game.Time.AddOffset(new TimeSpan(0, 0, 30));
            game.Timer();

            Console.WriteLine(game.State);

            foreach (var player in game.Players.Where(p => p.Character is Villager && p.Character.Alive))
            {
                player.VoteGuilty();
            }

            game.Time.AddOffset(new TimeSpan(0, 0, 30));
            game.Timer();
            Console.WriteLine(game.State);

            game.Time.AddOffset(new TimeSpan(0, 0, 10));
            game.Timer();
            Console.WriteLine(game.State);

            game.Time.AddOffset(new TimeSpan(0, 0, 10));
            game.Timer();
            Console.WriteLine(game.State);
        }
    }
}
