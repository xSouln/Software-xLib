using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using xLibV100.Controls;
using xLibV100.UI;

namespace xLibV100.Ports
{
    public class ReceivedPacketHandlerArg
    {
        public byte[] Data;
        public unsafe byte* DataPtr;

        //the size of the received data
        public int DataSize;

        //size without end characters
        public int PacketSize;

        //additional content if necessary
        public object Content;
    }

    public class ConnectionStateChangedEventHandlerArg
    {
        public States State;
    }

    public delegate void ReceivedPacketHandler(PortBase port, ReceivedPacketHandlerArg arg);
    public delegate void ConnectionStateChangedEventHandler(PortBase port, ConnectionStateChangedEventHandlerArg arg);
    public delegate void BridgeAddedEventHandler(PortBase port, PortBase bridgePort);
    public delegate void BridgeRemovedEventHandler(PortBase port, PortBase bridgePort);
    public delegate void ClosedEventHandler(PortBase port);
    public delegate void SubPortAddedEventHandler(PortBase port, PortBase subPort);
    public delegate void SubPortRemovedEventHandler(PortBase port, PortBase subPort);

    public enum States
    {
        Idle,

        Connecting,
        Connected,
        Disconnecting,

        Starting,
        Started,
        Stopping
    }

    public enum Roles
    {
        NotSet,

        Client,
        Server,
        Universal
    }

    public enum TransferLayers
    {
        NotSet,

        SerialPort,

        Tcp,
        Mqtt,

        Http,
        Https
    }

    public class PortTxRequest
    {
        public byte[] Data;
        public unsafe byte* DataPtr;
        public int Size;
        public int Offset;
    }

    public class PortRxRequest
    {
        public byte[] Data;
        public int Size;
    }

    public abstract class PortBase : UINotifyPropertyChanged, IPort
    {
        public event ReceivedPacketHandler PacketReceiver;
        public event ConnectionStateChangedEventHandler ConnectionStateChanged;
        public event BridgeAddedEventHandler BridgeAdded;
        public event BridgeRemovedEventHandler BridgeRemoved;
        public event ClosedEventHandler Closed;
        public event SubPortAddedEventHandler SubPortAdded;
        public event SubPortRemovedEventHandler SubPortRemoved;

        protected States state;
        protected TransferLayers transferLayer;
        protected Roles type;
        protected object options;
        protected object content;
        protected string name;
        protected PortBase parent;
        protected ObservableCollection<PortBase> bridges = new ObservableCollection<PortBase>();

        [JsonIgnore]
        public ObservableCollection<TerminalObject> Listeners { get; protected set; } = new ObservableCollection<TerminalObject>();

        [JsonIgnore]
        public PortBase Parent
        {
            get => parent;
            protected set => parent = value;
        }

        [JsonIgnore]
        public ObservableCollection<PortBase> SubPorts { get; set; } = new ObservableCollection<PortBase>();


        [JsonIgnore]
        public object Content
        {
            get => content;
            set => content = value;
        }

        [JsonIgnore]
        public ObservableCollection<PortBase> Bridges
        {
            get => bridges;
        }

        public PortBase()
        {
            Id = new Random().Next();
        }

        public PortBase(int id) : this()
        {
            Id = id;
        }

        protected PortResult Apply(PortBase port)
        {
            Name = port.Name;
            Options = port.Options;

            Type = port.Type;
            TransferLayer = port.TransferLayer;
            Role = port.Role;

            Id = port.Id;

            return PortResult.Accept;
        }

        public virtual object Options
        {
            get => options;
            set => options = value;
        }


        [PortProperty(Name = nameof(Name), Key = "Common info")]
        public virtual string Name
        {
            get => name;
            set => name = value;
        }

        [PortProperty(Name = nameof(Type), Key = "Common info")]
        public string Type { get; set; }


        [PortProperty(Name = nameof(TransferLayer), Key = "Common info")]
        public TransferLayers TransferLayer
        {
            get => transferLayer;
            set
            {
                if (transferLayer != value)
                {
                    transferLayer = value;
                    OnPropertyChanged(nameof(TransferLayer));
                }
            }
        }


