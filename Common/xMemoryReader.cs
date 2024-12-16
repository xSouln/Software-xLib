using System;

namespace xLibV100.Common
{
    public class xMemoryReader
    {
        public object Data { get; protected set; }

        public int DataLength => data != null ? data.Length : 0;

        public int RemainLength => DataLength - offset;


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

        public unsafe void GetValue<TValue>(out TValue result) where TValue : unmanaged
        {
            result = GetValue<TValue>();
        }

        public void Offset(int offset, bool generateException = true)
        {
            if (offset > 0 && (this.offset + offset > data.Length))
            {
                if (generateException)
                {
                    throw new IndexOutOfRangeException();
                }

                return;
            }

            if (offset < 0 && (this.offset - offset < 0))
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
