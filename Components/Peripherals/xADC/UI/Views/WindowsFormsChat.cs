using xLibV100.Peripherals.xADC.UI.Views.Interfaces;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace xLibV100.Peripherals.xADC.UI.Views
{
    public partial class WindowsFormsChat : UserControl, IWindowsFormsChat
    {
        protected int points_per_second = 0;
        public WindowsFormsChat()
        {
            InitializeComponent();
        }

        public int PointsPerSecond
        {
            get => 0;
            set => LabelPointsPerSecond.Text = "Points per second: " + value;
        }

        public Chart Chart => chart1;
    }
}
