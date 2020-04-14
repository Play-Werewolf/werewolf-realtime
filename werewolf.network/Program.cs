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
            while (true)
            {
                RenderAll();
                // Console.ReadLine();
                System.Threading.Thread.Sleep(500);
                manager.DoCleanup();
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
