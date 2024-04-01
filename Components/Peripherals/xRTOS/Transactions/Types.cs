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

    public enum ActionResult : ushort
    {
        ACCEPT = 0,
        ERROR_DATA,
        ERROR_CONTENT_SIZE,
        ERROR_REQUEST,
        ERROR_RESOLUTION,
        UNKNOWN_COMMAND,
        BUSY,
        OUTSIDE,
        ERROR_ACTION
    }
}
