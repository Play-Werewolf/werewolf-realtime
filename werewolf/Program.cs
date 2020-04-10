using System;
using WerewolfServer.Game;

namespace WerewolfServer
{
    class Program
    {
        static void Main(string[] args)
        {
            GameRoom g = new GameRoom();
            Character p = new Villager("John");
            Werewolf w = new Werewolf("Lolli");
            AlphaWerewolf aw = new AlphaWerewolf("aww");
            Healer h = new Healer("Heather");

            g.AddPlayer(new Player(p));
            g.AddPlayer(new Player(w));
            g.AddPlayer(new Player(aw));
            g.AddPlayer(new Player(h));

            g.StartNight();
            aw.SetAction(new UnaryAction(p)); // Attacking player p
            h.SetAction(new UnaryAction(p));

            g.ProcessNight();
            Console.WriteLine(p);
            Console.WriteLine(w);

            foreach (var msg in p.Night.Messages)
            {
                Console.WriteLine("==> " + msg.Content);
            }
        }
    }
}
