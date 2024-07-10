using xLibV100.Transactions;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xWiFi.Transactions
{
    public class RxTransaction<TResponse> : RxTransactionBase<TResponse, Action> where TResponse : IResponseAdapter, new()
    {
        public RxTransaction(Action action) : base(action)
        {
            ResponseHeader = PacketHeaderT.Init(PacketHeaderDescription.Event, Info.Address, Info.UID);
        }
    }
}
