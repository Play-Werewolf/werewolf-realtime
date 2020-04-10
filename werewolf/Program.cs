using System;
using WerewolfServer.Game;

namespace WerewolfServer
{
    class Program
    {
        static void Main(string[] args)
        {
            GameRoom g = new GameRoom();
            g.Config.MinPlayers = 1;

            Player a = new Player();
            Player b = new Player();

            g.HandleCommand(new GameCommand
            {
                Type = CommandType.UserJoin,
                Sender = a
            });

            Console.WriteLine(g.Players.Count);
        }
    }
}
