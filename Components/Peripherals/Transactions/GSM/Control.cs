using System.Collections.Generic;
using xLibV100.Common;
using xLibV100.Transactions;
using xLibV100.Transactions.Common;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.GsmControl.Transactions
{
    public partial class Control : ControlBase
    {
        [TxTransaction(Action = Actions.GetInfo)]
        public readonly TxTransaction<ResponseGetInfo> GetInfo;

        [TxTransaction(Action = Actions.GetInstances)]
        public readonly TxTransaction<ResponeGetInstances, RequestGetInstances> GetInstances;

        [TxTransaction(Action = Actions.GetCredentials)]
        public readonly TxTransaction<ResponseGetInfo, RequestGetByRange> GetCredentials;


        public Control(Gsm model, uint uid) : base(model, uid)
        {

        }


        public class RequestGetByRange : IRequestAdapter
        {
            public byte StartNumber;
            public byte EndNumber;

            public RequestGetByRange(byte startNumber, byte endNumber)
            {
                StartNumber = startNumber;
                EndNumber = endNumber;
            }

            public int Add(List<byte> buffer)
            {
                int size = xMemory.Add(buffer, StartNumber);
                size += xMemory.Add(buffer, EndNumber);

                return size;
            }

            public int GetSize()
            {
                return sizeof(byte) * 2;
            }
        }

        public class RequestGetInstances : IRequestAdapter
        {
            public byte StartNumber;
            public byte EndNumber;

            public RequestGetInstances(byte startNumber, byte endNumber)
            {
                StartNumber = startNumber;
                EndNumber = endNumber;
            }

            public int Add(List<byte> buffer)
            {
                int size = xMemory.Add(buffer, StartNumber);
                size += xMemory.Add(buffer, EndNumber);

                return size;
            }

            public int GetSize()
            {
                return sizeof(byte) * 2;
            }
        }

        public class ResponeGetInstances : IResponseAdapter
        {
            public class Instance
            {
                public InstanceTypes Type;
                public byte Extension;
            }


            public object Recieve(RxPacketManager manager, xContent content)
            {
                return this;
            }
        }


        public class ResponseGetInfo : IResponseAdapter
        {
            public byte CountOfInstances;

            public byte Type;
            public string Descriptions;

            public object Recieve(RxPacketManager manager, xContent content)
            {
                content.Get(out CountOfInstances);
                content.Get(out Type);

                return this;
            }
        }
    }
}
