using System;

namespace WerewolfServer.Platform
{
    public class TimeProvider
    {
        public TimeSpan Offset { get; set; }
        public DateTime Now => DateTime.Now + Offset;

        public TimeProvider(TimeSpan offset)
        {
            Offset = offset;
        }

        public TimeProvider()
        {
            Offset = TimeSpan.Zero;
        }

        public void AddOffset(TimeSpan time)
        {
            Offset += time;
        }
    }
}
