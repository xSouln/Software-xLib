namespace xLibV100.Peripherals.Transactions
{
    public enum Actions : ushort
    {
        GetTransactions = 32,

        GetInfo,
        GetInstances,
        GetProperty,

        GetTransactionsOffset = GetTransactions + 32,


        SetTransactions = 1000,

        SetProperty,

        SetTransactionsOffset = SetTransactions + 32,


        TryTransactions = 2000,

        TryTransactionsOffset = TryTransactions + 32,


        EvtTransactions = 3000,

        EvtTransactionsOffdet = EvtTransactions + 32
    }
}
