using xLibV100.Transactions;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xADC.Transactions
{
    public sealed class TxTransaction<TResponse> : TxTransactionBase<TResponse, Action> where TResponse : IResponseAdapter, new()
    {
        readonly PacketHeaderT txHeader = PacketHeaderT.Init(PacketHeaderDescription.Request, Info.Address, Info.UID);
        readonly PacketHeaderT rxHeader = PacketHeaderT.Init(PacketHeaderDescription.Response, Info.Address, Info.UID);

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
        readonly PacketHeaderT txHeader = PacketHeaderT.Init(PacketHeaderDescription.Request, Info.Address, Info.UID);
        readonly PacketHeaderT rxHeader = PacketHeaderT.Init(PacketHeaderDescription.Response, Info.Address, Info.UID);

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
