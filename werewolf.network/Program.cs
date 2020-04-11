using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Fleck;

using WerewolfServer.Management;
using WerewolfServer.Game;

namespace WerewolfServer.Network
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkManager manager = new NetworkManager();
            manager.Start();
            Console.ReadLine();
        }
    }
}
