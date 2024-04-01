using System;
using xLibV100.Controls;

namespace xLibV100.Ports
{
    public interface IPort : IDisposable
    {
        PortResult Send(string data);
        PortResult Send(byte[] data);
        void ClearRxBuffer();
        PortBase AddListener(TerminalObject device);
        PortBase RemoveListener(TerminalObject device);
        void ClearListeners();
    }
}
