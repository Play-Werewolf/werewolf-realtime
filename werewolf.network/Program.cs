using System;

using WerewolfServer.Game;

namespace WerewolfServer.Network
{
    class Program
    {
        // static NetworkManager manager;

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

            game.RolesBank.Add("villager");
            game.RolesBank.Add("werewolf");

            game.Timer();

            Console.WriteLine(game.State);

            Console.WriteLine(p1.Character);
            Console.WriteLine(p2.Character);
        }

        // static void RenderAll()
        // {
        //     Console.Clear();
        //     Console.WriteLine("Connections: ");
        //     foreach (var c in manager.Connections)
        //     {
        //         Console.WriteLine(c.ToString());
        //     }

        //     Console.WriteLine("\nSessions:");
        //     foreach (var s in manager.Sessions.Sessions)
        //     {
        //         Console.WriteLine(s.ToString());
        //     }

        //     Console.WriteLine("\nRooms:");
        //     foreach (var r in manager.Rooms.Games.Values)
        //     {
        //         Console.WriteLine(r.ToString());
        //     }
        // }
    }
}
