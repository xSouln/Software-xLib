using xLibV100.Transceiver;

namespace xLibV100.Transactions.Common
{
    public interface IControl
    {
        bool Identification(RxPacketManager manager, xContent content);
    }
}
