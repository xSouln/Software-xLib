using System.Windows;
using System.Windows.Controls;
using xLibV100.UI;

namespace xLibV100.UI.Views
{
    /// <summary>
    /// Логика взаимодействия для ConnectionOptionsView.xaml
    /// </summary>
    public partial class PortOptionsView : UserControl
    {
        public ViewModelBase ViewModel
        {
            get => DataContext as ViewModelBase;
            set => DataContext = value;
        }

        public PortOptionsView()
        {
            InitializeComponent();

            DataContextChanged += DataContextChangedHandler;
            SizeChanged += SizeChangedHandler;
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            ViewModel?.OnViewUpdateEvent();
        }

        private void UpdateLiseViewColumnsSize()
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

        private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ViewModel != null)
            {
                if (e.OldValue is ViewModelBase viewModel)
                {
                    viewModel.CellSizeChanged -= CellElemntSizeChanged;
                    viewModel.UpdateEvent -= UpdateEventHandler;
                }

                ViewModel.CellSizeChanged += CellElemntSizeChanged;
                ViewModel.UpdateEvent += UpdateEventHandler;
            }
        }

        private void UpdateEventHandler(ViewModelBase viewModel)
        {
            UpdateLiseViewColumnsSize();
            UpdateLayout();

            ViewModel?.OnViewUpdateEvent();
        }

        private void CellElemntSizeChanged(ViewModelBase viewModel, ICellElement element)
        {
            UpdateLiseViewColumnsSize();
            UpdateLayout();

            ViewModel?.OnViewUpdateEvent();
        }

        private void ListView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel?.OnViewUpdateEvent();
        }
    }
}
