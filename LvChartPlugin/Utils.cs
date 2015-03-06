using System;
using System.Collections.Generic;
using System.Linq;
using Grabacr07.KanColleWrapper.Models;

namespace LvChartPlugin
{
    internal static class Utils
    {
        public static System.Drawing.Color ToDrawingColor(this System.Windows.Media.Brush brush)
        {
            var solidColorBrush = brush as System.Windows.Media.SolidColorBrush;
            if (solidColorBrush == null) return System.Drawing.Color.Transparent;
            var color = solidColorBrush.Color;
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

        public static string ToTypeName(this ShipType type)
        {
            return type.Id == 8 ? "èÑómêÌäÕ" : type.Name;
        }

        public static IReadOnlyDictionary<ShipType, IReadOnlyDictionary<string, Tuple<int, string>>> CreateShipData(
            this IEnumerable<Ship> ships, int interval = 10, int min = 0, int max = 150)
        {
            var minimum = min < max ? min : max;
            var maximum = min < max ? max : min;
            return ships
                .OrderBy(x => x.Info.ShipType.Id)
                .GroupBy(
                    x => x.Info.ShipType,
                    x => x,
                    (shipType, elements) => new KeyValuePair<ShipType, IReadOnlyDictionary<string, Tuple<int, string>>>
                        (
                        shipType,
                        elements
                            .Where(x => x.Level <= maximum)
                            .Where(x => minimum <= x.Level)
                            .OrderBy(x => x.Level)
                            .GroupBy(
                                x => x.Level.LevelToPart(interval),
                                x => x,
                                (level, x) => new { level, count = x.Count(), name = x.ToTooltipNames() })
                            .Aggregate(CreateLevelGroupsDictionary(interval, minimum, maximum),
                                (result, x) =>
                                {
                                    if (result.ContainsKey(x.level)) result[x.level] = Tuple.Create(x.count, x.name);
                                    return result;
                                })
                        )
                ).ToDictionary(x => x.Key, x => x.Value);
        }

        public static IReadOnlyDictionary<TX, Tuple<int, string>> SumValues<TX>(
            this IEnumerable<IReadOnlyDictionary<TX, Tuple<int, string>>> perStypeData)
        {
            var dic = new Dictionary<TX, Tuple<int, string>>();
            foreach (var data in perStypeData)
            {
                foreach (var key in data.Keys)
                {
                    Tuple<int, string> value;
                    if (dic.TryGetValue(key, out value))
                    {
                        dic[key] = Tuple.Create(
                            value.Item1 + data[key].Item1,
                            value.Item2.JoinString(", ", data[key].Item2));
                    }
                    else
                    {
                        dic.Add(key, data[key]);
                    }
                }
            }
            return dic;
        }

        public static int CountMaximum<TX>(this IReadOnlyDictionary<TX, Tuple<int, string>> source)
        {
            return source.Max(x => x.Value.Item1) + 1;
        }

        private static Dictionary<string, Tuple<int, string>> CreateLevelGroupsDictionary(int interval, int min, int max)
        {
            return CreateLevelGroups(interval, min, max)
                .ToDictionary(x => x, x => Tuple.Create(0, ""));
        }

        private static string LevelToPart(this int level, int interval)
        {
            var div = level / interval;
            var min = div * interval;
            return min + "Å`";
        }

        private static IEnumerable<string> CreateLevelGroups(int interval, int min, int max)
        {
            for (var i = min; i <= max; i += interval)
            {
                yield return i.LevelToPart(interval);
            }
        }

        private static string ToTooltipNames(this IEnumerable<Ship> ships)
        {
            return string.Join(", ", ships.Select(x => x.Info.Name));
        }

        private static string JoinString(this string value1, string separator, string value2)
        {
            if (string.IsNullOrWhiteSpace(value1)) return value2;
            if (string.IsNullOrWhiteSpace(value2)) return value1;
            return string.Join(separator, value1, value2);
        }
    }
}