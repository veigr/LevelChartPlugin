using MetroTrilithon.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LvChartPlugin.Settings
{
    public static class ChartSettings
    {
        public static SerializableProperty<int> CountMaximumCurrentValue { get; }
            = new SerializableProperty<int>(GetKey(), Providers.Roaming, 11) { AutoSave = true };

        public static SerializableProperty<int> LevelInterval { get; }
            = new SerializableProperty<int>(GetKey(), Providers.Roaming, 10) { AutoSave = true };

        public static SerializableProperty<bool> IsLocked { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Roaming, true) { AutoSave = true };

        public static SerializableProperty<bool> IsExpand { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Roaming, false) { AutoSave = true };

        public static SerializableProperty<int> LevelMaximumSliderValue { get; }
            = new SerializableProperty<int>(GetKey(), Providers.Roaming, CommonSettings.LevelLimit) { AutoSave = true };

        public static SerializableProperty<int> LevelMinimumSliderValue { get; }
            = new SerializableProperty<int>(GetKey(), Providers.Roaming, 0) { AutoSave = true };

        public static SerializableProperty<int[]> SelectedShipTypes { get; }
            = new SerializableProperty<int[]>(GetKey(), Providers.Roaming, null) { AutoSave = true };

        private static string GetKey([CallerMemberName] string propertyName = "")
        {
            return nameof(ChartSettings) + "." + propertyName;
        }
    }
}
