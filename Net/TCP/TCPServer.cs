using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using xLibV100.Components;
using xLibV100.Ports;

namespace xLibV100.Net
{
    public class TCPServer : PortBase
    {
        private TcpListener server;
        private Thread serverThread;
        protected string ip = "127.0.0.10";
        protected int port = 5000;
        protected int queueSize = 10;
        protected int connections = 0;
        protected int maxConnections = 0;
        protected int totalNumberOfSockets = 0;

        private Semaphore semaphoreQueueSize;
        protected AutoResetEvent coreSynchronize = new AutoResetEvent(true);
        protected AutoResetEvent addPortSynchronizer = new AutoResetEvent(true);

        public TCPServer()
        {
            TransferLayer = TransferLayers.Tcp;
            Role = Roles.Server;

            Type = typeof(TCPServer).ToString();
        }

        public static TCPServer Create(PortBase port)
        {
            TCPServer server = null;

            if (port.Type == typeof(TCPServer).ToString())
            {
                server = new TCPServer();
                server.Name = port.Name;
                server.Id = port.Id;
                server.Options = port.Options;
            }

            return server;
        }

        public override object Options
        {
            get => new TCPServerOptions
            {
                Ip = ip,
                Port = port,
                MaxCountOfClients = QueueSize
            };
            set
            {
                if (value is  TCPServerOptions request)
                {
                    Ip = request.Ip;
                    Port = request.Port;
                    QueueSize = request.MaxCountOfClients;
                }
            }
        }

        [PortProperty(Name = nameof(Ip), Key = "Options")]
        public string Ip
        {
            get => ip;
            set
            {
                if (value != ip)
                {
                    try
                    {
                        IPAddress.Parse(value);
                    }
                    catch
                    {
                        return;
                    }

                    ip = value;
                    OnPropertyChanged(nameof(Ip));
                }
            }
        }

        [PortProperty(Name = nameof(Port), Key = "Options")]
        public int Port
        {
            get => port;
            set
            {
                if (value != port)
                {
                    port = value;
                    OnPropertyChanged(nameof(Port));
                }
            }
        }

        [PortProperty(Name = nameof(QueueSize), Key = "Options")]
        public int QueueSize
        {
            get => queueSize;
            set
            {
                if (value != queueSize)
                {
                    queueSize = value;
                    OnPropertyChanged(nameof(QueueSize));
                }
            }
        }

        [PortProperty(Name = nameof(Connections), Key = "Info")]
        public int Connections
        {
            get => connections;
            set
            {
                if (value != connections)
                {
                    connections = value;
                    OnPropertyChanged(nameof(Connections));
                }
            }
        }

        protected void MainThread()
        {
            semaphoreQueueSize = new Semaphore(QueueSize, QueueSize);
            xTracer.Message("tcp server: server started");

            try
            {
                IPAddress localAddr = IPAddress.Parse(Ip);
                server = new TcpListener(localAddr, Port);

                server.Start();
                State = States.Started;

                while (true)
                {
                    semaphoreQueueSize.WaitOne();
                    TcpClient client = server.AcceptTcpClient();

                    xTracer.Message("tcp server: client accept");

                    Connections++;
                    totalNumberOfSockets++;

                    TCPClient incomingClient = new TCPClient() { Name = "client: " + totalNumberOfSockets };
                    incomingClient.ConnectionStateChanged += IncomingClientConnectionChangedHandler;
                    incomingClient.PacketReceiver += IncomingClientPacketReceiverHandler;

                    foreach (var bridge in bridges)
                    {
                        incomingClient.AddBridge(bridge);
                    }

                    if (incomingClient.Connect(client) == PortResult.Accept)
                    {
                        addPortSynchronizer.WaitOne();
                        AddSubPort(incomingClient);
                        addPortSynchronizer.Set();

                        xTracer.Message("tcp server: client added");
                    }
                }
            }
            catch (Exception ex)
            {
                xTracer.Message("tcp server: " + ex);
            }
            finally
            {
                coreSynchronize.WaitOne();
                serverThread = null;
                Close();
                coreSynchronize.Set();
            }
        }

        private void IncomingClientPacketReceiverHandler(PortBase port, ReceivedPacketHandlerArg arg)
        {
            OnReceive(port, arg);

            /*int index = SubPorts.IndexOf(port);
            if (index != -1)
            {
                iReceive(arg);
            }
            else
            {
                OnReceive(port, arg);
            }*/
        }

        private void IncomingClientConnectionChangedHandler(PortBase port, ConnectionStateChangedEventHandlerArg arg)
        {
            if (arg.State == States.Idle)
            {
                addPortSynchronizer.WaitOne();

                port.Dispose();
                RemoveSubPort(port);
                Connections--;
                semaphoreQueueSize.Release();

                addPortSynchronizer.Set();
            }
        }

        public override void Close()
        {
            State = States.Stopping;

            server?.Stop();
            server = null;

            serverThread?.Abort();
            serverThread = null;

            while (SubPorts.Count > 0)
            {
                SubPorts[0].Dispose();
            }

            Connections = 0;
            totalNumberOfSockets = 0;

            semaphoreQueueSize?.Close();
            semaphoreQueueSize?.Dispose();
            semaphoreQueueSize = null;

            State = States.Idle;
        }

        public override PortResult Start()
        {
            if (server != null)
            {
                return PortResult.Error;
            }

            coreSynchronize.WaitOne();

            if (State == States.Idle)
            {
                State = States.Starting;
                serverThread = new Thread(MainThread);
                serverThread.Start();
            }

            coreSynchronize.Set();

            return PortResult.Accept;
        }

        public override PortResult Stop()
        {
            coreSynchronize.WaitOne();
            Close();
            coreSynchronize.Set();

            return PortResult.Accept;
        }

        public async override Task<PortResult> StartAsync()
        {
            PortResult result = PortResult.Error;

            await Task.Run(() =>
            {
                result = Start();
            });

            return result;
        }

        public async override Task<PortResult> StopAsync()
        {
            PortResult result = PortResult.Error;

            await Task.Run(() =>
            {
                result = Stop();
            });

            return result;
        }

        public override PortResult Send(byte[] data, int offset, int size)
        {
            foreach (var port in SubPorts)
            {
                port.Send(data, offset, size);
            }

            return PortResult.Error;
        }
    }
}
