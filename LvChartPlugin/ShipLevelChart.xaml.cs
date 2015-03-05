using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.DataVisualization.Charting;
using Grabacr07.KanColleWrapper.Models;

namespace LvChartPlugin
{
    /// <summary>
    /// ShipLevelChart.xaml の相互作用ロジック
    /// </summary>
    public partial class ShipLevelChart : UserControl
    {
        private readonly Color backColor;
        private readonly Color foreColor;

        #region DependencyProperties

        private static void ParamsChanges(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (ShipLevelChart)d;
            c.CreateChart();
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

        #endregion

        public ShipLevelChart()
        {
            this.InitializeComponent();

            this.backColor = Color.FromArgb(45, 45, 48);
            this.foreColor = Color.FromArgb(200, 200, 200);

            this.Chart.BackColor = this.backColor;
            this.Chart.ForeColor = this.foreColor;
        }

        private void CreateChart(object sender = null, EventArgs args = null)
        {
            this.Chart.ChartAreas.Clear();
            this.Chart.Series.Clear();
            this.Chart.Titles.Clear();

            if (this.Ships == null) return;

            var source = CollectionViewSource.GetDefaultView(this.Ships);
            var data = source.Cast<Ship>().ToArray().CreateShipData(this.LevelInterval, this.LevelMinimum, this.LevelMaximum);
            var yMax = data.Any()
                ? Math.Min(this.CountMaximum, data.Max(x => x.Value.Values.Max()) + 2)
                : this.CountMaximum;
            yMax = Math.Max(yMax, 5);

            foreach (var shipType in data.Keys)
            {
                var area = new ChartArea(shipType.ToTypeName())
                {
                    AxisX =
                    {
                        Title = "Level",
                        IntervalAutoMode = IntervalAutoMode.FixedCount,
                        Interval = 1,
                        LineColor = this.foreColor,
                        TitleForeColor = this.foreColor,
                        InterlacedColor = this.foreColor,
                        LabelStyle = new LabelStyle { ForeColor = this.foreColor },
                    },
                    AxisY =
                    {
                        Title = "Count",
                        IntervalAutoMode = IntervalAutoMode.FixedCount,
                        Interval = 5,
                        Maximum = yMax,
                        Minimum = 0,
                        LineColor = this.foreColor,
                        TitleForeColor = this.foreColor,
                    },
                    BackColor = this.backColor,
                };

                area.AxisX.LabelStyle.ForeColor = this.foreColor;
                area.AxisX.MajorGrid.Enabled = false;
                area.AxisX.MajorTickMark.LineColor = this.foreColor;
                area.AxisX.MajorTickMark.LineDashStyle = ChartDashStyle.Dot;

                area.AxisY.LabelStyle.ForeColor = this.foreColor;
                area.AxisY.MajorTickMark.LineColor = this.foreColor;
                area.AxisY.MajorGrid.LineColor = this.foreColor;

                area.AxisY.MinorGrid.Enabled = true;
                area.AxisY.MinorGrid.Interval = 1;
                area.AxisY.MinorGrid.LineColor = this.foreColor;
                area.AxisY.MinorGrid.LineDashStyle = ChartDashStyle.Dot;

                this.Chart.ChartAreas.Add(area);
                

                var series = new Series(area.Name)
                {
                    ChartType = SeriesChartType.Column,
                    ChartArea = area.Name,
                    Legend = area.Name,
                    LabelAngle = 30,
                    LabelBackColor = this.backColor,
                    LabelForeColor = this.foreColor,
                };
                foreach (var key in data[shipType].Keys)
                {
                    var point = new DataPoint();
                    point.SetValueXY(key, data[shipType][key]);
                    //point.ToolTip = key;
                    series.Points.Add(point);
                }
                this.Chart.Series.Add(series);
                this.Chart.Titles.Add(new Title
                {
                    Text = area.Name,
                    DockedToChartArea = area.Name,
                    IsDockedInsideChartArea = false,
                    Alignment = ContentAlignment.BottomRight,
                    BackColor = this.backColor,
                    ForeColor = this.foreColor,
                });
            }
        }
    }
}