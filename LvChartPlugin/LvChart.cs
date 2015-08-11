using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;
using LvChartPlugin.ViewModels;

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
        private readonly ToolViewModel vm = new ToolViewModel();
        
        public void Initialize() {}

        public string Name => "LvChart";

        // タブ表示するたびに new されてしまうが、今のところ new しないとマルチウィンドウで正常に表示されない
        public object View => new Views.ToolView {DataContext = this.vm};
    }
}
