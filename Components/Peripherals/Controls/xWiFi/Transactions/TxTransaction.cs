using xLibV100.Transactions;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xWiFi.Transactions
{
    public class TxTransaction<TResponse> : TxTransactionBase<TResponse, Action> where TResponse : IResponseAdapter, new()
    {
        protected static PacketHeaderT txHeader = PacketHeaderT.Init(PacketHeaderDescription.Request, Info.Address, Info.UID);
        protected static PacketHeaderT rxHeader = PacketHeaderT.Init(PacketHeaderDescription.Response, Info.Address, Info.UID);

        public TxTransaction(Action action) : base(action)
        {
            RequestHeader = txHeader;
            ResponseHeader = rxHeader;
        }

        protected override object Create()
        {
            return new TxTransaction<TResponse>(action);
        }
    }

    public class TxTransaction<TResponse, TRequest> : TxTransactionBase<TResponse, Action, TRequest>
        where TResponse : IResponseAdapter, new() where TRequest : IRequestAdapter
    {
        protected static PacketHeaderT txHeader = PacketHeaderT.Init(PacketHeaderDescription.Request, Info.Address, Info.UID);
        protected static PacketHeaderT rxHeader = PacketHeaderT.Init(PacketHeaderDescription.Response, Info.Address, Info.UID);

        public TxTransaction(Action action) : base(action)
        {
            RequestHeader = txHeader;
            ResponseHeader = rxHeader;
        }

        protected override object Create()
        {
            return new TxTransaction<TResponse, TRequest>(action);
        }
    }
}
