using xLibV100.Common;
using xLibV100.Ports;

namespace xLibV100.Transceiver
{
    public class RxPacketManager
    {
        public object Control;

        public PortBase Port;

        public ReceivedPacketHandlerArg ReceivedPacket;

        public unsafe PacketT* Packet;

        public IDataBuffer Response;

        public object Context;

        public object FoundObject;
    }
}
