using System;
using System.Runtime.InteropServices;

namespace xLibV100.Common
{
    public interface IValueConverter
    {
        int TypeSize { get; }
        bool IsCollection { get; set; }

        object GetValue(byte[] source, int offset = 0, int limit = int.MaxValue);
        object GetValues(byte[] source, int offset = 0, int limit = int.MaxValue);

        unsafe object GetValue(byte* source, int sourceLength, int offset = 0);
    }

    public class ValueConverterToString : IValueConverter
    {
        public unsafe int TypeSize => -1;
        public bool IsCollection { get; set; }

        public object GetValue(byte[] source, int offset = 0, int limit = int.MaxValue)
        {
            if (source == null || offset >= source.Length || offset < 0 || offset > limit)
            {
                throw new ArgumentException("conversion error");
            }

            limit = source.Length < limit ? source.Length : limit;

            string result = "";

            for (int i = offset; i < limit; i++)
            {
                if (source[i] == 0)
                {
                    break;
                }

                result += Convert.ToChar(source[i]);
            }

            return result;
        }

        public unsafe object GetValue(byte* source, int sourceLength, int offset = 0)
        {
            return null;
        }

        public object GetValues(byte[] source, int offset = 0, int limit = int.MaxValue)
        {
            return null;
        }
    }

    public class ValueConverterToBaseTypes<TValue> : IValueConverter where TValue : unmanaged
    {
        public unsafe int TypeSize => sizeof(TValue);
        public bool IsCollection { get; set; }

        public object GetValue(byte[] source, int offset = 0, int limit = int.MaxValue)
        {
            if (IsCollection)
            {
                return GetValues(source, offset, limit);
            }

            var result = xMemory.Convert(out TValue value, source, offset, limit);

            if (result == 0)
            {
                throw new ArgumentException("conversion error");
            }

            return value;
        }

        public unsafe object GetValue(byte* source, int sourceLength, int offset = 0)
        {
            if (sourceLength + offset > sizeof(TValue))
            {
                throw new ArgumentException("conversion error");
            }

            return *(TValue*)(source + offset);
        }

        public object GetValues(byte[] source, int offset = 0, int limit = int.MaxValue)
        {
            var result = xMemory.Convert(out TValue[] value, source, offset, limit);

            if (result == 0)
            {
                throw new ArgumentException("conversion error");
            }

            return value;
        }
    }

    public class ValueConverterToStructure : IValueConverter
    {
        private readonly Type type;
        private readonly int typeSize;

        public int TypeSize => typeSize;
        public bool IsCollection { get; set; }

        public ValueConverterToStructure(Type type)
        {
            this.type = type;
            typeSize = Marshal.SizeOf(type);
        }

        public unsafe object GetValue(byte[] source, int offset = 0, int limit = int.MaxValue)
        {
            if (source == null)
            {
                throw new ArgumentException("conversion error");
            }

            limit = source.Length < limit ? source.Length : limit;

            if (limit - offset < typeSize)
            {
                throw new ArgumentException("conversion error");
            }

            xMemory.Convert(out byte[] content, source, offset, typeSize);

            fixed (void* contentPtr = content)
            {
                IntPtr ptr = new IntPtr(contentPtr);
                return Marshal.PtrToStructure(ptr, type);
            }
        }

        public unsafe object GetValue(byte* source, int sourceLength, int offset = 0)
        {
            xMemory.Convert(out byte[] content, source, offset, typeSize);

            if (content == null)
            {
                throw new ArgumentException("conversion error");
            }

            fixed (void* contentPtr = content)
            {
                IntPtr ptr = new IntPtr(contentPtr);
                return Marshal.PtrToStructure(ptr, type);
            }
        }

        public object GetValues(byte[] source, int offset = 0, int limit = -1)
        {
            return null;
        }
    }
}
