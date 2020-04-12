using System;

namespace WerewolfServer.Platform
{
    public static class Rand
    {
        public static Random Random { get; private set; } = new Random();
    }
}
