using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

namespace LvChartPlugin
{
    [Export(typeof(IToolPlugin))]
    [ExportMetadata("Title", "LvChart")]
    [ExportMetadata("Description", "Level チャート を表示します。")]
    [ExportMetadata("Version", "1.1.0")]
    [ExportMetadata("Author", "@veigr")]
    public class LvChart : IToolPlugin
    {
        private readonly ToolViewModel vm = new ToolViewModel();

        public object GetToolView()
        {
            return new ToolView { DataContext = this.vm };
        }

        public string ToolName
        {
            get { return "LvChart"; }
        }

        public object GetSettingsView()
        {
            return null;
        }
    }
}
