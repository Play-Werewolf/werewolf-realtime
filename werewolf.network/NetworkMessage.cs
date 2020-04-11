using System;
using System.Collections.Generic;
using System.Text;

namespace WerewolfServer.Network
{
    class NetworkMessage
    {
        string[] parts;

        public string Type => parts[0];
        public IEnumerable<string> Args
        {
            get
            {
                for (int i = 1; i < parts.Length; i++)
                    yield return parts[i];
            }
        }

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

        public string Compile()
        {
            return String.Join('\x00', parts);
        }
    }
}
