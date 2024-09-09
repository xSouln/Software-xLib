using System.Threading.Tasks;
using xLibV100.Transactions;

namespace xLibV100.Adaptation
{
    public delegate Task TransferRequestReceiver(object sender, TxTransactionBase transaction);
}
