using System;
using System.Linq;

namespace WerewolfServer.Game
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new GameRoom();
            var p1 = new Player();
            var p2 = new Player();

            game.Config.MinPlayers = 2;

            Console.WriteLine(game.State);

            game.AddPlayer(p1);
            game.AddPlayer(p2);

            game.PlayerReady(p1);
            game.PlayerReady(p2);

            game.RolesBank.Add("werewolf");
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
            Console.WriteLine(">>> " + i.ToString());

            ww.Character.SetAction(new UnaryAction(villager));

            game.Time.AddOffset(new TimeSpan(0, 0, 29));
            game.Timer();

            Console.WriteLine(game.State);

            game.Time.AddOffset(new TimeSpan(0, 0, 3));
            game.Timer();

            Console.WriteLine(game.State);
        }
    }
}
