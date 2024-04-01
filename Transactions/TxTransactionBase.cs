using System;
using System.Collections.Generic;
using System.Threading;
using xLibV100.Transceiver;

namespace xLibV100.Transactions
{
    public class TxTransactionBase : RequestBase
    {
        public PacketHeaderT ResponseHeader;
        public PacketHeaderT RequestHeader;

        public string EndPacket = "\r";

        public uint Id;
    }

    public class TxTransactionBase<TResponse, TAction> : TxTransactionBase, IReceiver
        where TResponse : IResponseAdapter, new()
    {
        protected TResponse response;
        protected TAction action;

        public event xEvent<RxPacketManager, TResponse> ResponseReceiver;
        public TResponse Response => response;
        public IResponseAdapter GetResponseAdapter() => response;

        public void SetResponseReceiver(xEvent<RxPacketManager, TResponse> receiver)
        {
            ResponseReceiver = receiver;
        }


        public TAction Action
        {
            get => action;
            set => action = value;
        }

        public TxTransactionBase(TAction action) : this()
        {
            Name = "" + action;
            this.action = action;
        }

        public TxTransactionBase()
        {
            receiver = this;
        }

        protected virtual object Create()
        {
            return new TxTransactionBase<TResponse, TAction>();
        }

        public virtual unsafe ReceiverResult Receive(RxPacketManager manager, xContent content)
        {
            PacketT* packet = (PacketT*)content.Data;
            PacketHeaderT header = ResponseHeader;

            if (packet->Header.DeviceId == header.DeviceId
                && packet->Header.UID == header.UID
                && (packet->Info.Action == (ushort)(object)Action)
                && packet->Info.RequestId == Id)
            {
                if (packet->Header.Identificator == header.Identificator)
                {
                    content.Data += sizeof(PacketT);
                    content.DataSize -= sizeof(PacketT);

                    response = new TResponse();
                    manager.FoundObject = this;

                    response.Recieve(manager, content);
                    ResponseReceiver?.Invoke(manager, response);

                    return ReceiverResult.Accept;
                }
                else if (packet->Header.Description == (int)PacketHeaderDescription.Error)
                {
                    return ReceiverResult.NotSuported;
                }
            }
            return ReceiverResult.NotFound;
        }

        protected virtual unsafe TxTransactionBase<TResponse, TAction> Prepare(ResponseHandle handle)
        {
            var transaction = (TxTransactionBase<TResponse, TAction>)Create();

            transaction.name = name;
            transaction.action = action;
            transaction.ResponseReceiver += ResponseReceiver;
            transaction.Id = (uint)new Random().Next();

            List<byte> request_data = new List<byte>();

            PacketInfoT info = new PacketInfoT
            {
                Action = (ushort)(object)action,
                RequestId = transaction.Id
            };

            PacketBase.Add(request_data, RequestHeader);
            PacketBase.Add(request_data, info);
            PacketBase.Add(request_data, EndPacket);

            transaction.Data = request_data.ToArray();
            transaction.handle = handle;

            return transaction;
        }

        public virtual unsafe TxTransactionBase<TResponse, TAction> Prepare()
        {
            return Prepare(handle);
        }
    }

    public class TxTransactionBase<TResponse, TAction, TRequest> : TxTransactionBase, IReceiver
        where TResponse : IResponseAdapter, new()
        where TRequest : IRequestAdapter
    {
        protected TResponse response;
        protected TAction action;

        public event xEvent<RxPacketManager, TResponse> ResponseReceiver;
        public TResponse Response => response;
        public IResponseAdapter GetResponseAdapter() => response;

        protected static Random random = new Random();
        protected static AutoResetEvent randomSynchronize = new AutoResetEvent(true);


        public TAction Action
        {
            get => action;
            set => action = value;
        }

        public TxTransactionBase(TAction action) : this()
        {
            Name = "" + action;
            this.action = action;
        }

        public TxTransactionBase()
        {
            receiver = this;
        }

        public void SetResponseReceiver(xEvent<RxPacketManager, TResponse> receiver)
        {
            ResponseReceiver = receiver;
        }

        protected virtual object Create()
        {
            return new TxTransactionBase<TResponse, TAction>();
        }

        public virtual unsafe ReceiverResult Receive(RxPacketManager manager, xContent content)
        {
            PacketT* packet = (PacketT*)content.Data;
            PacketHeaderT header = ResponseHeader;

            if (packet->Header.DeviceId == header.DeviceId
                && packet->Header.UID == header.UID
                && (packet->Info.Action == (ushort)(object)Action)
                && packet->Info.RequestId == Id)
            {
                if (packet->Header.Identificator == header.Identificator)
                {
                    content.Data += sizeof(PacketT);
                    content.DataSize -= sizeof(PacketT);

                    response = new TResponse();
                    manager.FoundObject = this;

                    response.Recieve(manager, content);
                    ResponseReceiver?.Invoke(manager, response);

                    return ReceiverResult.Accept;
                }
                else if (packet->Header.Description == (int)PacketHeaderDescription.Error)
                {
                    return ReceiverResult.NotSuported;
                }
            }
            return ReceiverResult.NotFound;
        }

        protected unsafe virtual TxTransactionBase<TResponse, TAction, TRequest> Prepare(ResponseHandle handle, TRequest request)
        {
            var transaction = (TxTransactionBase<TResponse, TAction, TRequest>)Create();

            transaction.name = name;
            transaction.action = action;
            transaction.ResponseReceiver += ResponseReceiver;

            randomSynchronize.WaitOne();
            transaction.Id = (uint)random.Next();
            randomSynchronize.Set();

            List<byte> request_data = new List<byte>();

            PacketInfoT info = new PacketInfoT
            {
                Action = (ushort)(object)action,
                ContentSize = (ushort)request.GetSize(),
                RequestId = transaction.Id
            };

            PacketBase.Add(request_data, RequestHeader);
            PacketBase.Add(request_data, info);
            PacketBase.Aggregate(request_data, request);
            PacketBase.Add(request_data, EndPacket);

            transaction.Data = request_data.ToArray();
            transaction.handle = handle;

            return transaction;
        }

        public virtual unsafe TxTransactionBase<TResponse, TAction, TRequest> Prepare(TRequest request)
        {
            return Prepare(handle, request);
        }
    }
}
