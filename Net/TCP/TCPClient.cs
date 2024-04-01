using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using xLibV100.Common;
using xLibV100.Components;
using xLibV100.Ports;

namespace xLibV100.Net
{
    public class TCPClient : PortBase
    {
        public xAction<string> Tracer;
        protected TcpClient client;
        protected NetworkStream stream;
        protected Thread clientThread;

        protected string address;

        protected string ip;
        protected int port;

        public int UpdatePeriod = 1;
        protected int transmit_deadtime = 1;

        protected List<byte[]> transmit_line = new List<byte[]>();

        public RxReceiver Receiver = new RxReceiver(0x3fff, new byte[] { (byte)'\r' });

        private void Trace(string note)
        {
            Tracer?.Invoke(note);
            xTracer.Message(note);
        }

        public TCPClient() : base()
        {
            Receiver.PacketReceiver += RxPacketReceiver;

            TransferLayer = TransferLayers.Tcp;
            Role = Roles.Client;
            Type = typeof(TCPClient).ToString();
        }

        public TCPClient(PortBase port) : this()
        {
            Apply(port);
        }

        public TCPClient(TcpClient client) : this()
        {
            this.client = client;

            State = States.Connecting;

            Trace("tcp client: client begin connect");

            try
            {
                client.Connect(Ip, Port);
                clientThread = new Thread(new ThreadStart(RxThreadFunction));
                clientThread.Start();
            }
            catch (Exception ex)
            {
                Trace(ex.ToString());
                ClientClose();
            }
        }

        private void RxPacketReceiver(RxReceiver rx, ReceivedPacketHandlerArg arg)
        {
            iReceive(arg);
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

        [PortProperty(Name = nameof(Ip), Key = "Options")]
        public string Ip
        {
            get => ip;
            set
            {
                try
                {
                    IPAddress.Parse(value);
                    if (!xMemory.Compare(ip, value))
                    {
                        ip = value;
                        OnPropertyChanged(nameof(Ip));
                    }
                }
                catch
                {
                    OnPropertyChanged(nameof(Ip));
                }
            }
        }

        public override object Options
        {
            get => new TCPClientOptions
            {
                Ip = Ip,
                Port = Port
            };
            set
            {
                if (value is TCPClientOptions options)
                {
                    Ip = options.Ip;
                    Port = options.Port;
                }
            }
        }

        private void RxThreadFunction()
        {
            if (client == null)
            {
                Trace("tcp client: client == null");
                ClientClose();
            }
            try
            {
                client.ReceiveBufferSize = 0x10000;
                stream = client.GetStream();
                stream.Flush();
                State = States.Connected;
                Trace("tcp client: thread start");
                client.ReceiveBufferSize = 1000000;

                int count = 0;
                byte[] buf = new byte[1000000];
                Receiver.Clear();

                while (true)
                {
                    do
                    {
                        count = stream.Read(buf, 0, buf.Length);

                        if (count == 0)
                        {
                            Trace("tcp client: closing");
                            ClientClose();
                        }

                        for (int i = 0; i < count; i++)
                        {
                            Receiver.Add(buf[i]);
                        }
                    }
                    while ((bool)stream?.DataAvailable);
                }
            }
            catch (Exception e)
            {
                Trace(e.ToString());
                ClientClose();
            }
        }

        private void ClientClose()
        {
            try
            {
                clientThread?.Abort();

                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                    stream = null;
                }

                if (client != null)
                {
                    client.Client?.Close();
                    client.Close();
                    client = null;
                }
            }
            finally
            {
                Trace("tcp client: thread close");
                transmit_line.Clear();
                clientThread = null;
                State = States.Idle;
            }
        }

        private void request_callback(IAsyncResult ar)
        {
            try
            {
                TcpClient result = (TcpClient)ar.AsyncState;
                if (result != null && result.Client != null)
                {
                    Trace("tcp: client connected");
                    client = result;
                    clientThread = new Thread(new ThreadStart(RxThreadFunction));
                    clientThread.Start();
                }
                else
                {
                    Trace("tcp client: client connect error");
                }
            }
            catch (Exception ex)
            {
                Trace(ex.ToString());
                Trace("tcp client: client connect abort");
                ClientClose();
                return;
            }
        }

        public override PortResult Connect()
        {
            if (State != States.Idle)
            {
                Trace("tcp client: busy");
                return PortResult.Error;
            }

            if (Ip == null || Port == 0)
            {
                Trace("tcp client: address error");
                return PortResult.Error;
            }

            client = new TcpClient();

            State = States.Connecting;

            Trace("tcp client: client begin connect");
            IAsyncResult result = client.BeginConnect(Ip, Port, request_callback, client);

            return PortResult.Accept;
        }

        public PortResult Connect(TcpClient client)
        {
            if (client == null || client.Client == null)
            {
                return PortResult.Error;
            }

            State = States.Connecting;

            try
            {
                this.client = client;
                IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;

                Ip = endPoint.Address.ToString();
                Port = endPoint.Port;

                clientThread = new Thread(new ThreadStart(RxThreadFunction));
                clientThread.Start();

                return PortResult.Accept;
            }
            catch(Exception ex)
            {
                Trace(ex.ToString());
                ClientClose();
            }

            return PortResult.Accept;
        }

        public override Task<PortResult> ConnectAsync()
        {
            return Task.FromResult(Connect());
        }

        public override PortResult Disconnect()
        {
            Trace("tcp client: request disconnect");
            ClientClose();

            return PortResult.Accept;
        }

        public override Task<PortResult> DisconnectAsync()
        {
            return Task.FromResult(Disconnect());
        }

        public override void Dispose()
        {
            Disconnect();

            base.Dispose();
        }

        public override PortResult Send(byte[] data, int offset, int size)
        {
            if (data == null || !(data.Length > 0) || (offset + size) > data.Length)
            {
                return PortResult.DataError;
            }

            if (client == null || stream == null)
            {
                return PortResult.InternalError;
            }

            if (!client.Connected)
            {
                return PortResult.ConnectionError;
            }

            try
            {
                stream.Write(data, offset, size);
                return PortResult.Accept;
            }

            catch
            {
                Trace("tcp client: невозможно отправить на указаный ip");
            }

            return PortResult.Error;
        }

        public override void ClearRxBuffer()
        {
            Receiver.Clear();
        }
    }
}
