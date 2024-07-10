using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLibV100;
using xLibV100.Common;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xADC.Transactions
{
    public enum Action : ushort
    {
        GET = 100,
        GET_TIME,

        SET = 1000,
        SET_SET_NOTIFIED_CHANNELS,

        TRY = 2000,

        EVT = 10000,
        EVENT_NEW_POTINTS
    }

    public struct RequestSetNotifiedChannels : IRequestAdapter
    {
        public byte ADCs;
        public byte Action;
        public ushort Channels;

        public int Add(List<byte> buffer)
        {
            return xMemory.Add(buffer, this);
        }

        public unsafe int GetSize()
        {
            return sizeof(RequestSetNotifiedChannels);
        }
    }

    public class EventNewPoints : IResponseAdapter
    {
        private struct PacketHeader
        {
#pragma warning disable CS0649 // Полю "EventNewPoints.PacketHeader.Channels" нигде не присваивается значение, поэтому оно всегда будет иметь значение по умолчанию 0.
            public ushort Channels;
#pragma warning restore CS0649 // Полю "EventNewPoints.PacketHeader.Channels" нигде не присваивается значение, поэтому оно всегда будет иметь значение по умолчанию 0.
#pragma warning disable CS0649 // Полю "EventNewPoints.PacketHeader.PointsCount" нигде не присваивается значение, поэтому оно всегда будет иметь значение по умолчанию 0.
            public ushort PointsCount;
#pragma warning restore CS0649 // Полю "EventNewPoints.PacketHeader.PointsCount" нигде не присваивается значение, поэтому оно всегда будет иметь значение по умолчанию 0.
#pragma warning disable CS0649 // Полю "EventNewPoints.PacketHeader.PointSize" нигде не присваивается значение, поэтому оно всегда будет иметь значение по умолчанию 0.
            public byte PointSize;
#pragma warning restore CS0649 // Полю "EventNewPoints.PacketHeader.PointSize" нигде не присваивается значение, поэтому оно всегда будет иметь значение по умолчанию 0.
#pragma warning disable CS0649 // Полю "EventNewPoints.PacketHeader.PointAligment" нигде не присваивается значение, поэтому оно всегда будет иметь значение по умолчанию 0.
            public byte PointAligment;
#pragma warning restore CS0649 // Полю "EventNewPoints.PacketHeader.PointAligment" нигде не присваивается значение, поэтому оно всегда будет иметь значение по умолчанию 0.

            public int[] AvailableChannels
            {
                get
                {
                    List<int> result = new List<int>();
                    int step = 0;

                    ushort available_channels = Channels;

                    while (available_channels > 0)
                    {
                        if ((available_channels & 0x01) > 0)
                        {
                            result.Add(step);
                        }

                        step++;
                        available_channels >>= 1;
                    }

                    return result.ToArray();
                }
            }
        }

        public class Channel
        {
            public int Number;
            public int[] Points;

            public Channel(int number, int points_count)
            {
                Number = number;
                Points = new int[points_count];
            }
        }

        private PacketHeader Header;
        public Channel[] Channels;

        public unsafe object Recieve(RxPacketManager manager, xContent content)
        {
            Header = *(PacketHeader*)content.Data;

            content.Data += sizeof(PacketHeader);
            content.DataSize -= sizeof(PacketHeader);

            int[] available_channels = Header.AvailableChannels;

            if (Header.PointsCount * available_channels.Length * Header.PointSize == content.DataSize)
            {
                Channels = new Channel[available_channels.Length];

                for (int i = 0; i < available_channels.Length; i++)
                {
                    Channels[i] = new Channel(available_channels[i], Header.PointsCount);

                    ushort* points = (ushort*)content.Data + i;

                    for (int j = 0; j < Channels[i].Points.Length; j++)
                    {
                        Channels[i].Points[j] = *points;

                        points += available_channels.Length;
                    }
                }
            }

            return this;
        }
    }
}
