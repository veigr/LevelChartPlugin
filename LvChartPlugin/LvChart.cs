using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;
using LvChartPlugin.ViewModels;

namespace LvChartPlugin
{
    [Export(typeof(IPlugin))]
    [Export(typeof(ITool))]
    [Export(typeof(ISettings))]
    [ExportMetadata("Guid", "626E91CE-A031-4FB6-8E02-E52906A665BA")]
    [ExportMetadata("Title", "LvChart")]
    [ExportMetadata("Description", "Level チャート を表示します。")]
    [ExportMetadata("Version", "1.3.1")]
    [ExportMetadata("Author", "@veigr")]
    public class LvChart : IPlugin, ITool, ISettings
    {
        private readonly ToolViewModel vm = new ToolViewModel();
        
        public void Initialize() {}

        string ITool.Name => "LvChart";

        // タブ表示するたびに new されてしまうが、今のところ new しないとマルチウィンドウで正常に表示されない
        object ITool.View => new Views.ToolView {DataContext = this.vm};

        object ISettings.View => new Views.SettingsView();
    }
}
