namespace WerewolfServer.Network
{
    public class RoomCommand : BaseCommand
    {
        public override void AfterCommand()
        {
            sender.SendRoomStatus();
        }

        public override void OnError(Error error)
        {
            sender.Send(messageType: "room_status", "error", error.Description);
        }
    }

    public class CreateRoomCommand : RoomCommand
    {
        public override string CommandType => "create_room";

        public override bool Validate()
        {
            return !sender.IsInRoom;
        }

        public override void OnCommand()
        {
            var game = sender.Manager.Rooms.CreateGame();
            sender.Session.InitPlayer();
            
            var cmd = new Game.GameCommand
            {
                Type = Game.CommandType.UserJoin,
                Sender = sender.Session.Player,
            };
            game.HandleCommand(cmd);
        }
    }

    public class LeaveRoomCommand : RoomCommand
    {
        public override string CommandType => "leave_room";
        public override bool Validate()
        {
            return sender.IsInRoom;
        }

        public override void OnCommand()
        {
            var cmd = new Game.GameCommand
            {
                Type = Game.CommandType.UserLeave,
                Sender = sender.Session.Player
            };

            sender.Session.Player.Game.HandleCommand(cmd);
            sender.Session.DetachPlayer(); // Removing the player object


        }
    }

    public class JoinRoomCommand : RoomCommand
    {
        public override string CommandType => "join_room";
        public override int ArgumentsNumber => 1;

        public override bool Validate()
        {
            return !sender.IsInRoom;
        }

        public override void OnCommand()
        {
            if (sender.Manager.Rooms.Games.ContainsKey(message.Args[0]))
            {
                throw new Error("Room not found");
            }

            sender.Session.InitPlayer();
            
            var game = sender.Manager.Rooms.Games[message.Args[0]];
            var cmd = new Game.GameCommand
            {
                Type = Game.CommandType.UserJoin,
                Sender = sender.Session.Player,
            };

            game.HandleCommand(cmd);
        }
    }
}
