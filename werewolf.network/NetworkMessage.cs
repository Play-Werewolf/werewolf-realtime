using System;
using System.Collections.Generic;
using System.Linq;

using WerewolfServer.Game;

namespace WerewolfServer.Network
{
    public class NetworkMessage : GameMessage
    {
        string[] parts;

        public IEnumerable<string> IterArgs
        {
            get
            {
                for (int i = 1; i < parts.Length; i++)
                    yield return parts[i];
            }
        }

        public override string Type => parts[0];
        public override string[] Args => IterArgs.ToArray();

        public NetworkMessage(string raw)
        {
            parts = raw.Split(new char[] { '\x00' });
        }

        public NetworkMessage(string type, string[] args)
        {
            parts = new string[args.Length + 1];
            parts[0] = type;
            args.CopyTo(parts, 1);
        }

        public override string Compile()
        {
            return String.Join('\x00', parts);
        }
    }
}
