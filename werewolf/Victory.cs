using System.Linq;

namespace WerewolfServer.Game
{
    public enum GameResult
    {
        LoseAndEndGame,
        Lose,
        Unknown,
        WinAndContinue,
        WinAndEndGame,
    }

    public class GameStats
    {
        public int LivingTownies { get; set; }
        public int LivingWerewolves { get; set; }
        public int LivingChaotics { get; set; }
        public int LivingNeutrals { get; set; }

        public static GameStats CreateFrom(GameRoom game)
        {
            var players = game.Players.Where(p => p.Character != null);
            var livingPeople = players.Select(p => p?.Character).Where(p => p.Alive).ToList();
            var townies = livingPeople.Where(c => c.Alignment == Alignment.Good).Count();
            var werewolves = livingPeople.Where(c => c.Alignment == Alignment.Evil).Count();
            var chaotics = livingPeople.Where(c => c.Alignment == Alignment.Chaos).Count();
            var neutrals = livingPeople.Where(c => c.Alignment == Alignment.Neutral).Count();

            return new GameStats
            {
                LivingTownies = townies,
                LivingWerewolves = werewolves,
                LivingChaotics = chaotics,
                LivingNeutrals = neutrals
            };
        }
    }

    public interface VictoryCalculator
    {
        GameResult GetResult(GameStats game, Character character);
    }

    public class TownVictory : VictoryCalculator
    {
        public GameResult GetResult(GameStats game, Character character)
        {
            if (game.LivingTownies == 0)
                return GameResult.Lose;

            if (game.LivingTownies > 0 && game.LivingWerewolves == 0 && game.LivingChaotics == 0)
                return GameResult.WinAndEndGame;

            return GameResult.Unknown;
        }
    }

    public class WerewolfVictory : VictoryCalculator
    {
        public GameResult GetResult(GameStats game, Character character)
        {
            if (game.LivingWerewolves == 0)
                return GameResult.Lose;

            if (game.LivingWerewolves > 0 && game.LivingTownies == 0 && game.LivingChaotics == 0)
                return GameResult.WinAndEndGame;

            return GameResult.Unknown;
        }
    }
}
