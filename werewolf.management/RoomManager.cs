using System;
using System.Linq;
using System.Collections.Generic;

using WerewolfServer.Game;

namespace WerewolfServer.Management
{
    public class RoomManager
    {
        Random random {get;} = new Random();
        public Dictionary<string, GameRoom> Games;
        
        public RoomManager()
        {
            Games = new Dictionary<string, GameRoom>();
        }

        public void CleanupStaleGames()
        {
            foreach (var key in Games.Keys)
            {
                if (Games[key].Players.Where(p => p.IsConnected).Count() != 0)
                    continue;

                Games.Remove(key);
                Console.WriteLine("Closed room {0}", key);
            }
        }

        public string GenerateID()
        {
            string id;

            do
            {
                if (Games.Count > 5000)
                    id = random.Next(100000, 1000000).ToString();
                else
                    id = random.Next(1000, 10000).ToString();
            } while (Games.Keys.Contains(id));

            return id;
        }

        public GameRoom CreateGame(IGameUpdater updater)
        {
            string id = GenerateID();
            Games[id] = new GameRoom(id, updater);
            return Games[id];
        }
    }
}
