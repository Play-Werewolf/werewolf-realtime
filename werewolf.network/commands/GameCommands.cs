using System;
using System.Collections.Generic;
using System.Text;

namespace WerewolfServer.Network
{
    class ReadyCommand : BaseCommand
    {
        public override string CommandType => "ready";

        public override void OnCommand()
        {
            sender.Session?.Player?.Game?.PlayerReady(sender.Session?.Player);
        }
    }

    class NotReadyCommand : BaseCommand
    {
        public override string CommandType => "not_ready";

        public override void OnCommand()
        {
            sender.Session?.Player?.Game?.PlayerNotReady(sender.Session?.Player);
        }
    }
}
