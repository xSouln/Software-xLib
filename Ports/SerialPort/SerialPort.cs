using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xLibV100.Common;
using xLibV100.Components;

namespace xLibV100.Ports
{
    public partial class SerialPort : PortBase
    {
        protected Thread RxThread;
        protected static CancellationTokenSource TaskUpdatePortsCancellation;
        protected static string lastConnectedPortName = "";
        protected System.IO.Ports.SerialPort port;
        protected bool is_connected;
        protected int boad_rate = 115200;
        protected string port_name = "";
        protected string last_selected_port_name = "";
        protected AutoResetEvent TxSynchronizer = new AutoResetEvent(true);
        protected RxReceiver Receiver = new RxReceiver(0x1fff, new byte[] { (byte)'\r' });
        public static List<int> BaudRateList { get; set; } = new List<int>() { 9600, 38400, 115200, 128000, 256000, 521600, 840000, 900000, 921600 };

        public static ObservableCollection<string> PortList { get; set; } = new ObservableCollection<string>();

        public SerialPort() : base()
        {
            Receiver.PacketReceiver += RxPacketReceiver;
            Role = Roles.Universal;
            TransferLayer = TransferLayers.SerialPort;
            Type = typeof(SerialPort).ToString();
        }

        public SerialPort(PortBase port) : this()
        {
            Name = port.Name;
            Options = port.Options;

            Type = port.Type;
            TransferLayer = port.TransferLayer;
            Role = port.Role;

            Id = port.Id;
        }

        private void RxPacketReceiver(RxReceiver rx, ReceivedPacketHandlerArg arg)
        {
            iReceive(arg);
        }

        public static async void CoreStart()
        {
            if (TaskUpdatePortsCancellation != null)
            {
                return;
            }

            TaskUpdatePortsCancellation = new CancellationTokenSource();

            try
            {
                await Task.Run(async () =>
                {
                    await Task.Delay(1000, TaskUpdatePortsCancellation.Token);
                    while (true)
                    {
                        FindePorts();
                        await Task.Delay(1000, TaskUpdatePortsCancellation.Token);
                    }

                }, TaskUpdatePortsCancellation.Token);
            }
            catch
            {

            }
        }

        public static void CoreStop()
        {
            TaskUpdatePortsCancellation?.Cancel();
            TaskUpdatePortsCancellation = null;
        }

        public bool SelectIsEnable => !IsConnected;

        public override object Options
        {
            get => new SerialPortOptions()
            {
                BoadRate = BaudRate
            };
            set
            {
                if (value is SerialPortOptions options)
                {
                    BaudRate = options.BoadRate;
                }
            }
        }

        public bool IsConnected
        {
            get => is_connected;
            set
            {
                if (is_connected != value)
                {
                    is_connected = value;
                    OnPropertyChanged(nameof(IsConnected));
                    OnPropertyChanged(nameof(SelectIsEnable));

                    State = is_connected ? States.Connected : States.Idle;
                }
            }
        }


        [PortProperty(Name = nameof(BaudRate), Key = "Options")]
        public int BaudRate
        {
            get => boad_rate;
            set
            {
                if (boad_rate != value)
                {
                    if (port != null)
                    {
                        port.BaudRate = value;
                    }
                    boad_rate = value;
                    xTracer.Message("" + Name + "(boad rate changed at " + boad_rate + ")");
                    OnPropertyChanged(nameof(BaudRate));
                }
            }
        }

        private void RxThreadHandler()
        {
            try
            {
                while (port != null && port.IsOpen)
                {
                    while (port.BytesToRead > 0)
                    {
                        byte[] data = new byte[port.BytesToRead];
                        port.Read(data, 0, data.Length);

                        //Receiver.Add((byte)Port.ReadByte());
                        Receiver.Add(data);
                    }
                    Thread.Sleep(1);
                }

                xTracer.Message(Name + "(boadrate: " + BaudRate + "): error read data");
                Disconnect();
            }
            catch (Exception ex)
            {
                xTracer.Message(ex.ToString());
                Disconnect();
            }
        }

