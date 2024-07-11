using xLibV100.Controls;

namespace xLibV100.Peripherals.MqttControl
{
    public class Mqtt : PeripheralBase
    {
        public Mqtt(Control model) : base(model)
        {
            Name = nameof(Mqtt);

        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
