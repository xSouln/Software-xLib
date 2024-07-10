using System;

namespace xLib.Peripherals.Ethernet
{
    public enum ControlBitNumbers : ushort
    {
        DHCP_IsEnabled,
        SNTP_IsEnabled,

    }

    [Flags]
    public enum ControlMask : ushort
    {
        DHCP_IsEnabled = 1 << ControlBitNumbers.DHCP_IsEnabled,
        SNTP_IsEnabled = 1 << ControlBitNumbers.SNTP_IsEnabled,
    }

    public enum StatusBitNumber : ushort
    {
        PhyIsConnecnted,
        StaticAddressIsEnabled,
        DHCP_IsComplited,
        SNTP_IsComplited,
    }
}
