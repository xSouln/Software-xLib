using System.Collections.Generic;
using xLibV100.Transceiver;

namespace xLibV100.Transactions
{
    public class RxTransactionBase<TResponse, TAction> : ReceiverBase
        where TResponse : IResponseAdapter, new()
    {
        protected TAction action;

        public xEvent<RxPacketManager, TResponse> EventReceive { get; set; }

        public TAction Action
        {
            get => action;
            set => action = value;
        }

        public PacketHeaderT ResponseHeader;

        public TResponse Response
        {
            get => (TResponse)result;
            set => result = value;
        }

        public RxTransactionBase()
        {

        }

        public RxTransactionBase(TAction action) : this()
        {
            this.action = action;
            name = "" + action;
        }

        public RxTransactionBase(List<ReceiverBase> responses, TAction action) : this(action)
        {
            responses?.Add(this);
        }

        public override unsafe ReceiverResult Receive(RxPacketManager manager, xContent content)
        {
            PacketT* packet = (PacketT*)content.Data;
            PacketHeaderT header = ResponseHeader;

            if (packet->Header.DeviceId == header.DeviceId
                && packet->Header.UID == header.UID
                && (packet->Info.Action == (ushort)(object)Action))
            {
                content.Data += sizeof(PacketT);
                content.DataSize -= sizeof(PacketT);

                TResponse result = new TResponse();
                manager.FoundObject = this;

                result.Recieve(manager, content);

                Response = result;

                EventReceive?.Invoke(manager, result);

                receiverResult = ReceiverResult.Accept;

                return receiverResult;
            }

            return ReceiverResult.NotFound;
        }
    }
}
