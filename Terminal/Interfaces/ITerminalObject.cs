using System;
using xLibV100.Transceiver;

namespace xLibV100.Controls
{
    public interface ITerminalObject : IDisposable
    {
        string Name { get; set; }
        TerminalBase Terminal { get; set; }
        bool ResponseIdentification(RxPacketManager manager, xContent content);
    }
}
