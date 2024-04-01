using System;
using System.Windows;

namespace xLibV100
{
    /// <summary>
    /// Логика взаимодействия для InfoWindow.xaml
    /// </summary>
    public partial class TerminalWindow : Window
    {
        private static TerminalWindow window_terminal;

        public TerminalWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        public static void Open_Click(object sender, RoutedEventArgs e)
        {
            if (window_terminal == null)
            {
                window_terminal = new TerminalWindow();
                window_terminal.Closed += new EventHandler(Close_Click);
                window_terminal.Show();
            }
            else window_terminal.Activate();
        }

        public static void Close_Click(object sender, EventArgs e)
        {
            window_terminal?.Close();
            window_terminal = null;
        }

        public static void Close_Click()
        {
            window_terminal?.Close();
            window_terminal = null;
        }
    }
}
