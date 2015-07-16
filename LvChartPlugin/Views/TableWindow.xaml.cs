using System;
using System.Windows;
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
            this.InitializeComponent();
            WeakEventManager<Window, EventArgs>.AddHandler(
                Application.Current.MainWindow,
                "Closed",
                (_, __) => this.Close());
        }
    }
}
