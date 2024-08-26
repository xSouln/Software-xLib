using xLibV100.Controls;

namespace xLibV100.Peripherals.MqttControl
{
    public class Mqtt : PeripheralBase
    {
        protected string description;
        protected string version;
        protected int countOfInstances;

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
