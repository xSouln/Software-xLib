using MQTTnet.Client;
using xLibV100.Ports;
using xLibV100.Transceiver;

namespace xLibV100.Net.MQTT
{
    public class MqttTopic : PortBase
    {
        protected string topicName = "topic1";
        protected bool isSubscribed = false;
        protected bool isWriteable = false;

        public RxReceiver Receiver = new RxReceiver(0x1fff, new byte[] { (byte)'\r' });

        public event xPropertyChangedEventHandler<MqttTopic, xPropertyChangedEventHandlerArgs> PropertyChangedEvent;


        [PortProperty(Name = nameof(TopicName), Key = "Options")]
        public string TopicName
        {
            get => topicName;
            set
            {
                topicName = value;
                OnPropertyChanged(nameof(TopicName), topicName);
            }
        }

        private void RxPacketReceiver(RxReceiver rx, ReceivedPacketHandlerArg arg)
        {
            iReceive(arg);
        }

        public unsafe override PortResult Receive(object sender, object context, byte[] data, int dataLength, int dataOffset)
        {
            Receiver.Add(data, dataLength, dataOffset);
            return PortResult.Accept;
        }

        public override void ClearRxBuffer()
        {
            Receiver.Clear();
        }

        [PortProperty(Name = nameof(IsSubscribed), Key = "Options")]
        public bool IsSubscribed
        {
            get => isSubscribed;
            set
            {
                isSubscribed = value;
                OnPropertyChanged(nameof(IsSubscribed), isSubscribed);
            }
        }

        [PortProperty(Name = nameof(IsWriteable), Key = "Options")]
        public bool IsWriteable
        {
            get => isWriteable;
            set
            {
                isWriteable = value;
                OnPropertyChanged(nameof(IsWriteable), isWriteable);
            }
        }

        public MqttClient Client
        {
            get => parent != null ? (MqttClient)parent : null;
            set => parent = value;
        }

        public MqttTopic() : base()
        {
            Receiver.PacketReceiver += RxPacketReceiver;

            Type = typeof(MqttTopic).ToString();
            TransferLayer = TransferLayers.Mqtt;
            Role = Roles.Universal;
        }

        public MqttTopic(PortBase port) : this()
        {
            if (port.TransferLayer == TransferLayers.Mqtt)
            {
                Apply(port);
            }
        }

        protected override PortResult SetParent(PortBase port)
        {
            if (port == null)
            {
                Client.ConnectionStateChanged -= ConnectionChangedHandler;
                Client = null;
                return PortResult.Accept;
            }

            if (Client != null)
            {
                Client.ConnectionStateChanged -= ConnectionChangedHandler;
            }

            if (port is MqttClient client)
            {
                Client = client;
                Client.ConnectionStateChanged += ConnectionChangedHandler;
                Client.PacketReceiver += PacketReceiverHandler;

                return PortResult.Accept;
            }

            return PortResult.NotSupported;
        }

        public override object Options
        {
            get => new MqttTopicOptions
            {
                TopicName = TopicName,
                IsSubscribed = IsSubscribed,
                IsWriteable = IsWriteable,
            };
            set
            {
                if (value is MqttTopicOptions options)
                {
                    TopicName = options.TopicName;
                    IsSubscribed = options.IsSubscribed;
                    IsWriteable = options.IsWriteable;

                    this.options = options;

                    PropertyChangedEvent?.Invoke(this, new xPropertyChangedEventHandlerArgs
                    {
                        Name = nameof(Options),
                        Value = options
                    });
                }
            }
        }

        private void PacketReceiverHandler(PortBase port, ReceivedPacketHandlerArg arg)
        {
            if (arg.Content is MqttApplicationMessageReceivedEventArgs content && content.ApplicationMessage.Topic == TopicName)
            {
                iReceive(arg);
            }
        }

        private void ConnectionChangedHandler(PortBase port, ConnectionStateChangedEventHandlerArg arg)
        {
            State = port.State;
        }

        public override PortResult Send(byte[] data, int offset, int size)
        {
            if (Client == null)
            {
                return PortResult.InternalError;
            }

            if (!IsWriteable)
            {
                return PortResult.NotSupported;
            }

            if (data == null || !(data.Length > 0) || (offset + size) > data.Length)
            {
                return PortResult.DataError;
            }

            return Client.Send(new MqttClientTransmitRequest
            {
                Data = data,
                Offset = offset,
                Size = size,
                Sender = TopicName
            });
        }

        public override void Dispose()
        {
            base.Dispose();
            SetParent(null);
        }
    }
}
