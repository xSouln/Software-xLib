using xLibV100.Peripherals.Transactions;
using xLibV100.Transactions;
using xLibV100.Transactions.Common;

namespace xLibV100.Peripherals.GsmControl.Transactions
{
    public partial class Control : Peripherals.Transactions.Control
    {
        [TxTransaction(Action = Actions.GetCredentials)]
        public readonly TxTransaction<ResponseGetInfo, RequestGetByRange> GetCredentials;


        public Control(Gsm model, uint uid) : base(model, uid)
        {

        }
    }

   
}
