using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using xLibV100.Ports;
using xLibV100.UI;

namespace xLibV100.Controls
{
    public abstract class TerminalBase : ModelBase, ITerminal
    {
        public delegate void PortAddedEventHandler(TerminalBase terminal, PortBase port);
        public delegate void PortRemovedEventHandler(TerminalBase terminal, PortBase port);

        public event PortAddedEventHandler PortAdded;
        public event PortAddedEventHandler PortRemoved;

        public class UIContainer : UINotifyPropertyChanged
        {
            protected ViewModelBase connectionsViewModel;

            public ViewModelBase ConnectionsViewModel
            {
                get => connectionsViewModel;
                set
                {
                    connectionsViewModel = value;
                    OnPropertyChanged(nameof(ConnectionsViewModel), connectionsViewModel);
                }
            }
        }

        public ObservableCollection<PortBase> AvailablePorts { get; protected set; }

        public List<TerminalObject> Devices { get; protected set; }

        public UIContainer UI { get; set; } = new UIContainer();

        public DispatcherObject UIHolder { get; set; }

        public TerminalBase()
        {
            Devices = new List<TerminalObject>();
            AvailablePorts = new ObservableCollection<PortBase>();
        }

        public virtual int AddDevice(TerminalObject device)
        {
            if (device != null && Devices != null && !Devices.Contains(device))
            {
                Devices.Add(device);

                return 0;
            }

            return -1;
        }

        public void RemoveDevice(TerminalObject device)
        {
            Devices?.Remove(device);
        }

        public void ClearDevices()
        {
            Devices?.Clear();
        }

        public virtual void AddPort(PortBase port)
        {
            if (port != null && AvailablePorts != null && !AvailablePorts.Contains(port))
            {
                AvailablePorts.Add(port);
                port.PacketReceiver += PacketReceiver;

                if (port.SubPorts != null)
                {
                    foreach (var element in port.SubPorts)
                    {
                        element.PacketReceiver += PacketReceiver;
                    }
                }

                PortAdded?.Invoke(this, port);
            }
        }

        public virtual void RemovePort(PortBase port)
        {
            if (port != null && AvailablePorts.Contains(port))
            {
                AvailablePorts?.Remove(port);
                port.Close();

                PortRemoved?.Invoke(this, port);
            }
        }

        public virtual PortBase GetPortById(long id)
        {
            if (AvailablePorts != null && AvailablePorts.Count > 0)
            {
                foreach (var port in AvailablePorts)
                {
                    if (port.Id == id)
                    {
                        return port;
                    }
                }
            }
            return null;
        }

        public void ClearPorts()
        {
            AvailablePorts.Clear();
        }

        public virtual unsafe void PacketReceiver(PortBase port, ReceivedPacketHandlerArg arg)
        {

        }

        public override void Dispose()
        {
            if (Devices != null)
            {
                foreach (ITerminalObject device in Devices)
                {
                    device?.Dispose();
                }

                Devices.Clear();
            }
        }
    }
}
