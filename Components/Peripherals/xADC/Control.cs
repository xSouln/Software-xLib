using xLibV100.Serializers;
using xLibV100.Peripherals.xADC.Models;
using xLibV100.Peripherals.xADC.Transactions;
using System.Threading.Tasks;
using xLibV100.Components;
using xLibV100.Controls;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xADC
{
    public class Control : TerminalObject
    {
        protected Requests Requests;
        protected Responses Responses;

        public Charts Charts;

        public Control(TerminalBase control) : base(control)
        {
            Name = nameof(xADC);

            Requests = new Requests(this);
            Responses = new Responses(this);

            Charts = new Charts(this);

            //ViewModel = new ControlViewModel(this);

            PortSubscriptions.Open(this, "Components\\xADC\\Saves");
        }

        public async Task<Set.ResponseResult> SetNitifiedChannels(byte ADCs, ushort channels)
        {
            RequestSetNotifiedChannels request = new RequestSetNotifiedChannels
            {
                ADCs = ADCs,
                Channels = channels
            };

            var transaction = Set.NotifiedChannels.Prepare(request);
            await transaction.TransmitAsync(SelectedPort, 1, 300);

            xTracer.Trace(transaction, Name);

            return transaction.Response;
        }

        public override bool ResponseIdentification(RxPacketManager manager, xContent content)
        {
            return Requests.Identification(manager, content) || Responses.Identification(manager, content);
        }

        public override void Dispose()
        {
            base.Dispose();

            Charts.Dispose();

            PortSubscriptions.Save(this, "Components\\xADC\\Saves");
        }
    }
}
