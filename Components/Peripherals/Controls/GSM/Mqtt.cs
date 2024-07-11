using System.Collections.ObjectModel;
using xLibV100.Controls;
using xLibV100.UI;

namespace xLibV100.Peripherals.GsmControl
{
    public partial class Instance
    {
        public class MqttInstance : UINotifyPropertyChanged
        {
            protected string brokerAddress = "";
            protected string clientId = "";
            protected ushort port;
            protected ushort countOfTopics;

            public ObservableCollection<string> PublishedTopics { get; set; } = new ObservableCollection<string>();
            public ObservableCollection<string> SubscribedTopics { get; set; } = new ObservableCollection<string>();


            [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
            public string BrokerAddress
            {
                get => brokerAddress;
                protected set
                {
                    if (value != brokerAddress)
                    {
                        brokerAddress = value;
                        OnPropertyChanged(nameof(BrokerAddress), brokerAddress);
                    }
                }
            }


            [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
            public ushort Port
            {
                get => port;
                protected set
                {
                    if (value != port)
                    {
                        port = value;
                        OnPropertyChanged(nameof(Port), port);
                    }
                }
            }


            [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
            public string ClientId
            {
                get => clientId;
                protected set
                {
                    if (value != clientId)
                    {
                        clientId = value;
                        OnPropertyChanged(nameof(ClientId), clientId);
                    }
                }
            }


            [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
            public ushort CountOfTopics
            {
                get => countOfTopics;
                protected set
                {
                    if (value != countOfTopics)
                    {
                        countOfTopics = value;
                        OnPropertyChanged(nameof(CountOfTopics), countOfTopics);
                    }
                }
            }
        }
    }
}
