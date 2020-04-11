using System;
using System.Collections.Generic;

namespace WerewolfServer.Game
{
    public enum CommandType
    {
        Timer,
        UserJoin,
        UserLeave,
        UserReady,
        UserNotReady,
        UserAction,
        UserVote,
        UserUnvote,
        KickUser,
    }

    public sealed class GameCommand
    {
        public CommandType Type;
        public Player Sender; // set to null for admin Events
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        public object this[string index]
        {
            get
            {
                return Data.ContainsKey(index) ? Data[index] : null;
            }
        }
    }

    public class GameState
    {
        protected GameRoom Game { get; set; }

        private Dictionary<CommandType, Func<GameCommand, GameState>> handlers = new Dictionary<CommandType, Func<GameCommand, GameState>>();

        public GameState(GameRoom game)
        {
            Game = game;
        }

        // TODO: Convert to attribute
        protected void RegisterHandler(CommandType type, Func<GameCommand, GameState> handler)
        {
            handlers[type] = handler;
        }

        public GameState HandleEvent(GameCommand command)
        {
            if (handlers.ContainsKey(command.Type))
            {
                return handlers[command.Type](command);
            }
            else
            {
                return this;
            }
        }
    }
}
