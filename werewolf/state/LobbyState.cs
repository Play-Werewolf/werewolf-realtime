using System.Collections.Generic;

namespace WerewolfServer.Game
{
    public class LobbyState : GameState
    {
        public List<Player> ReadyPlayers;

        public LobbyState(GameRoom game) : base(game)
        {
            ReadyPlayers = new List<Player>();

            RegisterHandler(CommandType.UserJoin, HandlePlayerJoin);
            RegisterHandler(CommandType.UserLeave, HandlePlayerLeave);
            RegisterHandler(CommandType.UserReady, HandlePlayerReady);
            RegisterHandler(CommandType.UserNotReady, HandlePlayerNotReady);
        }
        
        public GameState HandlePlayerJoin(GameCommand command)
        {
            if (command.Sender == null)
                return this;
            if (Game.Players.Contains(command.Sender))
                return this;

            Game.AddPlayer(command.Sender);
            return this;
        }

        public GameState HandlePlayerLeave(GameCommand command)
        {
            if (command.Sender == null)
                return this;

            if (!Game.Players.Contains(command.Sender))
                return this;

            if (ReadyPlayers.Contains(command.Sender))
                ReadyPlayers.Remove(command.Sender);

            Game.RemovePlayer(command.Sender);
            return this;
        }

        public GameState HandlePlayerReady(GameCommand command)
        {
            if (!Game.Players.Contains(command.Sender))
                return this;
                
            if (ReadyPlayers.Contains(command.Sender))
                return this;

            ReadyPlayers.Add(command.Sender);

            if (ReadyPlayers.Count == Game.Players.Count && ReadyPlayers.Count >= Game.Config.MinPlayers)
            {
                return new GameInitState(Game);
            }

            return this;
        }

        public GameState HandlePlayerNotReady(GameCommand command)
        {
            if (!ReadyPlayers.Contains(command.Sender))
                return this;

            ReadyPlayers.Remove(command.Sender);
            return this;
        }
    }
}