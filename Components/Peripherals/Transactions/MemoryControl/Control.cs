using System.Collections.Generic;
using xLibV100.Common;
using xLibV100.Transactions;
using xLibV100.Transactions.Common;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.MemoryControl.Transactions
{
    public class Control : ControlBase
    {
        [TxTransaction(Action = Actions.DeleteFile)]
        public readonly TxTransaction<ResponseResult, RequestDeleteFile> DeleteFile;

        public Control() : base(null, Info.UID)
        {

        }

        public class RequestDeleteFile : IRequestAdapter
        {
            public byte[] FileName { get; protected set; }
            public byte ControlNumber { get; protected set; }

            public RequestDeleteFile(byte[] fileName, int controlNumber = 0)
            {
                FileName = fileName;
                ControlNumber = (byte)controlNumber;
            }

            public int Add(List<byte> buffer)
            {
                int size = xMemory.Add(buffer, ControlNumber);

                size += xMemory.Add(buffer, (byte)(FileName != null ? FileName.Length : 0));
                size += xMemory.Add(buffer, FileName);

                return size;
            }

            public int GetSize()
            {
                return 0;
            }
        }
    }
}
