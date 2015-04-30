using System;
using System.Windows;
using Grabacr07.KanColleViewer.Views;
using MetroRadiance.Controls;

namespace LvChartPlugin.Views
{
    /// <summary>
    /// TableWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TableWindow : MetroWindow
    {
        public TableWindow()
        {
            InitializeComponent();
            WeakEventManager<MainWindow, EventArgs>.AddHandler(
                MainWindow.Current,
                "Closed",
                (_, __) => this.Close());
        }
    }
}
