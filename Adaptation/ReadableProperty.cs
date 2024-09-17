using System;
using System.Collections.Generic;
using xLibV100.Common;

namespace xLibV100.Adaptation
{
    public enum SPropertyTypes : byte
    {
        Base,

        Floating,
        String,
        Object,

        FixedPoint,
    };


    [Flags]
    public enum RequestedPropertyMode : ushort
    {

    }

    public struct RequestedRedablePropertyT
    {
        private ushort value;

        public ushort Id
        {
            get => (ushort)BitsFieldHelper.GetValue(value, mask: 0x3ff, offset: 6);
            set => this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 0x3ff, offset: 6);
        }

        public bool ExtensionIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 0);
            set
            {
                if (value)
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 0);
                }
                else
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 0);
                }
            }
        }

        public bool ModeIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 1);
            set
            {
                if (value)
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 1);
                }
                else
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 1);
                }
            }
        }

        public bool LimitsIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 2);
            set
            {
                if (value)
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 2);
                }
                else
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 2);
                }
            }
        }

        public bool ResponseTypeInfoIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 3);
            set
            {
                if (value)
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 3);
                }
                else
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 3);
                }
            }
        }

        public bool ResponseSizeInfoIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 4);
            set
            {
                if (value)
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 4);
                }
                else
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 4);
                }
            }
        }

        public bool ResponseValueIsExcluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 5);
            set
            {
                if (value)
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 5);
                }
                else
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 5);
                }
            }
        }

        public ushort Value => value;
    }

    public struct RedablePropertyInfoT
    {
#pragma warning disable CS0649 // данная переменная служит для согласования раммера(целостности) данных при преобразовании с массива
        private ushort value;
#pragma warning restore CS0649

        public SPropertyTypes Type
        {
            get => (SPropertyTypes)BitsFieldHelper.GetValue(value, mask: 0xf, offset: 0);
        }

        public byte BaseTypeSize
        {
            get => (byte)BitsFieldHelper.GetValue(value, mask: 0x7, offset: 4);
        }

        public bool IsReadOnly
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 7);
        }

        public bool IsSequential
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 8);
        }

        public bool IsConstant
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 9);
        }

        public bool IsPointer
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 10);
        }

        public bool IsSigned
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 11);
        }

        public bool IsDynamic
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 12);
        }

        public bool IsVirtual
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 13);
        }
    }

    public struct ReceivedRedablePropertyT
    {
        private ushort value;

        public ushort Id
        {
            get => (ushort)BitsFieldHelper.GetValue(value, mask: 0x3ff, offset: 6);
            set => this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 0x3ff, offset: 6);
        }

        public bool ErrorIsOccured
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 0);
        }

        public bool ExtensionIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 1);
        }

        public bool ModeIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 2);
        }

        public bool TypeInfoIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 3);
        }

        public bool TypeSizeIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 4);
        }

        public bool ValueIsExcluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 5);
        }
    }

    public class ReceivedRedableProperty
    {
        public RedablePropertyInfoT Info;
        public ReceivedRedablePropertyT ReceivedInfo;

        public List<byte[]> Elements = new List<byte[]>();

        public ushort Extension;
        public ushort CountOfElements;

        public byte[] Element => Elements != null && Elements.Count > 0 ? Elements[0] : null;

        public ReceivedRedableProperty(ReceivedRedablePropertyT receivedInfo,
            RedablePropertyInfoT info,
            ushort extension = 0,
            ushort countOfElements = 1,
            List<byte[]> elements = null,
            byte[] content = null)
        {
            Info = info;
            ReceivedInfo = receivedInfo;

            if (content != null)
            {
                Elements.Add(content);
            }

            if (elements != null)
            {
                foreach (var element in elements)
                {
                    if (element != null)
                    {
                        Elements.Add(element);
                    }
                }
            }

            Extension = extension;
            CountOfElements = countOfElements;
        }
    
        public int CopyElementsTo<TElement>(IList<TElement> desteny,
            int startIndex = 0,
            int stopIndex = int.MaxValue,
            bool generateConvertException = false)
            where TElement : unmanaged
        {
            if (desteny == null || Elements == null)
            {
                return 0;
            }

            int i = startIndex;
            int j = 0;
            while (i < desteny.Count && i < stopIndex && i < Elements.Count)
            {
                try
                {
                    desteny[i] = xMemory.GetValue<TElement>(Elements[j], generateException: true);
                }
                catch (Exception ex)
                {
                    if (generateConvertException)
                    {
                        throw ex;
                    }
                }

                i++;
                j++;
            }

            return j;
        }
    }

    public class ReadableProperty
    {
        public ReceivedRedablePropertyT Info;

        public byte[] Content;

        public ReadableProperty(ReceivedRedablePropertyT info, byte[] content = null)
        {
            this.Info = info;
            Content = content;
        }
    }
}
