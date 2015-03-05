using Livet;
using Livet.Messaging;

namespace LvChartPlugin
{
    public class ToolViewModel : ViewModel
    {
        public void OpenChartWindow()
        {
            var message = new TransitionMessage("Show/ChartWindow");
            this.Messenger.RaiseAsync(message);
        }
    }
}
