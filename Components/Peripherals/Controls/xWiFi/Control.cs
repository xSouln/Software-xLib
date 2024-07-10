using xLibV100.Serializers;
using xLibV100.Peripherals.xWiFi.Transactions;
using xLibV100.Peripherals.xWiFi.Types;
using System.Threading.Tasks;
using xLibV100.Components;
using xLibV100.Controls;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xWiFi
{
    public class Control : TerminalObject
    {
        protected Requests Requests;
        protected Responses Responses;

        public Models.Config Config;
        public Models.Status Status;

        public Control(TerminalBase control) : base(control)
        {
            Name = nameof(xWiFi);

            Requests = new Requests(this);
            Responses = new Responses(this);

            Config = new Models.Config(this);
            Status = new Models.Status(this);

            PortSubscriptions.Open(this, "Components\\xWiFi\\Saves");
        }

        public async Task<Get.Response<ConfigT>> GetConfig(Module mask)
        {
            var transaction = Get.Config.Prepare(new Get.RequestHeaderT { Mask = mask });
            xTracer.Trace(await transaction.TransmitAsync(SelectedPort, 1, 300), Name);

            return transaction.Response;
        }

        public async Task<Set.Response> SetConfig(Module mask, ConfigT request)
        {
            var transaction = Set.Config.Prepare(new Set.Request<ConfigT> { Mask = mask, Value = request });
            xTracer.Trace(await transaction.TransmitAsync(SelectedPort, 1, 300), Name);

            return transaction.Response;
        }

        public async Task<Try.Response> Enable(Module mask)
        {
            var transaction = Try.Enable.Prepare(new Try.Request { Mask = mask });
            xTracer.Trace(await transaction.TransmitAsync(SelectedPort, 1, 300), Name);

            return transaction.Response;
        }

        public async Task<Try.Response> Disable(Module mask)
        {
            var transaction = Try.Disable.Prepare(new Try.Request { Mask = mask });
            xTracer.Trace(await transaction.TransmitAsync(SelectedPort, 1, 300), Name);

            return transaction.Response;
        }

        public async Task<Get.Response<AddressT>> GetAddress(Module mask)
        {
            var transaction = Get.Address.Prepare(new Get.RequestHeaderT { Mask = mask });
            xTracer.Trace(await transaction.TransmitAsync(SelectedPort, 1, 300), Name);

            return transaction.Response;
        }

        public override bool ResponseIdentification(RxPacketManager manager, xContent content)
        {
            return Requests.Identification(manager, content) || Responses.Identification(manager, content);
        }

        protected override async Task StateUpdateHandler(object arg)
        {
            xTracer.Trace(await Get.Status.Prepare(new Get.RequestHeaderT { Mask = Module.All }).TransmitAsync(SelectedPort, 1, 300), Name);
        }

        public override void Dispose()
        {
            base.Dispose();

            Config?.Dispose();

            PortSubscriptions.Save(this, "Components\\xWiFi\\Saves");
        }
    }
}
