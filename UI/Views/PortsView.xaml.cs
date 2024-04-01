using System.Windows;
using System.Windows.Controls;
using xLibV100.UI;

namespace xLibV100.UI.Views
{
    /// <summary>
    /// Логика взаимодействия для SubscriptionsView.xaml
    /// </summary>
    public partial class PortsView : UserControl
    {
        private IPortsViewModel ViewModel
        {
            get => DataContext as IPortsViewModel;
            set => DataContext = value;
        }

        public PortsView()
        {
            InitializeComponent();

            DataContextChanged += DataContextChangedHandler;
        }

        private void UpdateListViewPortsColumnsSize()
        {
            var listView = ListViewPorts;

            if (listView.View is GridView gridView)
            {
                foreach (var column in gridView.Columns)
                {
                    column.Width = 0;
                    column.Width = double.NaN;
                }
            }
        }

        private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ViewModel is ViewModelBase viewModel)
            {
                viewModel.UpdateEvent += UpdateEventHandler;
            }
        }

        private void UpdateEventHandler(ViewModelBase viewModel)
        {
            UpdateListViewPortsColumnsSize();
        }

        public PortsView(IPortsViewModel viewModel) : this()
        {
            ViewModel = viewModel;
        }

        private void ButSubscribe_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.Subscribe();
        }

        private void ButUnsubscribe_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.Unsubscribe();
        }

        private void ButInstallAsTx_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.SelectTxLine();
        }

        private void ButResetTx_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.ResetTxLine();
        }

        private void ListViewPorts_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateListViewPortsColumnsSize();
        }
    }
}
