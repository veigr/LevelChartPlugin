using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using LvChartPlugin.ViewModels;

namespace LvChartPlugin.Views.Converters
{
    public class LevelToColorConverter : IMultiValueConverter
    {
        public Brush Background1 { get; set; }
        public Brush Background2 { get; set; }
        private Brush CurrentBackground { get; set; }

        public LevelToColorConverter()
        {
            this.Background1 = new SolidColorBrush(Colors.Transparent);
            this.Background2 = new SolidColorBrush(Colors.Transparent);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var defaultValue = new SolidColorBrush(Colors.Transparent);
            if (values.Length != 2) return defaultValue;
            if (!values.Any(x => x is ShipTableRow)) return defaultValue;

            var current = values[0] as ShipTableRow;
            var previous = values[1] as ShipTableRow;
            var currentLv = current != null ? current.Lv : int.MaxValue;
            var previousLv = previous != null ? previous.Lv : int.MaxValue;

            if (previousLv == int.MaxValue) this.CurrentBackground = this.Background2;

            if (99 < currentLv) return this.CurrentBackground;
            if (currentLv / 10 != previousLv / 10) this.SwapBackground();

            return this.CurrentBackground;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private void SwapBackground()
        {
            this.CurrentBackground = this.CurrentBackground.Equals(this.Background1)
                ? this.Background2
                : this.Background1;
        }
    }
}
