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
            Player h = new Player(new Healer().WithName("Heather"));
            Player ft = new Player(new FortuneTeller().WithName("ft"));

            g.AddPlayer(p);
            g.AddPlayer(w);
            // g.AddPlayer(aw);
            g.AddPlayer(h);
            g.AddPlayer(ft);

            g.StartNight();
            w.Character.SetAction(new UnaryAction(ft));
            // aw.SetAction(new UnaryAction(p)); // Attacking player p
            // ft.SetAction(new UnaryAction(aw));

            g.ProcessNight();
            
            foreach (var player in g.Players)
            {
                Console.WriteLine(player.Character);
            }

            Console.WriteLine("=====\nLogs:");
            foreach (var msg in w.Character.Night.Messages)
            {
                Console.WriteLine("==> " + msg.Content);
            }
        }
    }
}