        [PortProperty(Name = nameof(Role), Key = "Common info")]
        public Roles Role
        {
            get => type;
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged(nameof(Role));
                }
            }
        }


        [PortProperty(Name = nameof(Id), Key = "Common info")]
        public long Id { get; set; }


        [PortProperty(Name = nameof(State), Key = "Common info")]
        public States State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    state = value;

                    ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventHandlerArg { State = state });
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        [PortProperty(Name = nameof(BridgeIsEnabled), Key = "Common info")]
        public bool BridgeIsEnabled
        {
            get => Bridges.Count > 0;
        }

        public virtual PortBase AddListener(TerminalObject device)
        {
            if (device != null && Listeners != null && !Listeners.Contains(device))
            {
                Listeners.Add(device);
                return this;
            }

            return null;
        }

        protected virtual PortResult SetParent(PortBase port)
        {
            parent = port;
            return PortResult.Accept;
        }

        public virtual PortBase RemoveListener(TerminalObject device)
        {
            Listeners.Remove(device);
            return this;
        }

        public void ClearListeners()
        {
            Listeners?.Clear();
        }

        public virtual PortResult AddSubPort(PortBase port)
        {
            if (port != null && port.Parent == null && !SubPorts.Contains(port))
            {
                if (port.SetParent(this) == PortResult.Accept)
                {
                    SubPorts.Add(port);
                    SubPortAdded?.Invoke(this, port);
                    return PortResult.Accept;
                }
            }
            return PortResult.Error;
        }

        public virtual void RemoveSubPort(PortBase port)
        {
            if (port.Parent == this && SubPorts.Contains(port))
            {
                port.SetParent(null);
                SubPorts.Remove(port);
                SubPortRemoved?.Invoke(this, port);
            }
        }

        public void ClearSubPorts()
        {
            SubPorts.Clear();
        }

        public virtual void ClearRxBuffer()
        {

        }

        public virtual PortResult Send(byte[] data, int offset, int size)
        {
            return PortResult.NotSupported;
        }

        public virtual PortResult Receive(object sender, object context, byte[] data, int dataLength, int dataOffset)
        {
            return PortResult.NotSupported;
        }

        public PortResult Send(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                return Send(data, 0, data.Length);
            }

            return PortResult.Error;
        }

        public PortResult Send(string data)
        {
            if (data != null && data.Length > 0)
            {
                return Send(Encoding.ASCII.GetBytes(data), 0, data.Length);
            }

            return PortResult.Error;
        }

        public virtual PortResult Send(object request)
        {
            return PortResult.NotSupported;
        }

        public virtual Task<PortResult> SendAsync(PortTxRequest request)
        {
            return Task.FromResult(PortResult.NotSupported);
        }

        public virtual PortResult AddBridge(PortBase port)
        {
            if (port == null)
            {
                return PortResult.Error;
            }

            if (!bridges.Contains(port))
            {
                bridges.Add(port);
                OnPropertyChanged(nameof(BridgeIsEnabled), BridgeIsEnabled);

                BridgeAdded?.Invoke(this, port);
            }

            return PortResult.Accept;
        }

        public virtual PortResult RemoveBridge(PortBase port)
        {
            if (port == null)
            {
                return PortResult.Error;
            }

            if (bridges.Contains(port))
            {
                bridges.Remove(port);
                OnPropertyChanged(nameof(BridgeIsEnabled), BridgeIsEnabled);

                BridgeRemoved?.Invoke(this, port);
            }

            return PortResult.Accept;
        }

        public void ToBridgePorts(ReceivedPacketHandlerArg arg)
        {
            if (bridges.Count > 0)
            {
                foreach (var port in bridges)
                {
                    port.Send(arg.Data, 0, arg.DataSize);
                }
            }
        }

        protected virtual void iReceive(ReceivedPacketHandlerArg arg)
        {
            PacketReceiver?.Invoke(this, arg);
        }

        protected void OnReceive(PortBase port, ReceivedPacketHandlerArg arg)
        {
            PacketReceiver?.Invoke(port, arg);
        }

        protected virtual void iReceive(object content, byte[] data, int size)
        {
            unsafe
            {
                fixed (byte* ptr = data)
                {
                    var arg = new ReceivedPacketHandlerArg
                    {
                        Content = content,
                        PacketSize = size,
                        DataPtr = ptr,
                        Data = data
                    };

                    iReceive(arg);
                }
            }
        }

        protected virtual void OnConnectionChanged()
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventHandlerArg
            {
                State = State
            });
        }

        public virtual PortResult Connect()
        {
            return PortResult.NotSupported;
        }

        public virtual Task<PortResult> ConnectAsync()
        {
            return Task.FromResult(PortResult.NotSupported);
        }

        public virtual PortResult Disconnect()
        {
            return PortResult.Accept;
        }

        public virtual Task<PortResult> DisconnectAsync()
        {
            return Task.FromResult(PortResult.NotSupported);
        }

        public virtual PortResult Start()
        {
            return PortResult.NotSupported;
        }

        public virtual Task<PortResult> StartAsync()
        {
            return Task.FromResult(PortResult.NotSupported);
        }

        public virtual PortResult Stop()
        {
            return PortResult.NotSupported;
        }

        public virtual Task<PortResult> StopAsync()
        {
            return Task.FromResult(PortResult.NotSupported);
        }

        public virtual void Close()
        {
            Stop();
            Disconnect();
            Closed?.Invoke(this);
        }

        public override void Dispose()
        {
            base.Dispose();

            Close();
        }
    }
}
