using xLibV100.Attributes;

namespace xLibV100.Peripherals.GsmControl.Transactions
{
    public enum Actions : ushort
    {
        Get = 20,

        GetInfo,
        GetInstances,

        GetCredentials,
        GetInstanceInfo,

        GetIMEI,
        GetAPN,
        GetClient,
        GetPassword,
        GetProperties,

        Set = 1000,

        SetCredentials,
        SetProperties,
    }
}
