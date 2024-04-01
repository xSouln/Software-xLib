using System.Collections.Generic;

namespace xLibV100.Common
{
    public interface IDataBuffer
    {
        int Add(string data);
        int Add(byte[] data);
        int Add<T>(T data) where T : unmanaged;
        unsafe int Add(void* data, int size);
        byte[] Data { get; }
        int DataSize { get; }
        void Clear();
    }

    public class DataBuffer : IDataBuffer
    {
        protected List<byte> data = new List<byte>();

        public int Add(string data)
        {
            return xMemory.Add(this.data, data);
        }

        public int Add(byte[] data)
        {
            return xMemory.Add(this.data, data);
        }

        public int Add<T>(T data) where T : unmanaged
        {
            return xMemory.Add(this.data, data);
        }

        public unsafe int Add(void* data, int size)
        {
            return xMemory.Add(this.data, data, size);
        }

        public virtual void Clear()
        {
            data.Clear();
        }

        public byte[] Data => data.ToArray();

        public int DataSize => data.Count;
    }
}
