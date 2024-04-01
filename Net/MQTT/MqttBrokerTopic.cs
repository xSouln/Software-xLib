using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xLibV100.Common;
using xLibV100.Controls;
using xLibV100.Ports;

namespace xLibV100.Net.MQTT
{
    public class MqttBrokerTopic : PortBase
    {
        public string RxTopicName { get; set; }
        public string TxTopicName { get; set; }
        protected IMqttClient mqttClient { get; set; }

        protected AutoResetEvent synchronize = new AutoResetEvent(true);

        public MqttBroker Broker
        {
            get => parent != null ? (MqttBroker)parent : null;
            set => parent = value;
        }

        public MqttBrokerTopic() : base()
        {
            Type = typeof(MqttBrokerTopic).ToString();
            TransferLayer = TransferLayers.Mqtt;
            Role = Roles.Universal;
        }

        public MqttBrokerTopic(PortBase port) : this()
        {
            Apply(port);
        }

        protected override PortResult SetParent(PortBase port)
        {
            if (port == null)
            {
                Broker.ConnectionStateChanged -= ConnectionChangedHandler;
                Broker = null;
                return PortResult.Accept;
            }

            if (Broker != null)
            {
                Broker.ConnectionStateChanged -= ConnectionChangedHandler;
            }

            if (port is MqttBroker broker)
            {
                Broker = broker;

                Broker.ConnectionStateChanged += ConnectionChangedHandler;

                return PortResult.Accept;
            }

            return PortResult.NotSupported;
        }

        private async void ConnectionChangedHandler(PortBase port, ConnectionStateChangedEventHandlerArg arg)
        {
            if (port is MqttBroker broker)
            {
                synchronize.WaitOne();

                State = broker.State;

                switch (arg.State)
                {
                    case States.Started:
                        var options = new MqttClientOptionsBuilder()
                        .WithTcpServer(broker.Ip, broker.Port)
                        .WithClientId(Id.ToString())
                        .Build();

                        mqttClient = new MqttFactory().CreateMqttClient();
                        mqttClient.ConnectedAsync += ConnectedAsync;
                        mqttClient.DisconnectedAsync += DisconnectedAsync;
                        mqttClient.ApplicationMessageReceivedAsync += ApplicationMessageReceivedAsync;
                        await mqttClient.ConnectAsync(options);
                        break;

                    case States.Idle:
                        if (mqttClient != null)
                        {
                            await mqttClient.DisconnectAsync();
                            mqttClient.Dispose();
                            mqttClient = null;
                        }
                        break;
                }

                synchronize.Set();
            }
        }

        private async void send(byte[] data)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(TxTopicName)
                .WithPayload(data)
                .WithRetainFlag()
                .Build();

            await mqttClient.PublishAsync(message);
        }

        public override PortResult Send(byte[] data, int offset, int size)
        {
            if (data == null || !(data.Length > 0) || (offset + size) > data.Length)
            {
                return PortResult.DataError;
            }

            if (State != States.Connected)
            {
                return PortResult.ConnectionError;
            }

            byte[] payload = new byte[size];

            xMemory.Copy(payload, 0, data, offset, size);
            send(payload);

            return PortResult.Accept;
        }

        private Task ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            /*
            xTracer.Message(nameof(InterceptingPublish),
                arg.ClientId + " topic: " + arg.ApplicationMessage.Topic + " data: " + Encoding.Default.GetString(arg.ApplicationMessage.PayloadSegment.Array));*/
            var data = new byte[arg.ApplicationMessage.PayloadSegment.Array.Length];
            xMemory.Copy(data, 0, arg.ApplicationMessage.PayloadSegment.Array, 0, arg.ApplicationMessage.PayloadSegment.Array.Length);
            unsafe
            {
                fixed (byte* ptr = data)
                {
                    iReceive(new ReceivedPacketHandlerArg
                    {
                        Data = data,
                        DataPtr = ptr,
                        PacketSize = data.Length - 1
                    });
                }
            }
            return Task.CompletedTask;
        }

        private Task DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            State = States.Idle;

            return Task.CompletedTask;
        }

        private async Task ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            State = States.Connected;

            if (mqttClient != null)
            {
                await mqttClient.SubscribeAsync(RxTopicName);
            }
        }

        public Results InterceptingPublish(PortBase port, InterceptingPublishEventArgs arg)
        {
            /*
            xTracer.Message(nameof(InterceptingPublish),
                arg.ClientId + " topic: " + arg.ApplicationMessage.Topic + " data: " + Encoding.Default.GetString(arg.ApplicationMessage.PayloadSegment.Array));
            */
            return Results.Accept;
        }

        public override object Options
        {
            get => new MqttBrokerTopicOptions
            {
                RxTopicName = RxTopicName,
                TxTopicName = TxTopicName
            };
            set
            {
                if (value is MqttBrokerTopicOptions options)
                {
                    RxTopicName = options.RxTopicName;
                    TxTopicName = options.TxTopicName;
                }
            }
        }

        public override async void Dispose()
        {
            base.Dispose();

            if (Broker != null)
            {
                Broker.ConnectionStateChanged -= ConnectionChangedHandler;
            }

            if (mqttClient != null)
            {
                await mqttClient.DisconnectAsync();
                mqttClient.Dispose();
                mqttClient = null;
            }
        }
    }
}
