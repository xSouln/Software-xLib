using System.Collections.Generic;
using System.Threading.Tasks;
using xLibV100.Transactions.Common;
using xLibV100.Common;
using xLibV100.Transceiver;
using System;
using System.Linq;

namespace xLibV100.Adaptation
{
    public class AdapterGetProperty
    {
        protected TxTransaction<ResponseGetProperty, Request<byte>> transactionTemplate;
        protected TransferRequestReceiver requestReceiver;


        public AdapterGetProperty()
        {

        }

        public AdapterGetProperty(TransactionsBase control, object action)
        {
            transactionTemplate = new TxTransaction<ResponseGetProperty, Request<byte>>(control, action);
        }

        public async Task<ReceivedRedableProperty> GetPropertyAsync(ushort id,
            ushort extension = 0,
            bool generateException = false,
            PropertyRangeArg range = null,
            IRequestAdapter header = null)
        {
            if (requestReceiver == null)
            {
                throw new InvalidOperationException("RequestReceiver is not installed");
            }

            List<byte> data = new List<byte>();

            var request = new RequestGetProperty(id,
                extension: extension,
                range: range);

            header?.Add(data);
            request.Add(data);

            var transaction = transactionTemplate.Prepare(data.ToArray());
            await requestReceiver?.Invoke(this, transaction);

            ReceivedRedableProperty result = null;

            if (transaction.ResponseResult == TxResponses.Accept)
            {
                result = transaction.Response.GetById((ushort)id);
            }

            if (generateException && result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return result;
        }

        public virtual AdapterGetProperty Prepare(TransferRequestReceiver requestReceiver)
        {
            var result = new AdapterGetProperty();
            result.transactionTemplate = transactionTemplate;
            result.requestReceiver = requestReceiver;

            return result;
        }
    }

    public class PropertyRangeArg
    {
        public ushort StartElement { get; set; }
        public ushort EndElement { get; set; }

        public PropertyRangeArg(ushort startElement, ushort endElement)
        {
            StartElement = startElement;
            EndElement = endElement;
        }
    }

    public class RequestGetProperty : IRequestAdapter
    {
        public RequestedRedablePropertyT PropertyHeader = new RequestedRedablePropertyT();

        public ushort Extension { get; protected set; }

        public ushort Mode { get; protected set; }

        public PropertyRangeArg Range { get; protected set; }

        public IRequestAdapter CustomHeader { get; protected set; }


        public RequestGetProperty(ushort propertyId,
            ushort mode = 0,
            ushort extension = 0,
            bool valueIsExcluded = false,
            bool typeInfoIsIncluded = false,
            bool sizeInfoIsIncluded = false,
            PropertyRangeArg range = null,
            IRequestAdapter customHeader = null)
        {
            PropertyHeader.Id = propertyId;
            Mode = mode;
            Extension = extension;

            Range = range;

            PropertyHeader.ExtensionIsIncluded = Extension != 0;
            PropertyHeader.ModeIsIncluded = Mode != 0;
            PropertyHeader.LimitsIsIncluded = Range != null;

            PropertyHeader.ResponseTypeInfoIsIncluded = typeInfoIsIncluded;
            PropertyHeader.ResponseSizeInfoIsIncluded = sizeInfoIsIncluded;
            PropertyHeader.ResponseValueIsExcluded = valueIsExcluded;

            CustomHeader = customHeader;
        }

        public virtual int Add(List<byte> buffer)
        {
            int size = CustomHeader != null ? CustomHeader.Add(buffer) : 0;

            size += xMemory.Add(buffer, PropertyHeader.Value);

            if (Extension != 0)
            {
                size += xMemory.Add(buffer, Extension);
            }

            if (Mode != 0)
            {
                size += xMemory.Add(buffer, Mode);
            }

            if (Range != null)
            {
                size += xMemory.Add(buffer, Range.StartElement);
                size += xMemory.Add(buffer, Range.EndElement);
            }

            return size;
        }

        public virtual unsafe int GetSize()
        {
            int size = CustomHeader != null ? CustomHeader.GetSize() : 0;

            size = sizeof(ushort); //PropertyId

            if (Extension != 0)
            {
                size += sizeof(ushort); //Extension
            }

            if (Mode != 0)
            {
                size += sizeof(RequestedPropertyMode);
            }

            if (Range != null)
            {
                size += sizeof(ushort);
                size += sizeof(ushort);
            }

            return size;
        }
    }

    public class ResponseGetProperty : IResponseAdapter
    {
        public List<ReceivedRedableProperty> Properties { get; protected set; } = new List<ReceivedRedableProperty>();

        public ReceivedRedableProperty Property => Properties != null && Properties.Count > 0 ? Properties[0] : null;

        public unsafe object Recieve(RxPacketManager manager, xContent content)
        {
            int conversationResult = 0;

            while (content.DataSize >= sizeof(ReceivedRedablePropertyT))
            {
                List<byte[]> elements = new List<byte[]>();

                ushort extension = 0;
                ushort typeSize = 0;
                ushort countOfElements = 1;
                RedablePropertyInfoT propertyInfo = new RedablePropertyInfoT();

                conversationResult = content.Get(out ReceivedRedablePropertyT info);

                if (info.ErrorIsOccured)
                {
                    continue;
                }

                if (info.TypeInfoIsIncluded)
                {
                    content.Get(out propertyInfo);
                }

                if (info.ExtensionIsIncluded)
                {
                    content.Get(out extension);
                }

                if (info.TypeSizeIsIncluded)
                {
                    content.Get(out typeSize);

                    bool countOfElementsIsIncluded = (typeSize & 0x8000) > 0;
                    unchecked
                    {
                        typeSize &= (ushort)~0x8000;
                    }

                    if (countOfElementsIsIncluded)
                    {
                        content.Get(out countOfElements);
                    }
                }
                else
                {
                    typeSize = (ushort)content.DataSize;
                }

                if (info.ValueIsExcluded)
                {
                    goto add;
                }

                int contentSize = typeSize * countOfElements;

                while (contentSize > 0)
                {
                    content.Get(out byte[] propertyContent, typeSize);
                    elements.Add(propertyContent);

                    contentSize -= typeSize;
                }

            add:;
                Properties.Add(new ReceivedRedableProperty(info,
                    propertyInfo,
                    extension,
                    elements: elements));
            }

            return this;
        }

        public ReceivedRedableProperty GetById(int id)
        {
            return Properties.FirstOrDefault(element => element.ReceivedInfo.Id == id);
        }

        public ReceivedRedableProperty this[int index]
        {
            get
            {
                if (index < 0 || index >= Properties.Count)
                {
                    throw new IndexOutOfRangeException("index");
                }

                return Properties[index];
            }
        }

        public TValue GetValueById<TValue>(int id, int elementIndex = 0, bool checkOversize = false)
            where TValue : unmanaged
        {
            ReceivedRedableProperty property = GetById(id) ?? throw new MissingMemberException();
            try
            {
                return property.GetElement<TValue>(elementIndex, checkOversize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TValue GetValueByIndex<TValue>(int index = 0, int elementIndex = 0, bool checkOversize = false)
            where TValue : unmanaged
        {
            if (index < 0 || index >= Properties.Count)
            {
                throw new IndexOutOfRangeException();
            }

            try
            {
                return Properties[index].GetElement<TValue>(elementIndex, checkOversize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
