using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.DataVisualization.Charting;
using Grabacr07.KanColleWrapper.Models;
using System.Windows.Media;

namespace LvChartPlugin
{
    /// <summary>
    /// ShipLevelChart.xaml の相互作用ロジック
    /// </summary>
    public partial class ShipLevelChart : UserControl
    {
        #region DependencyProperties

        private static void ParamsChanges(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (ShipLevelChart)d;
            c.CreateShipLevelChart();
        }

        #region CountMaximum

        public int CountMaximum
        {
            get { return (int)this.GetValue(CountMaximumProperty); }
            set { this.SetValue(CountMaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CountMaximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountMaximumProperty =
            DependencyProperty.Register("CountMaximum", typeof(int), typeof(ShipLevelChart), new PropertyMetadata(11, ParamsChanges));

        #endregion


        #region LevelInterval

        public int LevelInterval
        {
            get { return (int)this.GetValue(LevelIntervalProperty); }
            set { this.SetValue(LevelIntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LevelInterval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LevelIntervalProperty =
            DependencyProperty.Register("LevelInterval", typeof(int), typeof(ShipLevelChart), new PropertyMetadata(10, ParamsChanges));

        #endregion


        #region LevelMinimum

        public int LevelMinimum
        {
            get { return (int)this.GetValue(LevelMinimumProperty); }
            set { this.SetValue(LevelMinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LevelMinimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LevelMinimumProperty =
            DependencyProperty.Register("LevelMinimum", typeof(int), typeof(ShipLevelChart), new PropertyMetadata(0, ParamsChanges));

        #endregion


        #region LevelMaximum


        public int LevelMaximum
        {
            get { return (int)this.GetValue(LevelMaximumProperty); }
            set { this.SetValue(LevelMaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LevelMaximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LevelMaximumProperty =
            DependencyProperty.Register("LevelMaximum", typeof(int), typeof(ShipLevelChart), new PropertyMetadata(150, ParamsChanges));


        #endregion


        #region Ships

        public IEnumerable<Ship> Ships
        {
            get { return (IReadOnlyCollection<Ship>)this.GetValue(ShipsProperty); }
            set { this.SetValue(ShipsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ships.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShipsProperty =
            DependencyProperty.Register("Ships", typeof(IEnumerable<Ship>), typeof(ShipLevelChart), new PropertyMetadata(new Ship[0], ParamsChanges));

        #endregion


        #region BackgroundColor
        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(ShipLevelChart), new PropertyMetadata(Color.FromRgb(45, 45, 48), ParamsChanges));
        #endregion


        #region ForegroundColor
        public Color ForegroundColor
        {
            get { return (Color)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register("ForegroundColor", typeof(Color), typeof(ShipLevelChart), new PropertyMetadata(Color.FromRgb(200, 200, 200), ParamsChanges));
        #endregion

        #endregion

        public ShipLevelChart()
        {
            this.InitializeComponent();
        }

        private void CreateShipLevelChart()
        {

            if (this.Ships == null) return;

            var source = CollectionViewSource.GetDefaultView(this.Ships).Cast<Ship>().ToArray();
            var data = source.CreateShipData(this.LevelInterval, this.LevelMinimum, this.LevelMaximum);
            this.CreateChart(data, (x) => x.ToTypeName());
        }

        private void CreateChart<TKey, TX>(
            IReadOnlyDictionary<TKey, IReadOnlyDictionary<TX, Tuple<int, string>>> data,
            Func<TKey, string> areaNameSelector)
        {
            this.Chart.BackColor = this.BackgroundColor.ToDrawingColor();
            this.Chart.ForeColor = this.ForegroundColor.ToDrawingColor();

            this.Chart.ChartAreas.Clear();
            this.Chart.Series.Clear();
            this.Chart.Titles.Clear();

            var yMax = data.Any()
                ? Math.Min(this.CountMaximum, data.Max(x => x.Value.Values.Max(t => t.Item1)) + 2)
                : this.CountMaximum;
            yMax = Math.Max(yMax, 5);

            foreach (var key in data.Keys)
            {
                var area = this.CreateArea(areaNameSelector(key), yMax);
                this.Chart.ChartAreas.Add(area);

                var series = new Series(area.Name)
                {
                    ChartType = SeriesChartType.Column,
                    ChartArea = area.Name,
                    Legend = area.Name,
                    LabelAngle = 30,
                    LabelBackColor = this.BackgroundColor.ToDrawingColor(),
                    LabelForeColor = this.ForegroundColor.ToDrawingColor(),
                };
                foreach (var xKey in data[key].Keys)
                {
                    var point = new DataPoint();
                    point.SetValueXY(xKey, data[key][xKey].Item1);
                    point.ToolTip = data[key][xKey].Item2;
                    series.Points.Add(point);
                }
                this.Chart.Series.Add(series);

                this.Chart.Titles.Add(new Title
                {
                    Text = area.Name,
                    DockedToChartArea = area.Name,
                    IsDockedInsideChartArea = false,
                    Alignment = System.Drawing.ContentAlignment.BottomRight,
                    BackColor = this.BackgroundColor.ToDrawingColor(),
                    ForeColor = this.ForegroundColor.ToDrawingColor(),
                });
            }
        }

        private ChartArea CreateArea(string name, int yMax)
        {
            var backColor = this.BackgroundColor.ToDrawingColor();
            var foreColor = this.ForegroundColor.ToDrawingColor();
            var area = new ChartArea(name)
            {
                AxisX =
                {
                    Title = "Level",
                    IntervalAutoMode = IntervalAutoMode.FixedCount,
                    Interval = 1,
                    LineColor = foreColor,
                    TitleForeColor = foreColor,
                    InterlacedColor = foreColor,
                },
                AxisY =
                {
                    Title = "Count",
                    IntervalAutoMode = IntervalAutoMode.FixedCount,
                    Interval = 5,
                    Maximum = yMax,
                    Minimum = 0,
                    LineColor = foreColor,
                    TitleForeColor = foreColor,
                },
                BackColor = backColor,
            };

            area.AxisX.LabelStyle.ForeColor = foreColor;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.MajorTickMark.LineColor = foreColor;
            area.AxisX.MajorTickMark.LineDashStyle = ChartDashStyle.Dot;

            area.AxisY.LabelStyle.ForeColor = foreColor;
            area.AxisY.MajorTickMark.LineColor = foreColor;
            area.AxisY.MajorGrid.LineColor = foreColor;
            area.AxisY.MinorGrid.Enabled = true;
            area.AxisY.MinorGrid.Interval = 1;
            area.AxisY.MinorGrid.LineColor = foreColor;
            area.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dot;

            return area;
        }
    }
}