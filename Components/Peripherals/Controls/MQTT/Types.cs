namespace xLibV100.Peripherals.MqttControl
{
    public enum ClientStates : byte
    {
        Disconnected,

        Connecnting,
        Connected,

        Disconnecting
    }

    public enum PropertySelector
    {
        None = 0,
    }
}
