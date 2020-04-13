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
        public Type[] Possibilities { get; set; }

        public Generator(string name, DisplayColor color, params Type[] characters)
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
                = new Generator("Villager", DisplayColor.Green, typeof(Villager)),

            ["healer"]
                = new Generator("Healer", DisplayColor.Green, typeof(Healer)),

            ["fortune_teller"]
                = new Generator("Fortune Teller", DisplayColor.Green, typeof(FortuneTeller)),

            ["werewolf"]
                = new Generator("Werewolf", DisplayColor.Red, typeof(Werewolf)),

            ["alpha_werewolf"]
                = new Generator("Alpha Werewolf", DisplayColor.Red, typeof(AlphaWerewolf)),

            ["random"]
                = new Generator("♦ Random ♦", DisplayColor.Rainbow,
                    typeof(Villager),
                    typeof(Healer),
                    typeof(FortuneTeller),
                    typeof(Priest),
                    typeof(Werewolf),
                    typeof(AlphaWerewolf)
                )
        };

        Dictionary<Type, int> Limits = new Dictionary<Type, int>
        {
            [typeof(AlphaWerewolf)] = 1,
        };

        IEnumerable<Type> ApplyLimits(IEnumerable<Type> origin, Dictionary<Type, int> occurrences)
        {
            return origin.Where(c => 
                !occurrences.ContainsKey(c) ||
                !Limits.ContainsKey(c) ||
                occurrences[c] < Limits[c]);
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
                    var type = possibilities[Rand.Random.Next(possibilities.Length)];
                    var character = (Character)Activator.CreateInstance(type);
                    
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

            return town.OrderBy(x => Rand.Random.Next()).ToArray(); // TODO: Promote werewolf to AWW if there isn't any
        }
    }
}
