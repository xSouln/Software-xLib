using System.Collections.Generic;
using xLibV100.Common;
using xLibV100.Transactions.Common;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xADC.Transactions
{
    public class Set
    {
        public static TxTransaction<ResponseResult, RequestSetNotifiedChannels> NotifiedChannels = new TxTransaction<ResponseResult, RequestSetNotifiedChannels>(Action.SET_SET_NOTIFIED_CHANNELS);

        public unsafe class ResponseResult : IResponseAdapter
        {
            public ActionResult Result;

            public object Recieve(RxPacketManager manager, xContent content)
            {
                Result = *(ActionResult*)content.Data;
                return this;
            }
        }

        public unsafe class ResponseResult<TValue> : IResponseAdapter where TValue : unmanaged
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

        public class RequestSetTime : IRequestAdapter
        {
            public int Time;

            public RequestSetTime(int time)
            {
                Time = time;
            }

            public int Add(List<byte> buffer)
            {
                return xMemory.Add(buffer, Time);
            }

            public int GetSize()
            {
                return xMemory.GetSize(Time);
            }
        }
    }
}
