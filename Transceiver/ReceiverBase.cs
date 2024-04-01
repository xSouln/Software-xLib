using System.ComponentModel;

namespace xLibV100.Transceiver
{
    public delegate void ReceiverEventReceive<TData>(RxPacketManager manager, TData data);

    public abstract class ReceiverBase : INotifyPropertyChanged, IReceiver
    {
        protected string name = "";
        protected IResponseAdapter result;
        protected ReceiverResult receiverResult;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IResponseAdapter GetResponseAdapter() => result;

        public ReceiverResult ReceiverResult => receiverResult;

        public virtual string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public virtual ReceiverResult Receive(RxPacketManager manager, xContent content)
        {
            receiverResult = ReceiverResult.NotFound;

            return ReceiverResult.NotFound;
        }
    }
}
