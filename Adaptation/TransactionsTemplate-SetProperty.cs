using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using xLibV100.Transactions.Common;
using xLibV100.Transceiver;
using xLibV100.Common;

namespace xLibV100.Adaptation
{
    public class AdapterSetProperty
    {
        protected TxTransaction<ResponseSetProperty, Request<byte>> transactionTemplate;
        protected TransferRequestReceiver requestReceiver;

        public AdapterSetProperty()
        {

        }

        public AdapterSetProperty(TransactionsBase control, object action)
        {
            transactionTemplate = new TxTransaction<ResponseSetProperty, Request<byte>>(control, action);
        }

        public async Task<IList<ReceivedWritableProperty>> SetPropertyAsync(ushort id,
            byte[] content,
            ushort extension = 0,
            ushort offset = 0,
            IRequestAdapter header = null,
            bool generateTransferException = false,
            bool generateSettingErrorException = false)
        {
            if (requestReceiver == null)
            {
                throw new InvalidOperationException("RequestReceiver is not installed");
            }

            List<byte> data = new List<byte>();

            var request = new RequestSetProperty(id,
                content,
                offset: offset);

            header?.Add(data);
            request.Add(data);

            var transaction = transactionTemplate.Prepare(data.ToArray());

            await requestReceiver?.Invoke(this, transaction);

            if (generateTransferException && transaction.ResponseResult != TxResponses.Accept)
            {
                throw new ArgumentNullException("SetProperty: " + id + " error");
            }

            var result = transaction.Response.GetSettingErrors();

            if (generateSettingErrorException && result != null)
            {
                throw new ArgumentNullException("SetProperty: " + id + " error");
            }

            return result;
        }

        public virtual AdapterSetProperty Prepare(TransferRequestReceiver requestReceiver)
        {
            var result = new AdapterSetProperty();
            result.transactionTemplate = transactionTemplate;
            result.requestReceiver = requestReceiver;

            return result;
        }
    }

    public class RequestSetProperty : IRequestAdapter
    {
        public RequestedWritablePropertyT Info = new RequestedWritablePropertyT();

        public ushort Extension { get; protected set; }

        public ushort Mode { get; protected set; }

        public ushort TypeSize { get; protected set; }

        public ushort Offset { get; protected set; }

        public byte[] Content { get; protected set; }

        public RequestSetProperty(ushort propertyId,
            byte[] content,
            ushort mode = 0,
            ushort extension = 0,
            ushort typeSize = 0x8000,
            ushort offset = 0)
        {
            Info.Id = propertyId;
            Content = content;
            Extension = extension;
            Mode = mode;
            Offset = offset;
            TypeSize = typeSize;

            Info.ModeIsIncluded = mode != 0;
            Info.ExtensionIsIncluded = extension != 0;
            Info.TypeSizeVerificationIsIncluded = typeSize != 0x8000;
            Info.LimitsIsIncluded = offset != 0;
        }

        public virtual int Add(List<byte> buffer)
        {
            ushort contentSize = (ushort)(Content == null ? 0 : Content.Length);

            int size = 0;

            size += xMemory.Add(buffer, Info.Value);
            size += xMemory.Add(buffer, contentSize);

            if (Info.ExtensionIsIncluded)
            {
                size += xMemory.Add(buffer, Extension);
            }

            if (Info.ModeIsIncluded)
            {
                size += xMemory.Add(buffer, Mode);
            }

            if (Info.TypeSizeVerificationIsIncluded)
            {
                size += xMemory.Add(buffer, TypeSize);
            }

            if (Info.LimitsIsIncluded)
            {
                size += xMemory.Add(buffer, Offset);
            }

            size += xMemory.Add(buffer, Content);

            return size;
        }

        public int GetSize()
        {
            return 0;
        }
    }

    public class ResponseSetProperty : IResponseAdapter
    {
        public List<ReceivedWritableProperty> Properties { get; protected set; } = new List<ReceivedWritableProperty>();

        public unsafe object Recieve(RxPacketManager manager, xContent content)
        {
            while (content.DataSize >= sizeof(ReceivedWritablePropertyT))
            {
                while (content.DataSize >= sizeof(ReceivedWritablePropertyT))
                {
                    ushort extension = 0;
                    byte error = 0;

                    content.Get(out ReceivedWritablePropertyT info);

                    if (info.ExtensionIsIncluded)
                    {
                        content.Get(out extension);
                    }

                    content.Get(out error);

                    Properties.Add(new ReceivedWritableProperty(info, extension, (ActionResult)error));
                }
            }

            return this;
        }

        public IList<ReceivedWritableProperty> GetSettingErrors()
        {
            List<ReceivedWritableProperty> response = new List<ReceivedWritableProperty>();

            foreach (var property in Properties)
            {
                if (property.Result != ActionResult.Accept)
                {
                    response.Add(property);
                }
            }

            return response.Count == 0 ? null : response;
        }

        public ReceivedWritableProperty GetById(int id)
        {
            foreach (var property in Properties)
            {
                if (property.Info.Id == (ushort)id)
                {
                    return property;
                }
            }

            return null;
        }
    }
}
