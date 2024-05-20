using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using xLibV100.UI;

namespace xLibV100.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogWindowPresenter.xaml
    /// </summary>
    public partial class DialogWindowPresenter : Window
    {
        protected UIElement view = null;
        protected ViewModelBase viewModel = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public DialogWindowPresenter()
        {
            InitializeComponent();

            DataContext = this;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public UIElement View
        {
            get => view;
            set
            {
                if (value != view)
                {
                    view = value;
                    ContentControl.Content = value;
                    OnPropertyChanged(nameof(View));
                }
            }
        }

        public ViewModelBase ViewModel
        {
            get => viewModel;
            set
            {
                if (value != viewModel)
                {
                    viewModel = value;
                    View = viewModel.View as UIElement;
                    OnPropertyChanged(nameof(ViewModel));
                }
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
