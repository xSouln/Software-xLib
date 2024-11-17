using AutoMapper;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace xLibV100.Common
{
    public class xMemory
    {
        public static unsafe int Add<TData>(List<byte> packet, TData data, int offset = -1)
            where TData : unmanaged
        {
            if (packet == null && offset > packet.Count)
            {
                return 0;
            }

            byte* ptr = (byte*)&data;

            if (offset == -1)
            {
                for (int i = 0; i < sizeof(TData); i++)
                {
                    packet.Add(ptr[i]);
                }
            }
            else if (offset >= 0)
            {
                for (int i = 0; i < sizeof(TData); i++)
                {
                    packet.Insert(i + offset, ptr[i]);
                }
            }
            else
            {
                return 0;
            }

            return sizeof(TData);
        }


        public static unsafe int Add(List<byte> packet, void* data, int size)
        {
            if (packet != null && data != null)
            {
                byte* ptr = (byte*)&data;

                for (int i = 0; i < size; i++)
                {
                    packet.Add(ptr[i]);
                }

                return size;
            }
            return 0;
        }

        public static unsafe int Add(List<byte> packet, string data, int maxLength = int.MaxValue)
        {
            if (packet == null || data == null || maxLength < 1)
            {
                return 0;
            }

            int i = 0;
            foreach (byte ch in data)
            {
                packet.Add(ch);
                i++;

                if (i >= maxLength)
                {
                    break;
                }
            }

            return i;
        }

        public static unsafe int GetSize<T>(T element) where T : unmanaged
        {
            return sizeof(T);
        }

        public static unsafe int Add<TData>(List<byte> packet, TData[] data) where TData : unmanaged
        {
            if (packet != null && data != null && data.Length > 0)
            {
                foreach (TData element in data)
                {
                    byte* ptr = (byte*)&element;

                    for (int i = 0; i < sizeof(TData); i++)
                    {
                        packet.Add(ptr[i]);
                    }
                }

                return sizeof(TData) * data.Length;
            }

            return 0;
        }

        public static unsafe int Add<T>(List<byte> packet, ICollection<T> data) where T : unmanaged
        {
            if (packet != null && data != null && data.Count > 0)
            {
                foreach (T element in data)
                {
                    byte* ptr = (byte*)&element;

                    for (int i = 0; i < sizeof(T); i++)
                    {
                        packet.Add(ptr[i]);
                    }
                }

                return sizeof(T) * data.Count;
            }

            return 0;
        }

        public static unsafe byte[] ToByteArray<TRequest>(TRequest request) where TRequest : unmanaged
        {
            byte[] result = new byte[sizeof(TRequest)];
            byte* ptr = (byte*)&request;

            for (int i = 0; i < sizeof(TRequest); i++)
            {
                result[i] = ptr[i];
            }

            return result;
        }

        public static unsafe int Copy(void* destiny, void* source, int size)
        {
            if (destiny == null || source == null)
            {
                return 0;
            }

            byte* destinyPtr = (byte*)destiny;
            byte* sourcePtr = (byte*)source;

            for (int i = 0; i < size; i++)
            {
                destinyPtr[i] = sourcePtr[i];
            }

            return size;
        }

        public static unsafe int Copy(void* destiny, byte[] source, int offset = 0, int limit = int.MaxValue)
        {
            if (destiny == null || source == null || offset > limit)
            {
                return 0;
            }

            limit = source.Length < limit ? source.Length : limit;
            limit -= offset;

            fixed (byte* sourceFix = source)
            {
                byte* sourcePtr = sourceFix + offset;
                byte* destinyPtr = (byte*)destiny;

                for (int i = 0; i < limit; i++)
                {
                    destinyPtr[i] = sourcePtr[i];
                }
            }

            return limit;
        }

        public static unsafe int Copy<TElement>(TElement[] desteny, byte[] source) where TElement : unmanaged
        {
            if (desteny == null || source == null)
            {
                return 0;
            }

            int countOfElements = source.Length / sizeof(TElement);

            if (countOfElements == 0)
            {
                return 0;
            }

            int i = 0;

            fixed (byte* dataPtr = source)
            {
                TElement* sources = (TElement*)dataPtr;

                while (i < desteny.Length && i < countOfElements)
                {
                    desteny[i] = sources[i];
                    i++;
                }
            }

            return i;
        }

        public unsafe static int Convert<TDestiny>(out TDestiny destiny, void* source, int limit = int.MaxValue)
            where TDestiny : unmanaged
        {
            if (limit < sizeof(TDestiny))
            {
                destiny = default;
                return 0;
            }

            destiny = *(TDestiny*)source;

            return sizeof(TDestiny);
        }

        public unsafe static int Convert<TElement>(out TElement[] destiny, void* source, int sourceLength, int offset = 0)
            where TElement : unmanaged
        {
            if (source == null)
            {
                destiny = default;
                return 0;
            }

            int countOfElements = sourceLength / sizeof(TElement);

            if (countOfElements <= 0)
            {
                destiny = default;
                return 0;
            }

            TElement[] elements = new TElement[countOfElements];
            TElement* sources = (TElement*)((byte*)source + offset);

            for (int i = 0; i < countOfElements; i++)
            {
                elements[i] = sources[i];
            }

            destiny = elements;

            return countOfElements;
        }

        public unsafe static int Copy<TDestiny>(out TDestiny destiny, byte[] source, int offset = 0, int limit = int.MaxValue)
            where TDestiny : unmanaged
        {
            return Convert(out destiny, source, offset, limit);
        }

        public unsafe static int Convert<TDestiny>(out TDestiny destiny, byte[] source, int offset = 0, int limit = int.MaxValue)
            where TDestiny : unmanaged
        {
            if (source == null)
            {
                destiny = default;
                return 0;
            }

            limit = source.Length < limit ? source.Length : limit;
            limit -= offset;

            if (limit < sizeof(TDestiny))
            {
                destiny = default;
                return 0;
            }

            fixed (byte* ptr = source)
            {
                destiny = *(TDestiny*)(ptr + offset);
            }

            return sizeof(TDestiny);
        }

        public static unsafe int Convert<TElement>(out TElement[] desteny, byte[] source, int offset = 0, int limit = int.MaxValue) where TElement : unmanaged
        {
            if (source == null)
            {
                desteny = default;
                return 0;
            }

            limit = source.Length < limit ? source.Length : limit;
            limit -= offset;

            int countOfElements = limit / sizeof(TElement);

            if (countOfElements <= 0)
            {
                desteny = default;
                return 0;
            }

            TElement[] elements = new TElement[countOfElements];

            fixed (byte* dataPtr = source)
            {
                TElement* sources = (TElement*)dataPtr;

                for (int i = 0; i < countOfElements; i++)
                {
                    elements[i] = sources[i];
                }
            }

            desteny = elements;

            return elements.Length;
        }

        public static unsafe object Convert(Type type, byte[] source, int offset = 0, int limit = int.MaxValue)
        {
            if (type == null || source == null)
            {
                throw new ArgumentException("conversion error");
            }

            limit = source.Length < limit ? source.Length : limit;
            limit -= offset;

            if (type.IsValueType)
            {
                int typeSize = Marshal.SizeOf(type);

                if (limit < typeSize)
                {
                    throw new ArgumentException("conversion error");
                }

                fixed (byte* contentPtr = source)
                {
                    IntPtr intPtr = new IntPtr(contentPtr + offset);
                    object result = Marshal.PtrToStructure(intPtr, type);
                    Marshal.FreeHGlobal(intPtr);

                    return result;
                }
            }
            else if (type.IsArray)
            {
                var elementType = type.GetElementType();

                if (!elementType.IsValueType)
                {
                    throw new ArgumentException("conversion error");
                }

                var typeSize = Marshal.SizeOf(elementType);
                int countOfElements = limit / typeSize;

                if (countOfElements <= 0)
                {
                    throw new ArgumentException("conversion error");
                }

                object result = Activator.CreateInstance(type, countOfElements) ?? throw new ArgumentException("conversion error");
                GCHandle handle = GCHandle.Alloc(result, GCHandleType.Pinned);

                try
                {
                    byte* destinyPtr = (byte*)handle.AddrOfPinnedObject().ToPointer();
                    fixed (byte* sourceFix = source)
                    {
                        byte* sourcePtr = sourceFix + offset;

                        /*int i = 0;
                        int size = countOfElements * typeSize;
                        int step = 0;

                        while (i < size)
                        {
                            int remainingLength = size - i;

                            if (remainingLength > 8)
                            {
                                *(long*)destinyPtr = *(long*)sourcePtr;
                                step = sizeof(long);

                                goto end;
                            }
                            else if (remainingLength > 4)
                            {
                                *(int*)destinyPtr = *(int*)sourcePtr;
                                step = sizeof(int);

                                goto end;
                            }
                            else if (remainingLength > 2)
                            {
                                *(ushort*)destinyPtr = *(ushort*)sourcePtr;
                                step = sizeof(ushort);

                                goto end;
                            }

                            *destinyPtr = *sourcePtr;
                            step = sizeof(byte);

                        end:;
                            i += step;
                            destinyPtr += step;
                            sourcePtr += step;
                        }*/

                        for (int i = 0; i < countOfElements * typeSize; i++)
                        {
                            destinyPtr[i] = sourcePtr[i];
                        }
                    }
                }
                catch
                {
                    handle.Free();
                }

                return result;
            }

            throw new ArgumentException("not suported type");
        }

        public static byte[] ConvertToArray<TElement>(IEnumerable<TElement> source, int startIndex = 0, int length = int.MaxValue)
            where TElement : unmanaged
        {
            List<byte> result = new List<byte>();

            if (source == null)
            {
                result.ToArray();
            }

            int i = 0;
            foreach(var element in source)
            {
                if (i >= length)
                {
                    break;
                }

                if (i < startIndex)
                {
                    continue;
                }

                Add(result, element);
                i++;
            }

            return result.ToArray();
        }

        public static byte[] ToByteArray(string source, int startIndex = 0, int length = int.MaxValue)
        {
            List<byte> result = new List<byte>();

            if (source == null)
            {
                result.ToArray();
            }

            int i = 0;
            foreach (var element in source)
            {
                if (i >= length)
                {
                    break;
                }

                if (i < startIndex)
                {
                    continue;
                }

                Add(result, element);
                i++;
            }

            return result.ToArray();
        }

        public static unsafe byte[] ToByteArray(object source)
        {
            if (source == null)
            {
                throw new ArgumentException("null reference");
            }

            Type type = source.GetType();

            if (type.IsValueType)
            {
                int typeSize = Marshal.SizeOf(type);
                byte[] desteny = new byte[typeSize];

                fixed (byte* sourcePtr = desteny)
                {
                    IntPtr intPtr = new IntPtr(sourcePtr);
                    Marshal.StructureToPtr(source, intPtr, true);
                    Marshal.FreeHGlobal(intPtr);
                }

                return desteny;
            }
            else if (type.IsArray)
            {
                var elementType = type.GetElementType();

                if (!elementType.IsValueType)
                {
                    throw new ArgumentException("source type is not supported");
                }

                Array elements = source as Array;
                if (elements == null || elements.Length == 0)
                {
                    throw new ArgumentException("source type is not supported");
                }

                int typeSize = Marshal.SizeOf(elementType);
                GCHandle handle = GCHandle.Alloc(elements, GCHandleType.Pinned);
                byte[] desteny = null;

                try
                {
                    // Получаем указатель на первый элемент массива
                    void* sourcePtr = handle.AddrOfPinnedObject().ToPointer();

                    Convert(out desteny, sourcePtr, typeSize * elements.Length);
                }
                finally
                {
                    handle.Free();
                }

                return desteny;
            }

            throw new ArgumentException("source type is not supported");
        }

        public static unsafe int Convert<TElement>(List<TElement> desteny, byte[] data, int offset = 0, int limit = int.MaxValue)
            where TElement : unmanaged
        {
            Convert(out TElement[] result, data, offset, limit);

            if (result == null)
            {
                return 0;
            }

            desteny.AddRange(result);

            return result.Length;
        }

        public static unsafe int Copy(byte[] desteny, int destenyOffset, byte[] source, int sourceOffset, int size)
        {
            if (desteny == null || desteny.Length == 0 || source == null || source.Length == 0)
            {
                return 0;
            }

            if ((destenyOffset + size) > desteny.Length || (sourceOffset + size) > source.Length)
            {
                return 0;
            }

            for (int i = 0; i < size; i++)
            {
                desteny[i + destenyOffset] = source[i + sourceOffset];
            }

            return 0;
        }

        public static int Copy(object source, object desteny)
        {
            if (source == null || desteny == null)
            {
                throw new ArgumentNullException("source || desteny");
            }

            try
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap(source.GetType(), desteny.GetType());
                });

                IMapper mapper = config.CreateMapper();
                mapper.Map(source, desteny);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 0;
        }

        public static int Copy<TElement>(IEnumerable<TElement> source, ICollection<TElement> desteny)
        {
            if (source == null || desteny == null)
            {
                throw new ArgumentNullException("source || desteny");
            }

            try
            {
                foreach (var element in source)
                {
                    desteny.Add(element);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 0;
        }

        public static unsafe TValue GetValue<TValue>(byte[] source, int offset = 0, int length = int.MaxValue, bool generateException = false)
            where TValue : unmanaged
        {
            if (source == null)
            {
                throw new ArgumentException();
            }

            if (length - source.Length + offset < sizeof(TValue))
            {
                throw new ArgumentException();
            }

            fixed (byte* sourcePtr = source)
            {
                return *(TValue*)(sourcePtr + offset);
            }
        }

        public static string GetString(byte[] source, int offset = 0, int length = int.MaxValue, bool generateException = false)
        {
            if (source == null)
            {
                if (generateException)
                {
                    throw new NullReferenceException();
                }

                return null;
            }

            if (offset >= source.Length)
            {
                if (generateException)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return null;
            }

            string value = "";
            int i = offset;
            while (i < source.Length && i < length)
            {
                if (source[i] == 0)
                {
                    break;
                }

                value += (char)source[i];
                i++;
            }

            return value;
        }

        public static unsafe bool Compare<T>(ref T source, ref T element) where T : unmanaged
        {
            byte* source_mem = (byte*)Unsafe.AsPointer(ref source);
            byte* element_mem = (byte*)Unsafe.AsPointer(ref element);

            for (int i = 0; i < sizeof(T); i++)
            {
                if (source_mem[i] != element_mem[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static unsafe bool Compare(string source, string element)
        {
            if (source == null || element == null || (source.Length != element.Length))
            {
                return false;
            }

            for (int i = 0; i < element.Length; i++)
            {
                if (source[i] != element[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static unsafe bool Compare(void* in_1, int offset_1, void *in_2, int offset_2, int count)
        {
            if (in_1 != null && in_2 != null && count > 0)
            {
                byte* ptr_1 = (byte*)in_1;
                byte* ptr_2 = (byte*)in_2;
                for (int i = 0; i < count; i++)
                {
                    if (ptr_1[offset_1 + i] != ptr_2[offset_2 + i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static unsafe int Compare<T1, T2>(T1 source, T2 element)
            where T1 : unmanaged
            where T2 : unmanaged
        {
            if (sizeof(T1) != sizeof(T2))
            {
                return -2;
            }

            int length = sizeof(T1);
            byte* ptr1 = (byte*)&source;
            byte* ptr2 = (byte*)&element;

            for (int i = 0; i < length; i++)
            {
                if (ptr1[i] != ptr2[i])
                {
                    return -1;
                }
            }

            return 0;
        }

        public static unsafe bool Compare(void* in_1, void* in_2, int count)
        {
            if (in_1 != null && in_2 != null && count > 0)
            {
                byte* ptr_1 = (byte*)in_1;
                byte* ptr_2 = (byte*)in_2;
                for (int i = 0; i < count; i++)
                {
                    if (ptr_1[i] != ptr_2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static unsafe string ToString(void* obj, int obj_size)
        {
            string str = "";
            char* ptr;

            if (obj == null || obj_size <= 0)
            {
                return str;
            }

            ptr = (char*)obj;
            for (uint i = 0; i < obj_size; i++)
            {
                str += ptr[i];
            }

            return str;
        }

        public static int GetSizeOfType(Type type)
        {
            int typeSize = -1;

            if (type == typeof(byte) || type == typeof(sbyte))
            {
                typeSize = 1;
            }
            else if (type == typeof(ushort) || type == typeof(short))
            {
                typeSize = sizeof(ushort);
            }
            else if (type == typeof(uint) || type == typeof(int) || type == typeof(float))
            {
                typeSize = sizeof(uint);
            }
            else if (type == typeof(ulong) || type == typeof(long) || type == typeof(double))
            {
                typeSize = sizeof(ulong);
            }

            return typeSize;
        }

        public static IValueConverter GetValueConverterForBaseType(Type type)
        {
            if (type == typeof(byte))
            {
                return new ValueConverterToBaseTypes<byte>();
            }
            else if (type == typeof(sbyte))
            {
                return new ValueConverterToBaseTypes<sbyte>();
            }
            else if (type == typeof(ushort))
            {
                return new ValueConverterToBaseTypes<ushort>();
            }
            else if (type == typeof(short))
            {
                return new ValueConverterToBaseTypes<short>();
            }
            else if (type == typeof(uint))
            {
                return new ValueConverterToBaseTypes<uint>();
            }
            else if (type == typeof(int))
            {
                return new ValueConverterToBaseTypes<int>();
            }
            else if (type == typeof(ulong))
            {
                return new ValueConverterToBaseTypes<ulong>();
            }
            else if (type == typeof(long))
            {
                return new ValueConverterToBaseTypes<long>();
            }
            else if (type == typeof(float))
            {
                return new ValueConverterToBaseTypes<float>();
            }
            else if (type == typeof(double))
            {
                return new ValueConverterToBaseTypes<double>();
            }

            return null;
        }

        public static IValueConverter GetValueConverter(Type type)
        {
            if (type == typeof(string))
            {
                return new ValueConverterToString();
            }
            else if (type.IsValueType && type.IsPrimitive)
            {
                return GetValueConverterForBaseType(type);
            }
            else if (type.IsValueType)
            {
                return new ValueConverterToStructure(type);
            }
            else if (type.IsArray && type.GetElementType().IsValueType && type.IsPrimitive)
            {
                var converter = GetValueConverterForBaseType(type);
                converter.IsCollection = true;

                return converter;
            }

            return null;
        }

        public static unsafe object GetValue(Type type, byte[] source, int offset)
        {
            if (source == null || offset > source.Length)
            {
                return default;
            }

            int typeSize = -1;

            if (type.IsEnum)
            {
                typeSize = GetSizeOfType(Enum.GetUnderlyingType(type));
            }
            else if (type.IsAssignableFrom(type))
            {

            }
            else if(type.IsValueType)
            {
                typeSize = GetSizeOfType(type);
            }

            if (typeSize + offset > source.Length)
            {
                return default;
            }

            return default;
        }
    }
}
