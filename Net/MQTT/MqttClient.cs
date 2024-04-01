using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xLibV100.Ports;

namespace xLibV100.Net.MQTT
{
    public class MqttClient : PortBase
    {
        private CancellationTokenSource cancelTokenSource;
        private string address;
        private IPAddress ipAddress;
        private int port;

        protected IMqttClient mqttClient { get; set; }
        protected AutoResetEvent synchronize = new AutoResetEvent(true);

        protected Stopwatch TxStopwatch = new Stopwatch();

        public MqttClient() : base()
        {
            Type = typeof(MqttClient).ToString();
            TransferLayer = TransferLayers.Mqtt;
            Role = Roles.Client;

            cancelTokenSource = new CancellationTokenSource();

            mqttClient = new MqttFactory().CreateMqttClient();
            //mqttClient.Options.WillQualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce;
            mqttClient.ConnectedAsync += ConnectedAsync;
            mqttClient.ConnectingAsync += ConnectingAsync;
            mqttClient.DisconnectedAsync += DisconnectedAsync;
            mqttClient.ApplicationMessageReceivedAsync += ApplicationMessageReceivedAsync;

            SubPorts.CollectionChanged += (sender, e) => { OnPropertyChanged(nameof(NumberOfTopics)); };
        }

        public MqttClient(PortBase port) : this()
        {
            Apply(port);
        }

        private Task ConnectingAsync(MqttClientConnectingEventArgs args)
        {
            return Task.CompletedTask;
        }

        private Task ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            synchronize?.WaitOne();

            TxStopwatch.Stop();

            foreach (MqttTopic subPort in SubPorts)
            {
                if (subPort.IsSubscribed && arg.ApplicationMessage.Topic == subPort.TopicName)
                {
                    subPort.Receive(this,
                        arg,
                        arg.ApplicationMessage.PayloadSegment.Array,
                        arg.ApplicationMessage.PayloadSegment.Array.Length,
                        0);

                    break;
                }
            }

            synchronize?.Set();

            return Task.CompletedTask;
        }

        private Task DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            synchronize?.WaitOne();
            State = States.Idle;
            synchronize?.Set();

