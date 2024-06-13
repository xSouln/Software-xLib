using xLibV100.Transceiver;

namespace xLibV100.Transactions.Common
{
    /// <summary>
    /// шаблон транзакции приема. данный шаблон подходит для описание событий приходящих с устройства
    /// </summary>
    /// <typeparam name="TResponse">тип предостовляющий набор параметров ответа</typeparam>
    public class RxTransaction<TResponse> : RxTransactionBase<TResponse, object>
        where TResponse : IResponseAdapter, new()
    {
        public TransactionsBase Control { get; set; }

        public RxTransaction(TransactionsBase control, object action) : base(action)
        {
            Control = control;
            ResponseHeader = PacketHeaderT.Init(PacketHeaderDescription.Response, control.DeviceId, control.UID);

            control.ValuePropertyChanged += PropertyValueChanged;
        }

        private void PropertyValueChanged(object sender, PropertyChangedEventHandlerArg<object> args)
        {
            switch (args.Name)
            {
                case nameof(Control.DeviceId):
                    ResponseHeader.DeviceId = Control.DeviceId;
                    break;

                case nameof(Control.UID):
                    ResponseHeader.UID = Control.UID;
                    break;
            }
        }
    }
}
