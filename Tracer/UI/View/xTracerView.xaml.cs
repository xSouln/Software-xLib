using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using xLibV100.Components;
using xLibV100.UI;

namespace xLibV100.Tracer.UI.View
{
    /// <summary>
    /// Логика взаимодействия для xTracerView.xaml
    /// </summary>
    public partial class xTracerView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public TabItem SelectedItem { get; set; }

        public xTracerView()
        {
            InitializeComponent();

            ListViewRequestsInfo.ItemsSource = xTracer.RequestInfo;
            ListViewInfo.ItemsSource = xTracer.Info;

            TabItemRequests.DataContext = xTracer.RequestInfo;
            TabItemInfo.DataContext = xTracer.Info;

            DataContext = this;

            OnPropertyChanged(nameof(ButPauseBackground));
            OnPropertyChanged(nameof(ButPauseName));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Brush ButPauseBackground
        {
            get => xTracer.Pause ? UIProperty.RED : UIProperty.GREEN;
        }

        public string ButPauseName
        {
            get => xTracer.Pause ? "Launch" : "Pause";
        }

        private void ButClear_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem is TabItem tabItem && tabItem.DataContext is IList list)
            {
                list.Clear();
            }
        }

        private void ButPause_Click(object sender, RoutedEventArgs e)
        {
            xTracer.Pause = !xTracer.Pause;

            OnPropertyChanged(nameof(ButPauseBackground));
            OnPropertyChanged(nameof(ButPauseName));
        }
    }
}
