using xLibV100.Peripherals.xWiFi.Types;
using System.Collections.Generic;
using xLibV100;
using xLibV100.Common;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xWiFi.Transactions
{
    public class Set
    {
        public static TxTransaction<Response, Request<ConfigT>> Config = new TxTransaction<Response, Request<ConfigT>>(Action.SET_CONFIG);

        public unsafe class Response : IResponseAdapter
        {
            public ActionResult Result;

            public object Recieve(RxPacketManager manager, xContent content)
            {
                Result = *(ActionResult*)content.Data;
                return this;
            }
        }

        public unsafe class Response<TValue> : IResponseAdapter where TValue : unmanaged
        {
            public ActionResult Result;
            public TValue* Values;
            public int Count;

            public object Recieve(RxPacketManager manager, xContent content)
            {
                Result = *(ActionResult*)content.Data;
                content.Data += sizeof(ActionResult);

                Values = (TValue*)content.Data;
                Count = content.DataSize / sizeof(TValue);
                return this;
            }
        }

        public class Request<T> : IRequestAdapter where T : unmanaged
        {
            public Module Mask;
            public T Value;

            public int Add(List<byte> buffer)
            {
                int size = xMemory.Add(buffer, Mask);
                size += xMemory.Add(buffer, Value);

                return size;
            }

            public unsafe int GetSize()
            {
                return sizeof(byte) + sizeof(T);
            }
        }
    }
}
