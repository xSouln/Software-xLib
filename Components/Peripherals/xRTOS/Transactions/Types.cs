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
}
