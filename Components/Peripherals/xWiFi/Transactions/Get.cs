using xLibV100.Peripherals.xWiFi.Types;
using System.Collections.Generic;
using xLibV100;
using xLibV100.Common;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xWiFi.Transactions
{
    public class Get
    {
        public static TxTransaction<CoreInfoT> CoreInfo = new TxTransaction<CoreInfoT>(Action.GET_CORE_INFO);
        public static TxTransaction<Response<ConfigT>, RequestHeaderT> Config = new TxTransaction<Response<ConfigT>, RequestHeaderT>(Action.GET_CONFIG);
        public static TxTransaction<Response<StatusT>, RequestHeaderT> Status = new TxTransaction<Response<StatusT>, RequestHeaderT>(Action.GET_STATUS);
        public static TxTransaction<Response<AddressT>, RequestHeaderT> Address = new TxTransaction<Response<AddressT>, RequestHeaderT>(Action.GET_ADDRESS);

        public struct RequestHeaderT : IRequestAdapter
        {
            public Module Mask;

            public int Add(List<byte> buffer)
            {
                return xMemory.Add(buffer, this);
            }

            public unsafe int GetSize()
            {
                return sizeof(RequestHeaderT);
            }
        }

        public struct ResponseHeaderT
        {
            public byte Mask;
        }

        public class Response<T> : IResponseAdapter where T : unmanaged
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
                int mask = ((ResponseHeaderT*)content.Data)->Mask;
                List<Value> values = new List<Value>();

                content.Data += sizeof(ResponseHeaderT);
                content.DataSize -= sizeof(ResponseHeaderT);

                while (mask > 0)
                {
                    if ((mask & 0x01) > 0)
                    {
                        values.Add(new Value { Number = number });
                    }
                    number++;
                    mask >>= 1;
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
