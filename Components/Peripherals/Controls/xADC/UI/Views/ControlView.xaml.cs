using xLibV100.Peripherals.xADC.UI.Interfaces;
using xLibV100.Peripherals.xADC.UI.Models;
using System.Windows.Controls;

namespace xLibV100.Peripherals.xADC.UI.Views
{
    /// <summary>
    /// Логика взаимодействия для ControlView.xaml
    /// </summary>
    public partial class ControlView : UserControl
    {
        private IControlViewModel ViewModel { get; set; }

        public ControlView()
        {
            InitializeComponent();
        }

        public ControlView(ControlViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
