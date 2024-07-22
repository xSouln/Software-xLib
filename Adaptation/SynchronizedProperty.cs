using System;

namespace xLibV100.Adaptation
{
    public enum SynchronizedPropertyTypes : byte
    {
        Byte,

        HalfWord,
        Word,
        DoubleWord,

        Float,
        Double,

        String,
        Object
    };

    public enum SynchronizedPropertyFlagBitNumbers : byte
    {
        IsWritable,
        IsSequential,
        IsConstant,
    };


    [Flags]
    public enum SynchronizedPropertyFlags : byte
    {
        NotSet,

        IsWritable = 1 << SynchronizedPropertyFlagBitNumbers.IsWritable,
        IsSequential = 1 << SynchronizedPropertyFlagBitNumbers.IsSequential,
        IsConstant = 1 << SynchronizedPropertyFlagBitNumbers.IsConstant,
    };

    public struct SynchronizedPropertyInfoT
    {
        private uint value;

        public ushort Id
        {
            get => (ushort)((value >> 8) & 0x3ff);
        }

        public SynchronizedPropertyTypes Type
        {
            get => (SynchronizedPropertyTypes)(value & 0x0f);
        }

        public SynchronizedPropertyFlags Flags
        {
            get => (SynchronizedPropertyFlags)((value >> 4) & 0x0f);
        }

        public ushort Size
        {
            get => (ushort)((value >> 18) & 0x3fff);
        }

        //public ushort Id;
        //public SynchronizedPropertyTypes PropertyType;
        //public SynchronizedPropertyFlags PropertyFlags;
        //public ushort Size;
        //public byte Description;
    }

    public struct SynchronizedPropertySettingInfoT
    {
        private int value;

        public ushort Id
        {
            get => (ushort)((value >> 8) & 0x3ff);
            set => this.value |= (value & 0x3ff) << 8;
        }

        public SynchronizedPropertyTypes Type
        {
            get => (SynchronizedPropertyTypes)(value & 0x0f);
            set => this.value |= (int)value & 0x0f;
        }

        public byte Flags
        {
            get => (byte)((value >> 4) & 0x0f);
            set => this.value |= ((int)value & 0x0f) << 4;
        }

        public ushort Size
        {
            get => (ushort)((value >> 18) & 0x3fff);
            set => this.value |= ((int)value & 0x3fff) << 18;
        }

        public int Value
        {
            get => value;
        }
    }

    public class SynchronizedPropertyForSetting
    {
        public SynchronizedPropertySettingInfoT Info;
        public byte[] Content;

        public SynchronizedPropertyForSetting(SynchronizedPropertySettingInfoT info, byte[] content)
        {
            Info = info;
            Content = content;
        }
    }

    public class SynchronizedProperty
    {
        protected SynchronizedPropertyInfoT info;

        public ushort Id
        {
            get => info.Id;
        }

        public SynchronizedPropertyTypes Type
        {
            get => info.Type;
        }

        public SynchronizedPropertyFlags Flags
        {
            get => info.Flags;
        }

        public ushort Size
        {
            get => info.Size;
        }

        public byte[] Content;

        public SynchronizedProperty(SynchronizedPropertyInfoT info, byte[] content = null)
        {
            this.info = info;
            Content = content;
        }
    }
}
