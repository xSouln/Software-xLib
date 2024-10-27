using System;

namespace xLibV100.Common
{
    public class xMemoryReader
    {
        public object Data { get; protected set; }

        protected byte[] data;
        protected int offset;

        public xMemoryReader(byte[] data)
        {
            this.data = data;
        }

        public unsafe TValue GetValue<TValue>()
            where TValue : unmanaged
        {
            try
            {
                var result = xMemory.GetValue<TValue>(data, offset: offset, generateException: true);
                offset += sizeof(TValue);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
