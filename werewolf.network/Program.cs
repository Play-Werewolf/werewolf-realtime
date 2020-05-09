using System;

namespace WerewolfServer.Network
{
    class Program
    {
        static NetworkManager manager;

        static void Main(string[] args)
        {
            manager = new NetworkManager();
            manager.Start();

#if DEBUG
            new System.Threading.Thread(DebugStuff)
            {
                IsBackground=true
            }.Start();
#endif

            manager.WorkSingleThreadedly();
        }

        static void DebugStuff()
        {
            while (true)
            {
                Console.Write("> ");
                string nextCommand = Console.ReadLine();

                foreach (var g in manager.Rooms.Games)
                {
                    Command(g.Value, nextCommand.Split(' '));
                }
            }
        }

        static void Command(Game.GameRoom g, string[] args)
        {
            switch (args[0])
            {
            case "a":
                g.State = new Game.DeathAnnounceState(g)
                {
                    Callouts = new System.Collections.Generic.List<string> { "First callout", "Second callout", "Sukka blyat", "Third callout"}
                };
                break;
            }
        }

        static void RenderAll()
        {
            Console.Clear();
            Console.WriteLine("Connections: ");
            foreach (var c in manager.Connections)
            {
                Console.WriteLine(c.ToString());
            }

            Console.WriteLine("\nSessions:");
            foreach (var s in manager.Sessions.Sessions)
            {
                Console.WriteLine(s.ToString());
            }

            Console.WriteLine("\nRooms:");
            foreach (var r in manager.Rooms.Games.Values)
            {
                Console.WriteLine(r.ToString());
            }
            Console.WriteLine("----------------------------");
        }
    }
}
