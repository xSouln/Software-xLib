using System.Windows;
using System.Windows.Controls;
using xLibV100.UI;

namespace xLibV100.UI.Views
{
    /// <summary>
    /// Логика взаимодействия для ConnectionInfoView.xaml
    /// </summary>
    public partial class PortInfoView : UserControl
    {
        public ViewModelBase ViewModel
        {
            get => DataContext as ViewModelBase;
            set => DataContext = value;
        }

        public PortInfoView()
        {
            InitializeComponent();

            DataContextChanged += DataContextChangedHandler;
        }

        private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ViewModel != null)
            {
                if (e.OldValue is ViewModelBase viewModel)
                {
                    //viewModel.CelltSizeChanged -= CellElemntSizeChangedHandler;
                    viewModel.UpdateEvent -= UpdateEventHandler;
                }

                //ViewModel.CelltSizeChanged += CellElemntSizeChangedHandler;
                ViewModel.UpdateEvent += UpdateEventHandler;
            }
        }

        private void UpdateEventHandler(ViewModelBase viewModel)
        {
            UpdateListViewColumnsSize();
            UpdateLayout();
        }

        private void CellElemntSizeChangedHandler(ViewModelBase viewModel, ICellElement element)
        {
            UpdateListViewColumnsSize();
            UpdateLayout();
        }

        private void UpdateListViewColumnsSize()
        {
            var listView = ListView;
            if (listView.View is GridView gridView)
            {
                foreach (var column in gridView.Columns)
                {
                    column.Width = 0;
                    column.Width = double.NaN;
                }
            }
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateListViewColumnsSize();
            UpdateLayout();
        }

        private void ListView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //UpdateListViewColumnsSize();
            //UpdateLayout();

            //ViewModel?.OnViewUpdateEvent();
        }
    }
}
