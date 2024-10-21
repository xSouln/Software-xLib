using System.Threading.Tasks;
using xLibV100.Transactions;
using xLibV100.Transceiver;

namespace xLibV100.Adaptation
{
    public delegate Task TransferRequestReceiver(object sender, RequestBase transaction);
}
