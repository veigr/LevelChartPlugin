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
            => type.Id == 8 ? "巡洋戦艦" : type.Name;

        public static IReadOnlyDictionary<ShipType, IReadOnlyDictionary<string, Tuple<int, string>>> CreateShipData(
            this IEnumerable<Ship> ships, int interval, int min, int max)
        {
            var minimum = min < max ? min : max;
            var maximum = min < max ? max : min;
            return ships
                .OrderBy(x => x.Info.ShipType.Id)
                .GroupBy(
                    x => x.Info.ShipType,
                    x => x,
                    (shipType, elements) =>
                        new KeyValuePair<ShipType, IReadOnlyDictionary<string, Tuple<int, string>>>
                            (shipType, elements.GroupByLevel(interval, minimum, maximum))
                ).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static IReadOnlyDictionary<TX, Tuple<int, string>> SumValues<TX>(
            this IEnumerable<IReadOnlyDictionary<TX, Tuple<int, string>>> sources)
        {
            var dic = new Dictionary<TX, Tuple<int, string>>();
            foreach (var data in sources)
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
            => source.Max(x => x.Value.Item1) + 1;

        private static IReadOnlyDictionary<string, Tuple<int, string>> GroupByLevel(
            this IEnumerable<Ship> ships,
            int interval,
            int minimum,
            int maximum)
        {
            return ships
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
                        if (result.ContainsKey(x.level))
                            result[x.level] = Tuple.Create(x.count, x.name);
                        return result;
                    });
        }

        private static Dictionary<string, Tuple<int, string>> CreateLevelGroupsDictionary(int interval, int min, int max)
            => CreateLevelGroups(interval, min, max)
                .ToDictionary(x => x, x => Tuple.Create(0, ""));

        private static IEnumerable<string> CreateLevelGroups(int interval, int min, int max)
        {
            for (var i = min; i <= max; i += interval)
            {
                yield return i.LevelToPart(interval);
            }
        }

        private static string LevelToPart(this int level, int interval)
        {
            var div = level / interval;
            var min = div * interval;
            return min + "～";
        }

        private static string ToTooltipNames(this IEnumerable<Ship> ships)
            => string.Join(", ", ships.OrderByDescending(x => x.Level).Select(x => x.Info.Name));

        private static string JoinString(this string value1, string separator, string value2)
        {
            if (string.IsNullOrWhiteSpace(value1)) return value2;
            if (string.IsNullOrWhiteSpace(value2)) return value1;
            return string.Join(separator, value1, value2);
        }
    }
}