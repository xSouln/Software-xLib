using MQTTnet;
using MQTTnet.Server;
using MQTTnet.Client;
using System;
using System.Net;
using System.Threading.Tasks;
using xLibV100.Common;
using xLibV100.Controls;
using xLibV100.Ports;
using xLibV100.Components;

namespace xLibV100.Net.MQTT
{
    public class MqttBroker : PortBase
    {
        public MqttServer Server { get; set; }
        private string ip;
        private IPAddress ip_address;
        private int port;

        public MqttBroker() : base()
        {
            Type = typeof(MqttBroker).ToString();
            TransferLayer = TransferLayers.Mqtt;
            Role = Roles.Server;
        }

        private Task StoppedAsync(EventArgs arg)
        {
            State = States.Idle;
            return Task.CompletedTask;
        }

        private Task StartedAsync(EventArgs arg)
        {
            State = States.Started;
            return Task.CompletedTask;
        }

        private Task ClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs arg)
        {
            return Task.CompletedTask;
        }

        private Task ApplicationMessageNotConsumedAsync(ApplicationMessageNotConsumedEventArgs arg)
        {
            return Task.CompletedTask;
        }

        private Task ClientConnectedAsync(ClientConnectedEventArgs arg)
        {
            return Task.CompletedTask;
        }

        private Task InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            /*
            if (SubPorts != null)
            {
                foreach (MqttBrokerTopic port in SubPorts)
                {
                    if (port.InterceptingPublish(this, arg) == Results.Accept)
                    {
                        break;
                    }
                }
            }*/
            byte[] payload = arg.ApplicationMessage.PayloadSegment.Array;
            string data = arg.ApplicationMessage.PayloadSegment != null ? System.Text.Encoding.UTF8.GetString(payload) : "null";

            unsafe
            {
                xMemory.Convert(out ushort[] points, payload);
            }

            xTracer.Message("MQTT Broker", "topic: " + arg.ApplicationMessage.Topic + "\rdata: " + data);

            return Task.CompletedTask;
        }

        public override object Options
        {
            get => new MqttBrokerOptions
            {
                Ip = Ip,
                Port = Port
            };
            set
            {
                if (value is MqttBrokerOptions options)
                {
                    Ip = options.Ip;
                    Port = options.Port;
                }
            }
        }

        public string Ip
        {
            get => ip;
            set
            {
                if (ip != value)
                {
                    try
                    {
                        ip_address = IPAddress.Parse(value);
                        ip = value;
                    }
                    catch
                    {

                    }
                    OnPropertyChanged(nameof(Ip));
                }
            }
        }

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

        public override PortResult Start()
        {
            if (State == States.Idle)
            {
                State = States.Starting;

                var optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(port)
                .WithDefaultEndpointBoundIPAddress(ip_address);

                Server = new MqttFactory().CreateMqttServer(optionsBuilder.Build());

                //Server.InterceptingPublishAsync += InterceptingPublishAsync;
                Server.ClientConnectedAsync += ClientConnectedAsync;

                Server.ApplicationMessageNotConsumedAsync += ApplicationMessageNotConsumedAsync;
                Server.ClientSubscribedTopicAsync += ClientSubscribedTopicAsync;

                Server.StartedAsync += StartedAsync;
                Server.StoppedAsync += StoppedAsync;

                Server.StartAsync();

                return PortResult.Accept;
            }

            return PortResult.Busy;
        }

        public async override Task<PortResult> StartAsync()
        {
            if (State == States.Idle)
            {
                State = States.Starting;

                var optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(port)
                .WithDefaultEndpointBoundIPAddress(ip_address);

                Server = new MqttFactory().CreateMqttServer(optionsBuilder.Build());

                //Server.InterceptingPublishAsync += InterceptingPublishAsync;
                Server.ClientConnectedAsync += ClientConnectedAsync;

                Server.ApplicationMessageNotConsumedAsync += ApplicationMessageNotConsumedAsync;
                Server.ClientSubscribedTopicAsync += ClientSubscribedTopicAsync;

                Server.StartedAsync += StartedAsync;
                Server.StoppedAsync += StoppedAsync;

                await Server.StartAsync();

                return PortResult.Accept;
            }

            return PortResult.Busy;
        }

        public override PortResult Stop()
        {
            if (State == States.Starting || State == States.Started)
            {
                Server.StopAsync();
            }

            return PortResult.Accept;
        }

        public override async Task<PortResult> StopAsync()
        {
            if (State == States.Starting || State == States.Started)
            {
                await Server?.StopAsync();
            }

            return PortResult.Accept;
        }

        public override async void Dispose()
        {
            base.Dispose();

            if (Server != null)
            {
                await Server.StopAsync();
            }

            Server?.Dispose();
        }
    }
}
