using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Fleck;

using WerewolfServer.Management;

namespace WerewolfServer.Network
{

    class NetworkConnection
    {
        NetworkManager Manager;
        IWebSocketConnection Socket;

        Dictionary<string, Action<NetworkMessage>> handlers;

        public NetworkConnection(NetworkManager manager, IWebSocketConnection connection)
        {
            Console.WriteLine("A new network connection was opened");
            Manager = manager;
            Socket = connection;

            handlers = new Dictionary<string, Action<NetworkMessage>>()
            {
                ["echo"] = Echo,
            };

            Socket.OnMessage = (msg) =>
            {
                var message = new NetworkMessage(msg);
                if (handlers.ContainsKey(message.Type))
                {
                    handlers[message.Type](message);
                }
            };
        }

        public void Echo(NetworkMessage message)
        {
            Console.WriteLine("Echo message");
            Socket.Send(new NetworkMessage("echo_reply", message.Args.ToArray()).Compile());
        }
    }
}
