using Livet;
using Livet.Messaging;

namespace LvChartPlugin.ViewModels
{
    public class ToolViewModel : ViewModel
    {
        public void OpenChartWindow()
        {
            var message = new TransitionMessage("Show/ChartWindow");
            this.Messenger.RaiseAsync(message);
        }

        public void OpenTableWindow()
        {
            var message = new TransitionMessage("Show/TableWindow");
            this.Messenger.RaiseAsync(message);
        }
    }
}
