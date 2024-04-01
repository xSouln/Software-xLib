using xLibV100.Peripherals.Common;
using xLibV100;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xADC.Transactions
{
    public class Get
    {
        public unsafe class Response<TValue> : IResponseAdapter where TValue : unmanaged
        {
            public TValue* Values;
            public int Count;

            public object Recieve(RxPacketManager manager, xContent content)
            {
                Values = (TValue*)content.Data;
                Count = content.DataSize / sizeof(TValue);
                return this;
            }
        }
        public unsafe class Response : IResponseAdapter
        {
            public ActionResult Result;

            public object Recieve(RxPacketManager manager, xContent content)
            {
                Result = *(ActionResult*)content.Data;
                return this;
            }
        }
    }
}
