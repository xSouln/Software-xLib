using System;
using xLibV100.Common;

namespace xLibV100.Transceiver
{
    public struct xContent
    {
        public unsafe byte* Data;
        public int DataSize;

        public unsafe int Get<DataT>(out DataT result, bool typeSizeException = false) where DataT : unmanaged
        {
            result = default;

            if (sizeof(DataT) > DataSize)
            {
                if (typeSizeException)
                {
                    throw new Exception("sizeof(DataT) > DataSize");
                }

                return -1;
            }

            result = *(DataT*)Data;

            Data += sizeof(DataT);
            DataSize -= sizeof(DataT);

            return 0;
        }

        public unsafe byte[] GetSegment(int dataSize, bool typeSizeException = false)
        {
            if (DataSize < dataSize)
            {
                if (typeSizeException)
                {
                    throw new Exception("sizeof(DataT) > DataSize");
                }
                return null;
            }

            xMemory.Convert(out byte[] data, Data, dataSize);

            Data += dataSize;
            DataSize -= dataSize;

            return data;
        }

        public unsafe int Get(out byte[] data, int dataSize, bool typeSizeException = false)
        {
            if (dataSize > DataSize)
            {
                data = null;
                if (typeSizeException)
                {
                    throw new Exception("sizeof(DataT) > DataSize");
                }
                return -1;
            }

            xMemory.Convert(out data, Data, dataSize);

            Data += dataSize;
            DataSize -= dataSize;

            return 0;
        }

        public unsafe void Offset(int offset)
        {
            if (offset > DataSize)
            {
                offset = DataSize;
            }

            Data += offset;
        }
    }
}
