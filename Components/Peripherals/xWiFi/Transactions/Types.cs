using xLibV100.Peripherals.xWiFi.Types;
using System.Collections.Generic;
using xLibV100.Common;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xWiFi.Transactions
{
    public enum Action : ushort
    {
        GET = 100,
        GET_CORE_INFO,
        GET_CONFIG,
        GET_STATUS,
        GET_ADDRESS,

        SET = 1000,
        SET_CONFIG,

        TRY = 2000,
        TRY_ENABLE,
        TRY_DISABLE,

        EVT = 10000,
        EVENT_STATUS_CHANGED
    }

    public struct RequestSetConfig : IRequestAdapter
    {
        public byte Mask;
        public ConfigT Value;

        public unsafe int Add(List<byte> buffer)
        {
            return xMemory.Add(buffer, this);
        }

        public unsafe int GetSize()
        {
            return sizeof(RequestSetConfig);
        }
    }
}