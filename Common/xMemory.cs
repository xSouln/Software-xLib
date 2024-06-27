using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace xLibV100.Common
{
    public class xMemory
    {
        public static unsafe int Add<TData>(List<byte> packet, TData data) where TData : unmanaged
        {
            if (packet != null)
            {
                byte* ptr = (byte*)&data;

                for (int i = 0; i < sizeof(TData); i++)
                {
                    packet.Add(ptr[i]);
                }

                return sizeof(TData);
            }
            return 0;
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

        public static unsafe int Add(List<byte> packet, string data)
        {
            if (packet != null && data != null)
            {
                foreach (byte ch in data)
                {
                    packet.Add(ch);
                }

                return data.Length;
            }
            return 0;
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
            byte* request_ptr = (byte*)&request;

            for (int i = 0; i < sizeof(TRequest); i++)
            {
                result[i] = request_ptr[i];
            }

            return result;
        }

        public static unsafe int Copy(void* Out, void* In, int InSize, int Offset)
        {
            if (Out != null && In != null && InSize > 0)
            {
                byte* OutPtr = (byte*)Out;
                byte* InPtr = (byte*)In;

                for (int i = 0; i < InSize; i++)
                {
                    OutPtr[i + Offset] = InPtr[i];
                }
                return InSize;
            }
            return 0;
        }

        public unsafe static int Copy<TDestiny>(out TDestiny destiny, byte[] source) where TDestiny : unmanaged
        {
            TDestiny temp = default;

            if (source == null || sizeof(TDestiny) < source.Length)
            {
                goto end;
            }

            byte* ptr = (byte*)&temp;
           
            for (int i = 0; i < sizeof(TDestiny); i++)
            {
                ptr[i] = source[i];
            }

        end:;
            destiny = temp;

            return sizeof(TDestiny);
        }

        public static unsafe int Copy(byte[] destination, void* source, int size)
        {
            if (destination != null && source != null && size >= destination.Length)
            {
                for (int i = 0; i < size; i++)
                {
                    destination[i] = ((byte*)source)[i];
                }

                return size;
            }
            return 0;
        }


        public static unsafe int Copy<T>(T[] desteny, byte[] data) where T : unmanaged
        {
            if (desteny != null && desteny.Length > 0 && data != null && data.Length / sizeof(T) > 0)
            {
                int dataSize = data.Length / sizeof(T);

                int i = 0;
                fixed (byte* mem = data)
                {
                    byte* ptr = mem;

                    while (i < desteny.Length && i < dataSize)
                    {
                        desteny[i] = *(T*)ptr;

                        ptr += sizeof(T);
                        i++;
                    }
                }
                return i;
            }
            return 0;
        }

        public static unsafe int Copy<T>(List<T> desteny, byte[] data) where T : unmanaged
        {
            if (desteny != null && data != null && data.Length / sizeof(T) > 0)
            {
                int dataSize = data.Length / sizeof(T);

                int i = 0;
                fixed (byte* mem = data)
                {
                    byte* ptr = mem;

                    while (i < dataSize)
                    {
                        desteny.Add(*(T*)ptr);

                        ptr += sizeof(T);
                        i++;
                    }
                }
                return i;
            }
            return 0;
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

        public static unsafe int Copy(void* Out, void* In, int Size)
        {
            return Copy(Out, In, Size, 0);
        }

        public static unsafe int Copy(byte[] Out, void* In, int Size, int Offset)
        {
            if (Out != null && In != null)
            {
                fixed (byte* ptr = Out)
                {
                    return Copy(ptr, In, Size, Offset);
                }
            }
            return 0;
        }

        public static unsafe int Copy(byte[] Out, string In, int Offset)
        {
            if (Out != null && In != null && In.Length > 0 && (Out.Length >= (In.Length + Offset)))
            {
                for (int i = 0; i < In.Length; i++)
                {
                    Out[i + Offset] = (byte)In[i];
                }
                return In.Length;
            }
            return 0;
        }

        public static unsafe int Copy(void* source, string data, int max_size)
        {
            int i = 0;
            byte* mem = (byte*)source;

            if (source == null || data == null)
            {
                return 0;
            }

            while (i < data.Length && i < max_size)
            {
                mem[i] = (byte)data[i];
                i++;
            }

            return i;
        }

        public static unsafe int Copy(void* source, string data)
        {
            int i = 0;
            byte* mem = (byte*)source;

            if (source == null || data == null)
            {
                return 0;
            }

            while (i < data.Length)
            {
                mem[i] = (byte)data[i];
                i++;
            }

            return i;
        }

        public static unsafe int Copy(void* Out, byte[] In, int InSize, int Offset)
        {
            if (Out != null && In != null && InSize > 0)
            {
                byte* OutPtr = (byte*)Out;
                for (int i = 0; i < InSize; i++)
                {
                    OutPtr[i + Offset] = In[i];
                }
                return InSize;
            }
            return 0;
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
    }
}
