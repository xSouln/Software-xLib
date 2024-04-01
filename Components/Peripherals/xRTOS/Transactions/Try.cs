using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLibV100;
using xLibV100.Common;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xRTOS.Transactions
{
    public class Try
    {
        public unsafe class ResponseResult<TResult> : IResponseAdapter where TResult : unmanaged
        {
            public TResult Result;

            public object Recieve(RxPacketManager manager, xContent content)
            {
                Result = *(TResult*)content.Data;
                return this;
            }
        }

        public unsafe class ResponseResult<TResult, TValue> : IResponseAdapter where TResult : unmanaged where TValue : unmanaged
        {
            public TResult Result;
            public TValue* Values;
            public int Count;

            public object Recieve(RxPacketManager manager, xContent content)
            {
                Result = *(TResult*)content.Data;

                Values = (TValue*)(content.Data + sizeof(TResult));
                Count = content.DataSize / sizeof(TValue);

                return this;
            }
        }
    }
}
