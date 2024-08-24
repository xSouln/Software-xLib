﻿using System;
using xLibV100.Adaptation;
using xLibV100.Attributes;

namespace xLibV100.Peripherals.GsmControl
{
    public enum PropertySelector : ushort
    {
        Idle,

        [SynchronizedPropertySelector(Type = RWPropertyTypes.String)]
        IMEI,

        [SynchronizedPropertySelector(Type = RWPropertyTypes.String)]
        APN,

        [SynchronizedPropertySelector(Type = RWPropertyTypes.String)]
        Login,

        [SynchronizedPropertySelector(Type = RWPropertyTypes.String)]
        Password,

        [SynchronizedPropertySelector(Type = RWPropertyTypes.String)]
        SIMCardPassword,

        [SynchronizedPropertySelector(Type = RWPropertyTypes.HalfWord)]
        SignalQuality,

        [SynchronizedPropertySelector(Type = RWPropertyTypes.Object)]
        Status
    }

    public enum InstanceTypes : byte
    {
        SIM7000,
        SIM7600
    }

    public enum StatusBitNumbers
    {
        IMEI_IsReceived,
        SIMCardIsInstalled,
        ModemInitializationIsComplited,
        NetworkIsAvailible,

        TcpIsInitialized,
        TcpIsOpened,

        MqttIsInitialized,
        MqttIsConnected,
        MqttPortIsAvailable,

        NetworkState
    }

    public enum NetworkState
    {
        NonIdentified,
        IsOpened,
        IsClosed
    }

    public struct StatusRegisterT
    {
        public uint status;
        public uint Value
        {
            get => status;
            set => status = value;
        }

        public bool BitIsSet(StatusBitNumbers bitNumber)
        {
            uint mask = (uint)(1 << (int)bitNumber);

            return (status & mask) == mask;
        }

        public bool IMEI_IsReceived => BitIsSet(StatusBitNumbers.IMEI_IsReceived);

        public bool SIMCardIsInstalled => BitIsSet(StatusBitNumbers.SIMCardIsInstalled);

        public bool ModemInitializationIsComplited => BitIsSet(StatusBitNumbers.ModemInitializationIsComplited);

        public bool NetworkIsAvailible => BitIsSet(StatusBitNumbers.NetworkIsAvailible);

        public bool TcpIsInitialized => BitIsSet(StatusBitNumbers.TcpIsInitialized);

        public bool TcpIsOpened => BitIsSet(StatusBitNumbers.TcpIsOpened);

        public bool MqttIsConnected => BitIsSet(StatusBitNumbers.MqttIsConnected);

        public bool MqttIsInitialized => BitIsSet(StatusBitNumbers.MqttIsInitialized);

        public bool MqttPortIsAvailable => BitIsSet(StatusBitNumbers.MqttPortIsAvailable);


        public NetworkState NetworkState
        {
            get => (NetworkState)((status >> (int)StatusBitNumbers.NetworkState) & 0x0f);
        }
    }

}
