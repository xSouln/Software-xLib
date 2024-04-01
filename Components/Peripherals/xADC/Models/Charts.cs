using xLibV100.Peripherals.xADC.Transactions;
using System.Threading;
using xLibV100.Controls;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xADC.Models
{
    public class Charts : ModelBase<Control>
    {
        public delegate void PointsEventUpdateHandler(Channel channel, int[] points);

        public class Channel
        {
            public int Number;
            public int ReceivedPoints;

            public int MaxPointsInArray => SizeMask + 1;

            public Charts Parent;

            public int[] Points;
            protected AutoResetEvent synchronize = new AutoResetEvent(true);

            public PointsEventUpdateHandler PointsEventUpdate;

            public int TotalIndex = 0;
            public int HandlerIndex = 0;
            protected int SizeMask = 0;

            public Channel(int size_mask)
            {
                SizeMask = size_mask;
                Points = new int[size_mask + 1];
            }

            public void AddPoints(int[] points)
            {
                //synchronize.WaitOne();

                foreach (var point in points)
                {
                    Points[TotalIndex] = point;
                    TotalIndex++;
                    TotalIndex &= SizeMask;

                    ReceivedPoints++;
                }

                //synchronize.Set();
                //PointsEventUpdate?.Invoke(this, points);
            }

            public int ReadPoints(int[] points)
            {
                //synchronize.WaitOne();

                int i = 0;
                while (i < points.Length && HandlerIndex != TotalIndex)
                {
                    points[i] = (ushort)Points[HandlerIndex];
                    HandlerIndex++;
                    HandlerIndex &= SizeMask;

                    i++;
                }

                //synchronize.Set();

                return i;
            }

            public void ClearPoints()
            {
                //synchronize.WaitOne();

                TotalIndex = 0;
                HandlerIndex = 0;
                ReceivedPoints = 0;

                //synchronize.Set();
            }
        }

        public Channel[] Channels = new Channel[16];

        public Charts(Control control) : base(control)
        {
            Name = nameof(Charts);

            for (int i = 0; i < Channels.Length; i++)
            {
                Channels[i] = new Channel(0xffff)
                {
                    Number = i,
                    Parent = this,
                };
            }

            Events.NewPoint.EventReceive += NewPointEventReceive;

            //ViewModel = new ChartsViewModel(this);
        }

        private void NewPointEventReceive(RxPacketManager obj, EventNewPoints arg)
        {
            if (arg.Channels != null && arg.Channels.Length > 0)
            {
                foreach (var channel in arg.Channels)
                {
                    if (channel.Number == 0)
                    {
                        Channels[channel.Number].AddPoints(channel.Points);
                        return;
                    }
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
