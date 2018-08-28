using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MetroTrilithon.Serialization;

namespace LvChartPlugin.Settings
{
    public static class CommonSettings
    {
        public static SerializableProperty<int> LevelLimit { get; }
            = new SerializableProperty<int>(GetKey(), Providers.Roaming, 175) { AutoSave = true };

        private static string GetKey([CallerMemberName] string propertyName = "")
        {
            return nameof(CommonSettings) + "." + propertyName;
        }
    }
}
