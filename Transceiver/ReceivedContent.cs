using xLibV100.Common;

namespace xLibV100.Transceiver
{
    public struct xContent
    {
        public unsafe byte* Data;
        public int DataSize;

        public unsafe int Get<DataT>(out DataT result) where DataT : unmanaged
        {
            result = default;

            if (sizeof(DataT) > DataSize)
            {
                return -1;
            }

            result = *(DataT*)Data;

            Data += sizeof(DataT);
            DataSize -= sizeof(DataT);

            return 0;
        }

        public unsafe int Get(out byte[] data, int dataSize)
        {
            if (dataSize > DataSize)
            {
                data = null;
                return -1;
            }

            data = new byte[dataSize];
            xMemory.Copy(data, Data, dataSize);

            Data += dataSize;
            DataSize -= dataSize;

            return 0;
        }
    }
}
