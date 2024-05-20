using xLibV100.Peripherals.xADC.UI.Views;
using xLibV100.Peripherals.xADC.Models;
using xLibV100.Peripherals.xADC.UI.Interfaces;
using xLibV100.Peripherals.xADC.UI.Views.Interfaces;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using xLibV100.Common;
using xLibV100.UI;
using static xLibV100.Peripherals.xADC.Models.Charts;

namespace xLibV100.Peripherals.xADC.UI.Models
{
    public class ChartsViewModel : ViewModelBase<Charts, ChartsView>, IChartsViewModel
    {
        //public virtual System.Windows.Forms.DataVisualization.Charting.Chart Chart { get; set; }
        public virtual System.Windows.Forms.DataVisualization.Charting.Series S1 { get; set; }

        protected int points_per_second = 0;

        protected CancellationTokenSource task_token_source = new CancellationTokenSource();
        protected IWindowsFormsChat IWindowChat { get; set; }
        protected WindowsFormsChat WindowChat { get; set; } = new WindowsFormsChat();

        private Control Control { get; set; }
        public int MaxPontsAxisX { get; set; } = 10000;
        public int PointsPerSecond
        {
            get => points_per_second;
            protected set
            {
                points_per_second = value;
                OnPropertyChanged(nameof(PointsPerSecond));
            }
        }
        protected Thread UpdateGraphThread { get; set; }

        public ChartsViewModel(Charts model) : base(model)
        {
            Name = model.Name;

            Control = model.Parent;

            var chart = WindowChat.Chart;
            //chart.Series.Clear();

            //chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            chart.BackColor = System.Drawing.Color.FromName("#FF641818");
            chart.ForeColor = System.Drawing.Color.FromName("#FFDEC316");
            chart.BorderlineColor = System.Drawing.Color.FromName("#FFDEC316");

            /*
            System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
            contextMenu.MenuItems.Add("Clear", ContextMenuClear_Click);
            contextMenu.MenuItems.Add("Add random", ContextMenuAddRandom_Click);
            contextMenu.MenuItems.Add("Open excel", ContextMenuOpenExcel_Click);
            contextMenu.MenuItems.Add("Open json", ContextMenuOpneJson_Click);
            contextMenu.MenuItems.Add("Save", ContextMenuSave_Click);

            Chart.ContextMenu = contextMenu;
            */
            //var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea { Name = "Area1" };
            //chartArea.BackColor = System.Drawing.Color.FromName("#FF641818");

            //chart.ChartAreas.Add(chartArea);
            chart.ChartAreas[0].BackColor = System.Drawing.Color.FromName("#FF641818");
            chart.ChartAreas[0].AxisY.LineColor = System.Drawing.Color.Green;
            chart.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.Green;

            chart.ChartAreas[0].AxisX.MajorTickMark.LineColor = System.Drawing.Color.Green;
            chart.ChartAreas[0].AxisY.MajorTickMark.LineColor = System.Drawing.Color.Green;

            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Transparent;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Green;

            chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = System.Drawing.Color.Orange;
            chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = System.Drawing.Color.Orange;

            chart.ChartAreas[0].AxisY.Maximum = 4096;
            chart.ChartAreas[0].AxisY.Minimum = 0;

            S1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            S1.Name = nameof(S1);
            S1.LabelBackColor = System.Drawing.Color.FromName("#FF641818");
            S1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            chart.Series.Add(S1);
            chart.Update();

            //var view = new ChartsView(this);
            //View = view;

            View.HostElement = WindowChat;

            //model.Channels[0].PointsEventUpdate += PointsEventUpdate;

            UIModel = View;

            //UpdateGraphThread = new Thread(UpdateGraph);
            //UpdateGraphThread.Start();
            //Task.Run(UpdatePontsPerSecond, task_token_source.Token);
            Task.Run(UpdateGraph, task_token_source.Token);
            Task.Run(UpdatePontsPerSecond, task_token_source.Token);
        }

        private void PointsEventUpdate(Channel channel, ushort[] points)
        {
            xSupport.ActionThreadUI(() =>
            {
                foreach (ushort point in points)
                {
                    S1.Points.Add(point);

                    if (S1.Points.Count > MaxPontsAxisX)
                    {
                        S1.Points.RemoveAt(0);
                    }
                }
            });
        }

        private async void UpdateGraph()
        {
            int handler_index = 0;
            int total_index;
            int size_mask = Model.Channels[0].MaxPointsInArray - 1;
            int[] points = Model.Channels[0].Points;

            Stopwatch update_time = new Stopwatch();
            int delay = 0;

            try
            {
                while (true)
                {
                    delay = 73;

                    total_index = Model.Channels[0].TotalIndex;

                    if (handler_index != total_index)
                    {
                        update_time.Restart();

                        xSupport.ActionThreadUI(() =>
                        {
                            while (handler_index != total_index)
                            {
                                S1.Points.Add(points[handler_index]);
                                handler_index++;
                                handler_index &= size_mask;

                                if (S1.Points.Count > MaxPontsAxisX)
                                {
                                    S1.Points.RemoveAt(0);
                                }
                            }
                        });

                        update_time.Stop();
                        delay -= (int)update_time.ElapsedMilliseconds;
                    }

                    if (delay > 0)
                    {
                        //Thread.Sleep(delay);
                        await Task.Delay(delay, task_token_source.Token);
                    }
                }
            }
            catch
            {

            }
        }

        private async void UpdatePontsPerSecond()
        {
            int previous_received_points = 0;
            try
            {
                while (true)
                {
                    int received_points = Model.Channels[0].ReceivedPoints;

                    PointsPerSecond = received_points - previous_received_points;
                    previous_received_points = received_points;

                    xSupport.ActionThreadUI(() =>
                    {
                        WindowChat.PointsPerSecond = PointsPerSecond;
                    });

                    await Task.Delay(1000, task_token_source.Token);
                }
            }
            catch
            {

            }
        }

        public void ClearChart()
        {
            Model.Channels[0].ClearPoints();
            S1.Points.Clear();
        }

        public async void EnableNotification()
        {
            await Control?.SetNitifiedChannels(0xff, 0xffff);
        }

        public async void DisableNotification()
        {
            await Control?.SetNitifiedChannels(0xff, 0);
        }

        public override void Dispose()
        {
            base.Dispose();

            UpdateGraphThread?.Abort();
            UpdateGraphThread = null;
        }
    }
}
