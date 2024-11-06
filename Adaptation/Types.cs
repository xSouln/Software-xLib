using System;
using xLibV100.Common;

namespace xLibV100.Adaptation
{
    public enum PropertyAdaptionMode : byte
    {
        Undefined,

        ByRange,
        ByIds
    }


    [Flags]
    public enum PropertyAdaptionFlags : byte
    {
        None = 0,
        IncludePropertiesInfo = 1 << 0,
        IncludePropertiesIds = 1 << 1,
        IncludePropertiesSize = 1 << 2,
    }

    public enum PropertySizeInfo : byte
    {
        Byte,
        HalfWord,
        Word,
        DoubleWord,

        Custom
    }

    public enum PropertyTypeInfo : byte
    {
        BaseType,

        EnumType,
        FlagsType,

        FloatingPoint,
        FixedPoint,

        String,

        Sequence,
        Custom
    }

    public struct PropertyInfoT
    {
        private byte value;

        public PropertySizeInfo SizeInfo
        {
            get => (PropertySizeInfo)BitsFieldHelper.GetValue(value, 0x07, 0);
            set
            {
                this.value = (byte)BitsFieldHelper.SetValue(this.value, value, 0x07, 0);
            }
        }

        public PropertyTypeInfo TypeInfo
        {
            get => (PropertyTypeInfo)BitsFieldHelper.GetValue(value, 0x0F, 3);
            set
            {
                this.value = (byte)BitsFieldHelper.SetValue(this.value, value, 0x0F, 3);
            }
        }

        public bool IsSigned
        {
            get => BitsFieldHelper.GetState(value, 0x01, 7);
            set
            {
                this.value = (byte)BitsFieldHelper.SetValue(this.value, value, 0x01, 7);
            }
        }
    }

    public struct PropertyProviderInfoT
    {
        private ushort value;

        public PropertyAdaptionMode AdaptionMode
        {
            get => (PropertyAdaptionMode)BitsFieldHelper.GetValue(value, 0xFF, 0);
        }

        public bool PropertiesInfoIsIncluded
        {
            get => BitsFieldHelper.GetState(value, 0x01, 8);
        }

        public bool PropertiesIdsIsIncluded
        {
            get => BitsFieldHelper.GetState(value, 0x01, 9);
        }
    }
}
