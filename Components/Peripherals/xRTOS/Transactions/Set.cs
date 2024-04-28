using xLibV100.Transactions.Common;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xRTOS.Transactions
{
    public class Set
    {

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
    }
}
