using System;
using System.Collections.Generic;
using xLibV100.Common;
using xLibV100.Net;

namespace xLibV100.Adaptation
{
    public class NetAddressConverter : IPropertyConverter
    {
        public object Convert(xMemoryReader memoryReader)
        {
            try
            {
                return memoryReader.GetValue<NetAddressT>().ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ToBinary(object property)
        {
            try
            {
                if (property is NetAddressT)
                {
                    return xMemory.ToByteArray((NetAddressT)property);
                }
                else if (property is string)
                {
                    return xMemory.ToByteArray(NetAddressT.Create((string)property));
                }

                throw new FormatException();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class StringConverter : IPropertyConverter
    {
        public object Convert(xMemoryReader memoryReader)
        {
            try
            {
                return memoryReader.GetString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ToBinary(object property)
        {
            List<byte> content = new List<byte>();

            if (property is string)
            {
                xMemory.Add(content, (string)property);
                xMemory.Add(content, (byte)0);

                return content.ToArray();
            }
            else
            {
                throw new FormatException();
            }
        }
    }

    public class StringArrayConverter : IPropertyConverter
    {
        public object Convert(xMemoryReader memoryReader)
        {
            try
            {
                List<string> result = new List<string>();
                var countOfElements = memoryReader.GetValue<ushort>();

                while (countOfElements > 0)
                {
                    result.Add(memoryReader.GetString());
                    countOfElements--;
                }

                return result.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ToBinary(object property)
        {
            List<byte> content = new List<byte>();

            if (!(property is string[] list))
            {
                throw new FormatException();
            }

            foreach (var item in list)
            {
                xMemory.Add(content, (string)property);
                xMemory.Add(content, (byte)0);
            }

            return content.ToArray();
        }
    }

    public class BaseTypesConverter<T> : IPropertyConverter
        where T : unmanaged
    {
        public object Convert(xMemoryReader memoryReader)
        {
            try
            {
                return memoryReader.GetValue<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ToBinary(object property)
        {
            if (property is T)
            {
                return xMemory.ToByteArray((T)property);
            }
            else
            {
                throw new FormatException();
            }
        }
    }
}
