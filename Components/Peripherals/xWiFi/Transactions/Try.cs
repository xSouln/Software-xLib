using xLibV100.Peripherals.xWiFi.Types;
using System.Collections.Generic;
using xLibV100.Common;
using xLibV100.Transceiver;
using xLibV100.Transactions.Common;

namespace xLibV100.Peripherals.xWiFi.Transactions
{
    public class Try
    {
        public static TxTransaction<Response, Request> Enable = new TxTransaction<Response, Request>(Action.TRY_ENABLE);
        public static TxTransaction<Response, Request> Disable = new TxTransaction<Response, Request>(Action.TRY_DISABLE);

        public unsafe class Response : IResponseAdapter
        {
            public ActionResult Result;

            public object Recieve(RxPacketManager manager, xContent content)
            {
                Result = *(ActionResult*)content.Data;
                return this;
            }
        }

        public class Request : IRequestAdapter
        {
            public Module Mask;

            public int Add(List<byte> buffer)
            {
                int size = xMemory.Add(buffer, Mask);

                return size;
            }

            public unsafe int GetSize()
            {
                return sizeof(byte);
            }
        }
    }
}
