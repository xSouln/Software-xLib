using System;
using xLibV100.Ports;

namespace xLibV100.Controls
{
    public interface ITerminal : IDisposable
    {
        string Name { get; set; }
        int AddDevice(TerminalObject device);
        void RemoveDevice(TerminalObject device);
        void ClearDevices();
        void AddPort(PortBase port);
        void RemovePort(PortBase port);
        void ClearPorts();
    }
}
