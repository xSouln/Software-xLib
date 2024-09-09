using System;
using System.Collections.Generic;
using xLibV100.Adaptation;
using xLibV100.Common;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.Transactions
{
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
            public byte Type;
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

    public class RequestGetProperties : RequestToInstance
    {
        public ushort[] Properties;

        public RequestGetProperties(ushort selector, byte number = 0) : base(number)
        {
            Properties = new ushort[] { selector };
        }

        public RequestGetProperties(byte number = 0, params object[] properties) : base(number)
        {
            List<ushort> elements = new List<ushort>();

            foreach (var property in properties)
            {
                try
                {
                    var element = (ushort)property;
                    elements.Add(element);
                }
                catch
                {

                }
            }
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
            return base.GetSize() + (Properties != null ? Properties.Length * sizeof(ushort) : 0);
        }
    }

    public class ResponseGetProperties : IResponseAdapter
    {
        public List<ReadableProperty> Properties = new List<ReadableProperty>();

        public unsafe object Recieve(RxPacketManager manager, xContent content)
        {
            /*while (content.DataSize >= sizeof(ReadablePropertyHeaderT))
            {
                content.Get(out ReadablePropertyHeaderT info);

                if (info.Size > content.DataSize)
                {
                    break;
                }

                Properties.Add(new ReadableProperty(info, content.GetSegment(info.Size)));
            }*/

            return this;
        }

        public ReadableProperty GetPropertyById(ushort id)
        {
            foreach (var property in Properties)
            {
                if (property.Info.Id == id)
                {
                    return property;
                }
            }

            return null;
        }
    }


    public class RequestSetProperties : RequestToInstance
    {
        protected List<RequestedWritableProperty> Properties = new List<RequestedWritableProperty>();

        public RequestSetProperties(RequestedWritableProperty property, byte number = 0) : base(number)
        {
            Properties.Add(property);
        }

        public RequestSetProperties(RequestedWritableProperty[] properties, byte number = 0) : base(number)
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

            /*foreach (var property in Properties)
            {
                size += xMemory.Add(buffer, property.Info);
                size += xMemory.Add(buffer, property.Content);
            }*/

            return size;
        }
    }
}
