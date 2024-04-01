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
    public class Events
    {
        public static RxTransaction<EventNewPoints> NewPoint = new RxTransaction<EventNewPoints>(Action.EVENT_NEW_POTINTS);

        public unsafe class EventResult<TResponse> : IResponseAdapter where TResponse : unmanaged
        {
            public TResponse Result;

            public object Recieve(RxPacketManager manager, xContent content)
            {
                Result = *(TResponse*)content.Data;
                return this;
            }
        }
    }
}
