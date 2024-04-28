using xLibV100.Transceiver;

namespace xLibV100.Transactions.Common
{
    /// <summary>
    /// шаблон транзакции приема. данный шаблон подходит для описание транзакций для установления/прием параметров
    /// </summary>
    /// <typeparam name="TResponse">тип предостовляющий набор параметров ответа</typeparam>
    public class TxTransaction<TResponse> : TxTransactionBase<TResponse, object>
        where TResponse : IResponseAdapter, new()
    {
        public TransactionsBase Control { get; set; }

        public TxTransaction(TransactionsBase control, object action) : this(control, action, null)
        {
            control.ValuePropertyChanged += PropertyValueChanged;
        }

        public TxTransaction(TransactionsBase control, object action, object arg) : base(action)
        {
            Control = control;
            Handle = control.Handle;

            RequestHeader = PacketHeaderT.Init(PacketHeaderDescription.Request, control.DeviceId, control.UID);
            ResponseHeader = PacketHeaderT.Init(PacketHeaderDescription.Response, control.DeviceId, control.UID);
        }

        private void PropertyValueChanged(object sender, PropertyChangedEventHandlerArg<object> args)
        {
            switch (args.Name)
            {
                case nameof(Control.DeviceId):
                    RequestHeader.DeviceId = Control.DeviceId;
                    ResponseHeader.DeviceId = Control.DeviceId;
                    break;

                case nameof(Control.UID):
                    RequestHeader.UID = Control.UID;
                    ResponseHeader.UID = Control.UID;
                    break;
            }    
        }

        protected override object Create()
        {
            return new TxTransaction<TResponse>(Control, action, null);
        }
    }

    /// <summary>
    ///  шаблон транзакции приема. данный шаблон подходит для описание транзакций для установления/прием параметров
    /// </summary>
    /// <typeparam name="TResponse">>тип предостовляющий набор параметров ответа</typeparam>
    /// <typeparam name="TRequest">>тип предостовляющий набор параметров требуемых для запроса</typeparam>
    public class TxTransaction<TResponse, TRequest> : TxTransactionBase<TResponse, object, TRequest>
        where TResponse : IResponseAdapter, new()
        where TRequest : IRequestAdapter
    {
        public TransactionsBase Control { get; set; }

        public TxTransaction(TransactionsBase control, object action) : this(control, action, null)
        {
            control.ValuePropertyChanged += PropertyValueChanged;
        }

        public TxTransaction(TransactionsBase control, object action, object arg) : base(action)
        {
            Control = control;
            Handle = control.Handle;

            RequestHeader = PacketHeaderT.Init(PacketHeaderDescription.Request, control.DeviceId, control.UID);
            ResponseHeader = PacketHeaderT.Init(PacketHeaderDescription.Response, control.DeviceId, control.UID);
        }

        private void PropertyValueChanged(object sender, PropertyChangedEventHandlerArg<object> args)
        {
            switch (args.Name)
            {
                case nameof(Control.DeviceId):
                    RequestHeader.DeviceId = Control.DeviceId;
                    ResponseHeader.DeviceId = Control.DeviceId;
                    break;

                case nameof(Control.UID):
                    RequestHeader.UID = Control.UID;
                    ResponseHeader.UID = Control.UID;
                    break;
            }
        }

        protected override object Create()
        {
            return new TxTransaction<TResponse, TRequest>(Control, action, null);
        }
    }
}
