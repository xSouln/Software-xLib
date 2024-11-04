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

        public void Offset(int offset, bool generateException = true)
        {
            if (this.offset - offset < 0
                || (data != null && (this.offset + offset > data.Length)))
            {
                if (generateException)
                {
                    throw new IndexOutOfRangeException();
                }

                return;
            }

            this.offset += offset;
        }

        public unsafe string GetString()
        {
            try
            {
                string result = xMemory.GetString(data, offset, generateException: true);
                if (result == null)
                {
                    return null;
                }

                offset += result.Length + 1;

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
