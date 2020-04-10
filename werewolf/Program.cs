using System;
using WerewolfServer.Game;

namespace WerewolfServer
{
    class Program
    {
        static void Main(string[] args)
        {
            GameRoom g = new GameRoom();

            Player p = new Player(new Villager().WithName("John"));
            Player w = new Player(new Werewolf().WithName("Lolli"));
            Player h = new Player(new Priest().WithName("Heather"));
            Player ft = new Player(new FortuneTeller().WithName("ft"));

            g.AddPlayer(p);
            g.AddPlayer(w);
            g.AddPlayer(h);
            g.AddPlayer(ft);

            g.StartNight();
            w.Character.SetAction(new UnaryAction(ft));
            h.Character.SetAction(new UnaryAction(p));

            g.ProcessNight();
            
            foreach (var player in g.Players)
            {
                Console.WriteLine(player.Character);
            }

            Console.WriteLine("=====\nLogs:");
            foreach (var msg in g.MakeDeathLog())
            {
                Console.WriteLine("==> " + msg);
            }
        }
    }
}
