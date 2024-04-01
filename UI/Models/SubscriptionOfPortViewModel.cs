using xLibV100.UI;
using System.Windows.Media;
using xLibV100.Ports;

namespace xLibV100.Common.UI.Models
{
    public class SubscriptionOfPortViewModel : ViewModelBase, ISubscriptionOfPortViewModel
    {
        protected bool portIsSelected;

        public PortBase Port { get; set; }

        public SubscriptionOfPortViewModel(PortBase port)
        {
            Port = port;
            port.ConnectionStateChanged += ConnectionChanged;
        }

        private void ConnectionChanged(PortBase port, ConnectionStateChangedEventHandlerArg arg)
        {
            OnPropertyChanged(nameof(BackgroundForState));
        }

        public bool TxIsSelected
        {
            get => portIsSelected;
            set
            {
                portIsSelected = value;
                OnPropertyChanged(nameof(TxIsSelected));
                OnPropertyChanged(nameof(BackgroundForTx));
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public Brush BackgroundForState
        {
            get
            {
                switch (Port.State)
                {
                    case States.Connecting:
                        return UIProperty.YELLOW;

                    case States.Idle:
                        return UIProperty.RED;

                    case States.Connected:
                        return UIProperty.GREEN;

                    default: return null;
                }
            }
        }

        public Brush BackgroundForTx => portIsSelected ? UIProperty.GREEN : null;
    }
}
