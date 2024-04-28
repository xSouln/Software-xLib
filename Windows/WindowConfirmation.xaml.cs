using System.ComponentModel;
using System.Windows;

namespace xLibV100.xWindows
{
    /// <summary>
    /// Логика взаимодействия для WindowConfirmation.xaml
    /// </summary>
    public partial class WindowConfirmation : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string request = "";
        private string note = "";

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public WindowConfirmation()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public string Request
        {
            get => request;
            set
            {
                request = value;
                OnPropertyChanged(nameof(Request));
            }
        }

        public string Note
        {
            get => note;
            set
            {
                note = value;
                OnPropertyChanged(nameof(Note));
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
