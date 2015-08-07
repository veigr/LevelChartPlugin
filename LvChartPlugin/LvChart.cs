using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;
using LvChartPlugin.ViewModels;
using LvChartPlugin.Views;

namespace LvChartPlugin
{
    [Export(typeof(IPlugin))]
    [Export(typeof(ITool))]
    [ExportMetadata("Guid", "626E91CE-A031-4FB6-8E02-E52906A665BA")]
    [ExportMetadata("Title", "LvChart")]
    [ExportMetadata("Description", "Level チャート を表示します。")]
    [ExportMetadata("Version", "1.3.0")]
    [ExportMetadata("Author", "@veigr")]
    public class LvChart : IPlugin, ITool
    {
        private readonly ToolView v = new ToolView { DataContext = new ToolViewModel() };
        
        public void Initialize() {}

        public string Name => "LvChart";

        public object View => this.v;
    }
}
