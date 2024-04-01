using xLibV100.Peripherals.xWiFi.Transactions;
using System;
using System.Collections.Generic;
using xLibV100;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xWiFi.Transactions
{
    public partial class Responses
    {
        public List<ReceiverBase> List;
        public Control Parent { get; set; }

        public Responses(Control parent)
        {
            Parent = parent;

            List = new List<ReceiverBase>();

            List.Add(Events.StatusChanged);
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
