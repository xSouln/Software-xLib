namespace xLibV100.Peripherals.GsmControl.Transactions
{
    public enum Actions : ushort
    {
        Get = Peripherals.Transactions.Actions.GetTransactionsOffset,

        GetCredentials,

        GetStatus,

        GetIMEI,
        GetAPN,
        GetClient,
        GetPassword,

        Set = Peripherals.Transactions.Actions.SetTransactionsOffset,

        SetCredentials,
    }
}
