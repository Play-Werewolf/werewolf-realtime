using System.Collections.Generic;
using System.Linq;
using System;

using WerewolfServer.Platform;

namespace WerewolfServer.Game
{
    public enum DisplayColor
    {
        Green,
        Red,
        Purple,
        Blue,
        Orange,
        Rainbow,
    }

    public struct Generator
    {
        public string Name { get; set; }
        public DisplayColor Color { get; set; }
        public Character[] Possibilities { get; set; }

        public Generator(string name, DisplayColor color, params Character[] characters)
        {
            Name = name;
            Color = color;
            Possibilities = characters;
        }
    }

    public class RoleGenerator_
    {
        public Dictionary<string, Generator> Generators { get; set; } = new Dictionary<string, Generator>
        {
            ["villager"]
                = new Generator("Villager", DisplayColor.Green, new Villager()),

            ["healer"]
                = new Generator("Healer", DisplayColor.Green, new Healer()),

            ["fortune_teller"]
                = new Generator("Fortune Teller", DisplayColor.Green, new FortuneTeller()),

            ["werewolf"]
                = new Generator("Werewolf", DisplayColor.Red, new Werewolf()),

            ["alpha_werewolf"]
                = new Generator("Alpha Werewolf", DisplayColor.Red, new AlphaWerewolf()),

            ["random"]
                = new Generator("♦ Random ♦", DisplayColor.Rainbow,
                    new Villager(),
                    new Healer(),
                    new FortuneTeller(),
                    new Priest(),
                    new Werewolf(),
                    new AlphaWerewolf()
                )
        };

        Dictionary<Type, int> Limits = new Dictionary<Type, int>
        {
            [typeof(AlphaWerewolf)] = 1,
        };

        IEnumerable<Character> ApplyLimits(IEnumerable<Character> origin, Dictionary<Type, int> occurrences)
        {
            return origin.Where(c => 
                !occurrences.ContainsKey(c.GetType()) ||
                !Limits.ContainsKey(c.GetType()) ||
                occurrences[c.GetType()] < Limits[c.GetType()]);
        }

        public Character[] GenerateTown(params string[] roles)
        {
            var rand = new Random();
            var town = new List<Character>();

            var occurrences = new Dictionary<Type, int>();

            try
            {
                var generators = roles
                    .Select(role => Generators[role])
                    .OrderBy(generator => generator.Possibilities.Length);

                foreach (var g in generators)
                {
                    var possibilities = ApplyLimits(g.Possibilities, occurrences).ToArray();
                    var character = possibilities[Rand.Random.Next(possibilities.Length)];
                    
                    if (occurrences.ContainsKey(character.GetType()))
                    {
                        occurrences[character.GetType()]++;
                    }
                    else
                    {
                        occurrences[character.GetType()] = 1;
                    }
                    
                    town.Add(character);
                }
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidOperationException("Invalid role was supplied");
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidOperationException("Role constraint failed");
            }

            return town.OrderBy(x => Rand.Random.Next()).ToArray();
        }
    }
}