        public bool Connect(string name)
        {
            if (name == null || name.Length < 3)
            {
                return false;
            }

            if (port != null && port.IsOpen)
            {
                return false;
            }

            try
            {
                if (Receiver == null)
                {
                    Receiver = new RxReceiver(0x7fff + 1, new byte[] { (byte)'\r', (byte)'\n' });
                }

                port = new System.IO.Ports.SerialPort(name, BaudRate, Parity.None, 8, StopBits.One);
                port.Encoding = Encoding.GetEncoding("iso-8859-1");
                port.ReadBufferSize = 0x7fff + 1;
                port.WriteBufferSize = 0x7fff + 1;

                State = States.Connecting;
                port.Open();

                Name = name;
                Receiver.Clear();
                xTracer.Message(name + "(boadrate: " + BaudRate + "): rx thred started");

                IsConnected = true;
                RxThread = new Thread(RxThreadHandler);
                RxThread.Start();
                xTracer.Message(name + "(boadrate: " + BaudRate + "): connected");
            }
            catch (Exception ex)
            {
                xTracer.Message(name + "(boadrate: " + BaudRate + "): error connect " + ex);
                Disconnect();
            }

            return IsConnected;
        }

        public override PortResult Connect()
        {
            return Connect(Name) ? PortResult.Accept : PortResult.Error;
        }

        public override Task<PortResult> ConnectAsync()
        {
            return Task.FromResult(Connect());
        }

        public override PortResult Disconnect()
        {
            RxThread?.Abort();
            RxThread = null;

            port?.Close();
            port = null;

            IsConnected = false;
            State = States.Idle;
            xTracer.Message(Name + "(boadrate: " + BaudRate + "): disconnected");

            return PortResult.Accept;
        }

        public override Task<PortResult> DisconnectAsync()
        {
            return Task.FromResult(Disconnect());
        }

        public override void Dispose()
        {
            Disconnect();
        }

        private static void UpdatePortList(ObservableCollection<string> ports, List<string> remove, List<string> add)
        {
            foreach (string name in remove)
            {
                int i = 0;
                while (i < ports.Count)
                {
                    if (xConverter.Compare(name, ports[i]))
                    {
                        ports.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            foreach (string name in add)
            {
                ports.Add(name);
            }
        }

        private static void FindePorts()
        {
            List<string> Ports = System.IO.Ports.SerialPort.GetPortNames().ToList<string>();
            List<string> TotalPorts = new List<string>();
            int count = TotalPorts.Count;
            
            foreach (string name in PortList)
            {
                TotalPorts.Add(name);
            }
            
            int i = 0;
            while (i < TotalPorts.Count && Ports.Count > 0)
            {
                int j = 0;
                while (j < Ports.Count)
                {
                    if (xConverter.Compare(TotalPorts[i], Ports[j]))
                    {
                        TotalPorts.RemoveAt(i);
                        Ports.RemoveAt(j);
                        goto end_while;
                    }
                    j++;
                }
                i++;
            end_while:;
            }

            if (TotalPorts.Count != Ports.Count || count != TotalPorts.Count)
            {
                xSupport.ActionThreadUI(() =>
                {
                    UpdatePortList(PortList, TotalPorts, Ports);
                });
            }
        }

        public override PortResult Send(byte[] data, int offset, int size)
        {
            if (data == null || !(data.Length > 0) || (offset + size) > data.Length)
            {
                return PortResult.DataError;
            }

            if (port == null)
            {
                return PortResult.InternalError;
            }

            if (!port.IsOpen)
            {
                return PortResult.ConnectionError;
            }

            try
            {
                port.Write(data, offset, size);
                return PortResult.Accept;
            }
            catch
            {

            }

            return PortResult.Error;
        }

        public override void ClearRxBuffer()
        {
            Receiver.Clear();
        }
    }
}
