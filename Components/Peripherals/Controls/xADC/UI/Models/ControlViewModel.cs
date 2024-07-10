using xLibV100.Peripherals.xADC.UI.Interfaces;
using xLibV100.Peripherals.xADC.UI.Views;
using System.Collections.Generic;
using System.Windows.Controls;
using xLibV100.UI;
using xLibV100.Common.UI.Models;
using xLibV100.Controls;

namespace xLibV100.Peripherals.xADC.UI.Models
{
    public class ControlViewModel : ViewModelBase<Control, ControlView>, IControlViewModel
    {
        protected TabControl tab_control;

        public ControlViewModel(Control model) : base(model)
        {
            Name = model.Name;

            List<ModelBase> modelBases = new List<ModelBase>();

            Models.Add(new PortsViewModel(model));

            View = new ControlView(this);
            UIModel = View;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
