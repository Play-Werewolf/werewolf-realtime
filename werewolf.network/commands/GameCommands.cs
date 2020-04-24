using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using WerewolfServer.Game;

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

    class SetRolesCommand : BaseCommand
    {
        public override string CommandType => "set_roles";
        public override int ArgumentsNumber => 1;

        public override void OnCommand()
        {
            if (sender.Session?.Player?.Game?.IsAdmin(sender.Session?.Player) != true)
                return;

            try
            {
                string[] roles = JsonSerializer.Deserialize<string[]>(message.Args[0]);
                sender.Session?.Player?.Game?.SetRolesList(roles);
            }
            catch (JsonException) { }
        }
    }

    class VoteAgainstPlayerCommand : BaseCommand
    {
        public override string CommandType => "vote_player";
        public override int ArgumentsNumber => 1;

        public override void OnCommand()
        {
            var p = sender.Session?.Player?.Game?.GetPlayer(message.Args[0]);
            if (p == null || p == sender.Session?.Player)
                return;

            sender.Session?.Player?.VoteToKill(p);
        }
    }

    class UnvotePlayerCommand : BaseCommand
    {
        public override string CommandType => "unvote_player";

        public override void OnCommand()
        {
            sender.Session?.Player?.ResetVote();
        }
    }

    class VoteCommand : BaseCommand
    {
        public override string CommandType => "vote";
        public override int ArgumentsNumber => 1;

        public override void OnCommand()
        {
            switch (message.Args[0])
            {
                case "guilty":
                    sender.Session?.Player?.VoteGuilty();
                    break;
                case "innocent":
                    sender.Session?.Player?.VoteInnocent();
                    break;
                case "abstain":
                    sender.Session?.Player?.UndoTrialVote();
                    break;
            }
        }
    }

    class SetBooleanActionCommand : BaseCommand
    {
        public override string CommandType => "set_boolean_action";
        public override int ArgumentsNumber => 1;

        public override void OnCommand()
        {
            bool shouldAct = message.Args[0] == "true";
            sender.Session?.Player?.Character?.SetAction(new BooleanAction(shouldAct));
        }
    }

    class SetUnaryActionCommand : BaseCommand
    {
        public override string CommandType => "set_unary_action";
        public override int ArgumentsNumber => 1;
        public override void OnCommand()
        {
            var player = sender.Session?.Player?.Game?.GetPlayer(message.Args[0]);
            sender.Session?.Player?.Character?.SetAction(new UnaryAction(player));
        }
    }

    class SetBinaryActionCommand : BaseCommand
    {
        public override string CommandType => "set_binary_action";
        public override int ArgumentsNumber => 2;
        public override void OnCommand()
        {
            var player0 = sender.Session?.Player?.Game?.GetPlayer(message.Args[0]);
            var player1 = sender.Session?.Player?.Game?.GetPlayer(message.Args[1]);
            sender.Session?.Player?.Character?.SetAction(new BinaryAction(player0, player1));
        }
    }
}
