namespace xLibV100.Peripherals.MemoryControl.Transactions
{
    public enum Actions : ushort
    {
        OffsetGet = Peripherals.Transactions.Actions.GetTransactionsOffset,
        GetFileInfo,

        OffsetSet = Peripherals.Transactions.Actions.SetTransactionsOffset,


        OffsetTry = Peripherals.Transactions.Actions.TryTransactionsOffset,
        ReadFile,
        WriteFile,
        DeleteFile,

        OffsetEvent = Peripherals.Transactions.Actions.EvtTransactionsOffdet,
    }
}