            return Task.CompletedTask;
        }

        private Task ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            synchronize?.WaitOne();
            State = States.Connected;
            synchronize?.Set();

            foreach (var element in SubPorts)
            {
                if (element is MqttTopic port && port.TopicName != null && port.IsSubscribed)
                {
                    mqttClient.SubscribeAsync(port.TopicName);
                }
            }

            return Task.CompletedTask;
        }

        [PortProperty(Name = nameof(Address), Key = "Options")]
        public string Address
        {
            get => address;
            set
            {
                if (address != value)
                {
                    try
                    {
                        ipAddress = IPAddress.Parse(value);
                        address = value;
                    }
                    catch
                    {
                        address = value;
                    }
                    OnPropertyChanged(nameof(Address));
                }
            }
        }

        [PortProperty(Name = nameof(Port), Key = "Options")]
        public int Port
        {
            get => port;
            set
            {
                if (port != value)
                {
                    port = value;
                    OnPropertyChanged(nameof(Port));
                }
            }
        }

        public int NumberOfTopics => SubPorts.Count;

        public override object Options
        {
            get => new MqttClientOptions
            {
                Address = Address,
                Port = Port
            };
            set
            {
                if (value is MqttClientOptions options)
                {
                    Address = options.Address;
                    Port = options.Port;
                }
            }
        }

        public override PortResult AddSubPort(PortBase port)
        {
            if (port is MqttTopic subPort
                && base.AddSubPort(subPort) == PortResult.Accept
                && subPort.IsSubscribed)
            {
                mqttClient.SubscribeAsync(subPort.TopicName);
                port.ValuePropertyChanged += SubPortPropertyValueChangedHandler;

                return PortResult.Accept;
            }

            return PortResult.Error;
        }

        private void SubPortPropertyValueChangedHandler(object sender, PropertyChangedEventHandlerArg<object> args)
        {
            MqttTopic subPort = sender as MqttTopic;

            synchronize.WaitOne();

            switch (args.Name)
            {
                case nameof(subPort.IsSubscribed):

                    if (subPort.IsWriteable)
                    {
                        mqttClient.SubscribeAsync(subPort.TopicName);
                    }
                    else
                    {
                        mqttClient.UnsubscribeAsync(subPort.TopicName);
                    }

                    break;
            }

            synchronize.Set();
        }

        public override void RemoveSubPort(PortBase port)
        {
            if (port is MqttTopic subPort)
            {
                base.RemoveSubPort(port);

                mqttClient.UnsubscribeAsync(subPort.TopicName);
                port.ValuePropertyChanged -= SubPortPropertyValueChangedHandler;
            }
        }

        public override PortResult Send(object request)
        {
            if (State != States.Connected || !(request is MqttClientTransmitRequest mqttClientTransmitRequest))
            {
                return PortResult.Error;
            }

            string topic = mqttClientTransmitRequest.Sender as string;

            if (request is MqttClientTransmitRequest req)
            {
                var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(req.Data)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
                .WithRetainFlag(false)
                .Build();

                TxStopwatch.Restart();
                mqttClient.PublishAsync(message);

                return PortResult.Accept;
            }

            return PortResult.Error;
        }

        public override async Task<PortResult> SendAsync(PortTxRequest request)
        {
            if (State != States.Connected || !(request is MqttClientTransmitRequest mqttClientTransmitRequest))
            {
                return PortResult.Error;
            }

            string topic = mqttClientTransmitRequest.Sender as string;

            if (request is MqttClientTransmitRequest req)
            {
                var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(req.Data)
                //.WithRetainFlag()
                .Build();

                await mqttClient.PublishAsync(message);

                return PortResult.Accept;
            }

            return PortResult.Error;
        }

        public override PortResult Connect()
        {
            synchronize.WaitOne();

            if (State == States.Idle)
            {
                var options = new MqttClientOptionsBuilder()
                .WithTcpServer(Address, Port)
                .WithClientId(Id.ToString())
                .WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
                //.WithCredentials("roehrgwl", "hwIdm3L4VljF")
                .Build();

                State = States.Connecting;

                mqttClient.ConnectAsync(options, cancelTokenSource.Token);
            }

            synchronize.Set();

            return State == States.Connecting ? PortResult.Accept : PortResult.Busy;
        }

        public override async Task<PortResult> ConnectAsync()
        {
            synchronize.WaitOne();

            if (State == States.Idle)
            {
                var options = new MqttClientOptionsBuilder()
                .WithTcpServer("90.156.229.205", 1883)
                .WithClientId(Id.ToString())
                //.WithCredentials("roehrgwl", "hwIdm3L4VljF")
                .Build();

                State = States.Connecting;
                synchronize.Set();

                await mqttClient.ConnectAsync(options, cancelTokenSource.Token);
            }
            else
            {
                synchronize.Set();
            }

            return State == States.Connecting ? PortResult.Accept : PortResult.Busy;
        }

        public override PortResult Disconnect()
        {
            synchronize.WaitOne();

            if (State != States.Idle)
            {
                State = States.Disconnecting;

                mqttClient.DisconnectAsync();
            }

            synchronize.Set();

            return PortResult.Accept;
        }

        public override async Task<PortResult> DisconnectAsync()
        {
            synchronize.WaitOne();

            if (State != States.Idle)
            {
                State = States.Disconnecting;
                synchronize.Set();

                await mqttClient.DisconnectAsync();
            }
            else
            {
                synchronize.Set();
            }

            return PortResult.Accept;
        }

        public override async void Dispose()
        {
            base.Dispose();

            if (mqttClient != null)
            {
                foreach (var subport in SubPorts)
                {
                    subport.ValuePropertyChanged -= SubPortPropertyValueChangedHandler;
                }

                cancelTokenSource?.Cancel();
                await mqttClient.DisconnectAsync();
                mqttClient.Dispose();
                mqttClient = null;
            }
        }
    }
}
