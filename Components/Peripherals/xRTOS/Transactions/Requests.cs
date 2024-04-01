using System.Collections.Generic;
using xLibV100.Controls;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xRTOS.Transactions
{
    public partial class Requests : ModelBase<Control>
    {
        public ResponseHandle Handle = new ResponseHandle();
        public List<ReceiverBase> List = new List<ReceiverBase>();

        public Requests(Control control) : base(control)
        {
            Parent = control;
        }

        public unsafe bool Identification(RxPacketManager manager, xContent content)
        {
            return manager.Packet != null && manager.Packet->Header.DeviceId == Info.Address
                && Handle.Receive(manager, content) != ReceiverResult.NotFound;
        }
    }
}
