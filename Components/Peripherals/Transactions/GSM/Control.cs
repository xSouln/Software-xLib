using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using xLibV100.Adaptation;
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

        [TxTransaction(Action = Actions.GetProperties)]
        public readonly TxTransaction<ResponseGetProperties, RequestGetProperties> GetProperties;

        [TxTransaction(Action = Actions.SetProperties)]
        public readonly TxTransaction<ResponseGetProperties, RequestSetProperties> SetProperties;


        public Control(Gsm model, uint uid) : base(model, uid)
        {

        }
    }

    public class RequestToInstance : IRequestAdapter
    {
        public byte Number;

        public RequestToInstance(byte number = 0)
        {
            Number = number;
        }

        public virtual int Add(List<byte> buffer)
        {
            return xMemory.Add(buffer, Number);
        }

        public virtual unsafe int GetSize()
        {
            return sizeof(byte);
        }
    }

    public class RequestGetProperties : RequestToInstance
    {
        public PropertySelector[] Properties;

        public RequestGetProperties(PropertySelector selector, byte number = 0) : base(number)
        {
            Properties = new PropertySelector[] { selector };
        }

        public RequestGetProperties(PropertySelector[] properties, byte number = 0) : base(number)
        {
            Properties = properties;
        }

        public override int Add(List<byte> buffer)
        {
            int size = base.Add(buffer);

            if (Properties != null)
            {
                foreach (var property in Properties)
                {
                    size += xMemory.Add(buffer, property);
                }
            }

            return size;
        }

        public override unsafe int GetSize()
        {
            return base.GetSize() + (Properties != null ? Properties.Length * sizeof(PropertySelector) : 0);
        }
    }


    public class ResponseGetProperties : IResponseAdapter
    {
        public List<SynchronizedProperty> Properties = new List<SynchronizedProperty>();

        public unsafe object Recieve(RxPacketManager manager, xContent content)
        {
            while (content.DataSize >= sizeof(SynchronizedPropertyInfoT))
            {
                content.Get(out SynchronizedPropertyInfoT info);

                if (info.Size > content.DataSize)
                {
                    break;
                }

                Properties.Add(new SynchronizedProperty(info, content.GetSegment(info.Size)));
            }

            return this;
        }

        public SynchronizedProperty GetPropertyById(PropertySelector id)
        {
            foreach (var property in Properties)
            {
                if (property.Id == (ushort)id)
                {
                    return property;
                }
            }

            return null;
        }
    }


    public class RequestSetProperties : RequestToInstance
    {
        protected List<SynchronizedPropertyForSetting> Properties = new List<SynchronizedPropertyForSetting>();

        public RequestSetProperties(SynchronizedPropertyForSetting property, byte number = 0) : base(number)
        {
            Properties.Add(property);
        }

        public RequestSetProperties(SynchronizedPropertyForSetting[] properties, byte number = 0) : base(number)
        {
            if (properties == null || properties.Length == 0)
            {
                throw new ArgumentException();
            }

            Properties.AddRange(properties);
        }

        public override int Add(List<byte> buffer)
        {
            int size = base.Add(buffer);

            foreach (var property in Properties)
            {
                size += xMemory.Add(buffer, property.Info);
                size += xMemory.Add(buffer, property.Content);
            }

            return size;
        }
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
