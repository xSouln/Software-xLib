namespace xLibV100.Transactions.Common
{
    public enum ActionResult : ushort
    {
        Accept,
        Error,

        InvalidParameter,
        Busy,
        TimeOut,
        NotSupported,
        ValueIsNotFound,
        RequestIsNotFound,
        LinkError,
        InitializationError,
        OutOfRange,
        NotFound,
        WaitOperation,
        InProgress,
        MemoryAllocationError,
        ConnectionError,
        InvalidRequest,
        GettingResourceError,
        NotIdentified
    }
}
