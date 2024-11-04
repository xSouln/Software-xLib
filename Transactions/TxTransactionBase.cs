using System;
using System.Collections.Generic;
using System.Threading;
using xLibV100.Common;
using xLibV100.Transceiver;

namespace xLibV100.Transactions
{
    public class TxTransactionBase<TResponse> : RequestBase<TResponse>
    {
        public PacketHeaderT ResponseHeader;
        public PacketHeaderT RequestHeader;

        public string EndPacket = "\r";

        public uint Id;
    }

    public class TxTransactionBase<TResponse, TAction> : TxTransactionBase<TResponse>, IReceiver
        where TResponse : IResponseAdapter, new()
    {
        protected TAction action;

        public event xEvent<RxPacketManager, TResponse> ResponseReceiver;

        public IResponseAdapter GetResponseAdapter() => Response;

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
                && (packet->Info.Action == Convert.ToUInt16(Action))
                && packet->Info.RequestId == Id)
            {
                if (packet->Header.Identificator == header.Identificator)
                {
                    content.Data += sizeof(PacketT);
                    content.DataSize -= sizeof(PacketT);

                    Response = new TResponse();
                    manager.FoundObject = this;

                    Response.Recieve(manager, content);
                    ResponseReceiver?.Invoke(manager, Response);

                    return ReceiverResult.Accept;
                }
                else if (packet->Header.Description == (int)PacketHeaderDescription.Error)
                {
                    return ReceiverResult.NotSuported;
                }
            }
            return ReceiverResult.NotFound;
        }

        public virtual unsafe TxTransactionBase<TResponse, TAction> Prepare(ResponseHandle handle = null)
        {
            var transaction = (TxTransactionBase<TResponse, TAction>)Create();

            transaction.name = name;
            transaction.action = action;
            transaction.ResponseReceiver += ResponseReceiver;
            transaction.Id = (uint)new Random().Next();

            List<byte> packet = new List<byte>();

            PacketInfoT info = new PacketInfoT
            {
                Action = (ushort)(object)action,
                RequestId = transaction.Id
            };

            PacketBase.Add(packet, RequestHeader);
            PacketBase.Add(packet, info);
            PacketBase.Add(packet, EndPacket);

            transaction.Data = packet.ToArray();
            transaction.handle = handle ?? this.handle;

            return transaction;
        }
    }

    public class TxTransactionBase<TResponse, TAction, TRequest> : TxTransactionBase<TResponse>, IReceiver
        where TResponse : IResponseAdapter, new()
        where TRequest : IRequestAdapter
    {
        protected TAction action;

        public event xEvent<RxPacketManager, TResponse> ResponseReceiver;
        public IResponseAdapter GetResponseAdapter() => Response;

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
                && (packet->Info.Action == Convert.ToUInt16(Action))
                && packet->Info.RequestId == Id)
            {
                if (packet->Header.Identificator == header.Identificator)
                {
                    content.Data += sizeof(PacketT);
                    content.DataSize -= sizeof(PacketT);

                    var response = new TResponse();
                    manager.FoundObject = this;

                    response.Recieve(manager, content);

                    Response = response;
                    ResponseReceiver?.Invoke(manager, Response);

                    return ReceiverResult.Accept;
                }
                else if (packet->Header.Description == (int)PacketHeaderDescription.Error)
                {
                    return ReceiverResult.NotSuported;
                }
            }
            return ReceiverResult.NotFound;
        }

        public unsafe virtual TxTransactionBase<TResponse, TAction, TRequest> Prepare(TRequest request, ResponseHandle handle = null)
        {
            List<byte> content = new List<byte>();
            request.Add(content);

            return Prepare(content.ToArray(), handle: handle);
        }

        public unsafe virtual TxTransactionBase<TResponse, TAction, TRequest> Prepare(byte[] content, ResponseHandle handle = null)
        {
            var transaction = (TxTransactionBase<TResponse, TAction, TRequest>)Create();

            transaction.name = name;
            transaction.action = action;
            transaction.ResponseReceiver += ResponseReceiver;

            randomSynchronize.WaitOne();
            transaction.Id = (uint)random.Next();
            randomSynchronize.Set();

            List<byte> packet = new List<byte>();

            PacketInfoT info = new PacketInfoT
            {
                Action = Convert.ToUInt16(action),
                ContentSize = (ushort)content.Length,
                RequestId = transaction.Id
            };

            xMemory.Add(packet, RequestHeader);
            xMemory.Add(packet, info);
            xMemory.Add(packet, content);
            xMemory.Add(packet, EndPacket);

            transaction.Data = packet.ToArray();
            transaction.handle = handle == null ? this.handle : handle;

            return transaction;
        }
    }
}
