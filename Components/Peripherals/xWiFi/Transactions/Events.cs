using xLibV100.Peripherals.xWiFi.Types;
using System.Collections.Generic;
using xLibV100;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xWiFi.Transactions
{
    public class Events
    {
        public static RxTransaction<Event<StatusT>> StatusChanged = new RxTransaction<Event<StatusT>>(Action.EVENT_STATUS_CHANGED);

        public struct EventHeaderT
        {
            public byte Numbers;
        }

        public class Event<T> : IResponseAdapter where T : unmanaged
        {
            public class Value
            {
                public int Number;
                public T Element;
            }

            public Value[] Values;

            public unsafe object Recieve(RxPacketManager manager, xContent content)
            {
                int number = 0;
                int numbers = ((EventHeaderT*)content.Data)->Numbers;
                List<Value> values = new List<Value>();

                content.Data += sizeof(EventHeaderT);
                content.DataSize -= sizeof(EventHeaderT);

                while (numbers > 0)
                {
                    if ((numbers & 0x01) > 0)
                    {
                        values.Add(new Value { Number = number });
                    }
                    number++;
                    numbers >>= 1;
                }

                if (values.Count == content.DataSize / sizeof(T))
                {
                    for (int i = 0; i < values.Count; i++)
                    {
                        values[i].Element = *(T*)content.Data;
                        content.Data += sizeof(T);
                    }

                    Values = values.ToArray();
                }

                return this;
            }
        }
    }
}
