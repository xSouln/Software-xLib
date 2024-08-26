using xLibV100.Transactions.Common;
using xLibV100.Transactions;

namespace xLibV100.Peripherals.Transactions
{
    public partial class Control : ControlBase
    {
        [TxTransaction(Action = Actions.GetInfo)]
        public readonly TxTransaction<ResponseGetInfo> GetInfo;

        [TxTransaction(Action = Actions.GetInstances)]
        public readonly TxTransaction<ResponeGetInstances, RequestGetInstances> GetInstances;

        [TxTransaction(Action = Actions.GetProperty)]
        public readonly TxTransaction<ResponseGetProperties, RequestGetProperties> GetProperties;

        [TxTransaction(Action = Actions.SetProperty)]
        public readonly TxTransaction<ResponseGetProperties, RequestSetProperties> SetProperties;


        public Control(PeripheralBase model, uint uid) : base(model, uid)
        {

        }
    }
}
