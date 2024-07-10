using System.Collections.ObjectModel;
using xLibV100.Controls;

namespace xLibV100.Peripherals
{
    public class Control : TerminalObject
    {
        public ObservableCollection<PeripheralBase> Peripherals { get; set; } = new ObservableCollection<PeripheralBase>();

        public Control(TerminalBase model) : base(model)
        {

        }
    }
}
