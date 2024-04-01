using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using xLibV100;
using xLibV100.Common;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xWiFi.Types
{
    public struct CoreInfoT : IResponseAdapter
    {
        public byte NumbersOfModules;

        public unsafe object Recieve(RxPacketManager manager, xContent content)
        {
            this = *(CoreInfoT*)content.Data;

            return this;
        }
    }

    [Flags]
    public enum Module : byte
    {
        Number1 = 0x01,
        Number2 = 0x02,
        Number3 = 0x04,
        Number4 = 0x08,
        Number5 = 0x10,
        Number6 = 0x20,
        Number7 = 0x40,
        Number8 = 0x80,

        All = 0xFF
    }

    public enum Modes : byte
    {
        STA,
        AP
    }

    public enum FlagsOffset
    {
        Modes = 0,
        SSID_BroadcastIsEnable = 2
    }

    [Flags]
    public enum FlagsMask
    {
        Modes = 0x03 << FlagsOffset.Modes,
        SSID_BroadcastIsEnable = 0x01 << FlagsOffset.SSID_BroadcastIsEnable
    }

    public struct SSID
    {
        public unsafe fixed byte Value[32];
    }

    public struct ConfigT : IResponseAdapter, IRequestAdapter
    {
        public unsafe fixed byte SSID[32];
        public unsafe fixed byte Password[64];

        public int Flags;

        public byte RadioBand;
        public byte WideChannel;
        public byte StandardChannel;

        public byte SecurityMode;
        public byte Encription;

        public Modes Mode
        {
            get => (Modes)((Flags & (int)FlagsMask.Modes) >> (int)FlagsOffset.Modes);
            set
            {
                int temp = Flags;
                temp &= ~(int)FlagsMask.Modes;

                Flags = temp | ((int)value << (int)FlagsOffset.Modes);
            }
        }
        public bool SSID_BroadcastIsEnable
        {
            get => (Flags & (int)FlagsMask.SSID_BroadcastIsEnable) > 0;
            set
            {
                int state = value ? 1 : 0;

                int temp = Flags;
                temp &= ~(int)FlagsMask.SSID_BroadcastIsEnable;

                Flags = temp | (state << (int)FlagsOffset.SSID_BroadcastIsEnable);
            }
        }

        public unsafe object Recieve(RxPacketManager manager, xContent content)
        {
            this = *(ConfigT*)content.Data;

            return this;
        }

        public unsafe int GetSize()
        {
            return sizeof(ConfigT);
        }

        public int Add(List<byte> buffer)
        {
            return xMemory.Add(buffer, this);
        }
    }

    public enum StateFlagsOffset : byte
    {
        IsEnable = 0,
        IsStarted,
        IsInit
    }

    [Flags]
    public enum StateFlags : byte
    {
        IsEnable = 0x01 << StateFlagsOffset.IsEnable,
        IsStarted = 1 << StateFlagsOffset.IsStarted,
        IsInit = 1 << StateFlagsOffset.IsInit,
    }

    public enum States : byte
    {
        Idle,

        //STA
        Connecting,
        Connected,
        Disconnecting,

        //AP
        Opening,
        Open,
        Closing
    }

    public enum Errors : byte
    {
        NoErrors,
    }

    public struct StatusT : IResponseAdapter
    {
        public StateFlags Flags;
        public States State;
        public Errors Error;
#pragma warning disable CS0169 // Поле "StatusT.Reserved" никогда не используется.
        private readonly byte Reserved;
#pragma warning restore CS0169 // Поле "StatusT.Reserved" никогда не используется.

        public bool IsEnable
        {
            get => (Flags & StateFlags.IsEnable) > 0;
            set
            {
                int state = value ? 1 : 0;

                int temp = (int)Flags;
                temp &= ~(int)StateFlags.IsEnable;

                Flags = (StateFlags)(temp | (state << (int)StateFlagsOffset.IsEnable));
            }
        }

        public bool IsStarted => (Flags & StateFlags.IsStarted) > 0;
        public bool IsInit => (Flags & StateFlags.IsInit) > 0;

        public unsafe object Recieve(RxPacketManager manager, xContent content)
        {
            this = *(StatusT*)content.Data;

            return this;
        }
    }

    public struct AddressT : IResponseAdapter
    {
        public uint Ip;
        public uint Netmask;
        public uint Gateway;

        public unsafe object Recieve(RxPacketManager manager, xContent content)
        {
            this = *(AddressT*)content.Data;

            return this;
        }
    }
}
