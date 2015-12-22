using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using LvChartPlugin.ViewModels;

namespace LvChartPlugin.Views.Converters
{
    public class LevelToColorConverter : IValueConverter
    {
        public Brush Background1 { get; set; }
        public Brush Background2 { get; set; }

        public LevelToColorConverter()
        {
            this.Background1 = new SolidColorBrush(Colors.Transparent);
            this.Background2 = new SolidColorBrush(Colors.Transparent);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var defaultValue = new SolidColorBrush(Colors.Transparent);
            var row = value as ShipTableRow;
            if (row == null) return defaultValue;

            if (99 < row.Lv) return this.Background1;
            return row.Lv / 10 % 2 == 0 ? this.Background1 : this.Background2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
