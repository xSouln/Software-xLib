using xLibV100.Controls;

namespace xLibV100.Peripherals.MQTT
{
    public class MQTT : PeripheralBase
    {
        public MQTT(Control model) : base(model)
        {
            Name = nameof(MQTT);

        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
