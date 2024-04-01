using System;
using System.Collections.Generic;
using xLibV100;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xRTOS.Transactions
{
    public partial class Responses
    {
        public List<ReceiverBase> List;
        protected Control Control { get; set; }

        public Responses(Control control)
        {
            Control = control;

            List = new List<ReceiverBase>();
        }

        public unsafe bool Identification(RxPacketManager manager, xContent content)
        {
            if (manager.Packet == null || manager.Packet->Header.DeviceId != Info.Address)
            {
                return false;
            }

            foreach (ReceiverBase response in List)
            {
                if (response.Receive(manager, content) != ReceiverResult.NotFound)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
