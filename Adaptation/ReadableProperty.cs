using System;

namespace xLibV100.Adaptation
{
    public enum RWPropertyTypes : byte
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

    public enum RWPropertyFlagBitNumbers : byte
    {
        IsWritable,
        IsSequential,
        IsConstant,
    };


    [Flags]
    public enum RWPropertyFlags : byte
    {
        NotSet,

        IsWritable = 1 << RWPropertyFlagBitNumbers.IsWritable,
        IsSequential = 1 << RWPropertyFlagBitNumbers.IsSequential,
        IsConstant = 1 << RWPropertyFlagBitNumbers.IsConstant,
    };

    public enum ReadablePropertyInfoOffsets
    {
        Type = 0,
        Flags = 4,
        Id = 8,
        Size = 18
    }

    public enum ReadablePropertyInfoMasks
    {
        Type = 0x0f,
        Flags = 0x0f,
        Id = 0x03ff,
        Size = 0x03fff,
    }

    public struct ReadablePropertyInfoT
    {
        private uint value;

        public ushort Id
        {
            get => (ushort)((value >> (int)ReadablePropertyInfoOffsets.Id) & (int)ReadablePropertyInfoMasks.Id);
        }

        public RWPropertyTypes Type
        {
            get => (RWPropertyTypes)(value & (int)ReadablePropertyInfoMasks.Type);
        }

        public bool IsWritable
        {
            get => ((value >> (int)ReadablePropertyInfoOffsets.Flags) & (uint)RWPropertyFlags.IsWritable) == (uint)RWPropertyFlags.IsWritable;
        }

        public bool IsSequential
        {
            get => ((value >> (int)ReadablePropertyInfoOffsets.Flags) & (uint)RWPropertyFlags.IsSequential) == (uint)RWPropertyFlags.IsSequential;
        }

        public bool IsConstant
        {
            get => ((value >> (int)ReadablePropertyInfoOffsets.Flags) & (uint)RWPropertyFlags.IsConstant) == (uint)RWPropertyFlags.IsConstant;
        }

        public ushort Size
        {
            get => (ushort)((value >> (int)ReadablePropertyInfoOffsets.Size) & (int)ReadablePropertyInfoMasks.Size);
        }
    }

    public struct WritablePropertyInfoT
    {
        private int value;

        public ushort Id
        {
            get => (ushort)((value >> 8) & 0x3ff);
            set => this.value |= (value & 0x3ff) << 8;
        }

        public RWPropertyTypes Type
        {
            get => (RWPropertyTypes)(value & 0x0f);
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

    public class WritableProperty
    {
        public WritablePropertyInfoT Info;
        public byte[] Content;

        public WritableProperty(WritablePropertyInfoT info, byte[] content = null)
        {
            Info = info;
            Content = content;
        }
    }

    public struct RequestReadPropertyT
    {
        private ushort value;

        public ushort Id
        {
            get => (ushort)(value & 0x3ff);
            set
            {
                int newValue = this.value;

                newValue &= ~0x3ff;
                newValue |= value;

                this.value = (ushort)newValue;
            }
        }
    }

    public class ReadableProperty
    {
        public ReadablePropertyInfoT Info;

        public byte[] Content;

        public ReadableProperty(ReadablePropertyInfoT info, byte[] content = null)
        {
            this.Info = info;
            Content = content;
        }
    }
}
