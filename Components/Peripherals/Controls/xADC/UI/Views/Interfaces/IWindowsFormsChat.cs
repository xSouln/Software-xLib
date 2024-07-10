using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace xLibV100.Peripherals.xADC.UI.Views.Interfaces
{
    public interface IWindowsFormsChat
    {
        int PointsPerSecond { get; set; }
        Chart Chart { get; }
    }
}
