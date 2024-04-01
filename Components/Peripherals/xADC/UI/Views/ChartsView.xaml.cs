using xLibV100.Peripherals.xADC.UI.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace xLibV100.Peripherals.xADC.UI.Views
{
    /// <summary>
    /// Логика взаимодействия для ChartsView.xaml
    /// </summary>
    public partial class ChartsView : UserControl, IChartsView
    {
        protected IChartsViewModel ViewModel { get; set; }

        public ChartsView()
        {
            InitializeComponent();
        }

        public ChartsView(IChartsViewModel viewModel) : this()
        {
            ViewModel = viewModel;
            DataContext = viewModel;
        }

        public System.Windows.Forms.Control HostElement
        {
            get => WindowsFormsHost.Child;
            set => WindowsFormsHost.Child = value;
        }

        private void ButClearPoints_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.ClearChart();
        }

        private void ButEnableNotyfication_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.EnableNotification();
        }

        private void ButDisableNotyfication_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.DisableNotification();
        }
    }
}
