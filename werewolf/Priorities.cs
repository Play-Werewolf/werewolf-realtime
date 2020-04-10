using System;
using System.Collections.Generic;
using System.Linq;

namespace WerewolfServer.Game
{
    public enum NightPriority
    {
        Normal,
    }

    public static class NightPriorityExtensions
    {
        private static Dictionary<Type, NightPriority> nightPriorities = new Dictionary<Type, NightPriority>
        {
            [typeof(Werewolf)] = NightPriority.Normal,
        };

        public static NightPriority GetNightPriority(this Character character)
        {
            return nightPriorities.ContainsKey(character.GetType())
                ? nightPriorities[character.GetType()]
                : NightPriority.Normal;
        }

        public static IEnumerable<Character> Prioritized(this IEnumerable<Character> list)
        {
            return list.OrderBy(c => c.GetNightPriority());
        }

        public static IEnumerable<Player> Prioritized(this IEnumerable<Player> list)
        {
            return list.OrderBy(p => p.Character.GetNightPriority());
        }
    }
}
