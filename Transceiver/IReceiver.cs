namespace xLibV100.Transceiver
{
    public enum ReceiverResult
    {
        NotFound,
        Accept,

        NotSuported
    }

    public interface IReceiver
    {
        string Name { get; set; }
        ReceiverResult Receive(RxPacketManager manager, xContent content);
        IResponseAdapter GetResponseAdapter();
    }
}
