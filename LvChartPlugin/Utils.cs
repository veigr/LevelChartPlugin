using System.Collections.Generic;
using System.Linq;
using Grabacr07.KanColleWrapper.Models;

namespace LvChartPlugin
{
    internal static class Utils
    {
        private static string LevelToPart(this int level, int interval)
        {
            var div = level / interval;
            var min = div * interval;
            return min + "Å`";
        }

        public static string ToTypeName(this ShipType type)
        {
            return type.Id == 8 ? "èÑómêÌäÕ" : type.Name;
        }

        public static IReadOnlyDictionary<ShipType, IReadOnlyDictionary<string, int>> CreateShipData(
            this IEnumerable<Ship> ships, int interval = 10, int min = 0, int max = 150)
        {
            var minimum = min < max ? min : max;
            var maximum = min < max ? max : min;
            return ships
                .OrderBy(x => x.Info.ShipType.Id)
                .GroupBy(
                    x => x.Info.ShipType,
                    x => x,
                    (shipType, elements) => new KeyValuePair<ShipType, IReadOnlyDictionary<string, int>>
                        (
                        shipType,
                        elements
                            .Where(x => x.Level <= maximum)
                            .Where(x => minimum <= x.Level)
                            .OrderBy(x => x.Level)
                            .GroupBy(
                                x => x.Level.LevelToPart(interval),
                                x => x,
                                (level, x) => new {level, count = x.Count()})
                            .Aggregate(CreateLevelGroupsDictionary(interval, minimum, maximum),
                                (result, x) =>
                                {
                                    if (result.ContainsKey(x.level)) result[x.level] = x.count;
                                    return result;
                                })
                        )
                ).ToDictionary(x => x.Key, x => x.Value);
        }

        private static Dictionary<string, int> CreateLevelGroupsDictionary(int interval, int min, int max)
        {
            return CreateLevelGroups(interval, min, max)
                .ToDictionary(x => x, x => 0);
        }

        private static IEnumerable<string> CreateLevelGroups(int interval, int min, int max)
        {
            for (var i = min; i <= max; i += interval)
            {
                yield return i.LevelToPart(interval);
            }
        }
    }
}