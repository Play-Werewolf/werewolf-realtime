using System;
using System.Collections.Generic;
using System.Text;

namespace WerewolfServer.Game
{
    public abstract class GameMessage
    {
        public abstract string Type { get; }
        public abstract string[] Args { get; }

        public abstract string Compile();
    }
}
