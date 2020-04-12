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
                manager.CleanupSessions();
                // RenderAll();
                System.Threading.Thread.Sleep(500);
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
        }
    }
}
