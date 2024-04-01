using MQTTnet;
using MQTTnet.Server;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using xLibV100.Components;
using xLibV100.Controls;


namespace xLibV100.MQTT.Broker
{
    public class Control : ModelBase
    {
        private MqttServer mqttServer;
        private string ip;
        private IPAddress ip_address;
        private int port;

        public Control(string ip, int port)
        {
            Ip = ip;
            Port = port;

            var optionsBuilder = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(port)
            .WithDefaultEndpointBoundIPAddress(ip_address);

            mqttServer = new MqttFactory().CreateMqttServer(optionsBuilder.Build());

            mqttServer.InterceptingPublishAsync += InterceptingPublishAsync;
            mqttServer.ClientConnectedAsync += ClientConnectedAsync;

            mqttServer.ApplicationMessageNotConsumedAsync += ApplicationMessageNotConsumedAsync;
            mqttServer.ClientSubscribedTopicAsync += ClientSubscribedTopicAsync;
        }

        private Task ClientSubscribedTopicAsync(ClientSubscribedTopicEventArgs arg)
        {
            xTracer.Message(nameof(ClientSubscribedTopicAsync), arg.ClientId);
            return Task.CompletedTask;
        }

        private Task ApplicationMessageNotConsumedAsync(ApplicationMessageNotConsumedEventArgs arg)
        {
            xTracer.Message(nameof(ApplicationMessageNotConsumedAsync), arg.ApplicationMessage.Topic);
            return Task.CompletedTask;
        }

        private Task ClientConnectedAsync(ClientConnectedEventArgs arg)
        {
            xTracer.Message(nameof(ClientConnectedAsync), arg.ClientId);
            return Task.CompletedTask;
        }

        private Task InterceptingPublishAsync(InterceptingPublishEventArgs arg)
        {
            xTracer.Message(nameof(InterceptingPublishAsync),
                arg.ClientId + " topic: " + arg.ApplicationMessage.Topic + " data: " + Encoding.Default.GetString(arg.ApplicationMessage.PayloadSegment.Array));
            return Task.CompletedTask;
        }

        public async void Start()
        {
            await mqttServer.StartAsync();
        }

        public async void Stop()
        {
            await mqttServer.StopAsync();
        }

        public string Ip
        {
            get => ip;
            protected set
            {
                if (ip != value)
                {
                    try
                    {
                        ip_address = System.Net.IPAddress.Parse(value);
                        ip = value;

                        OnPropertyChanged(nameof(Ip));
                    }
                    catch
                    {
                        return;
                    }
                }
            }
        }

        public int Port
        {
            get => port;
            protected set
            {
                if (port != value)
                {
                    port = value;
                    OnPropertyChanged(nameof(Port));
                }
            }
        }
    }
}
