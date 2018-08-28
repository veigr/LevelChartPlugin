using MetroTrilithon.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LvChartPlugin.Settings
{
    public static class Providers
    {
        private static string RoamingFilePath { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "cat-ears.net", "LvChart", "Settings.xaml");

        public static ISerializationProvider Roaming { get; } = new FileSettingsProvider(RoamingFilePath);
    }
}
