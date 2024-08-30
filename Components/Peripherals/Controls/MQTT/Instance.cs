namespace xLibV100.Peripherals.MqttControl
{
    public class Instance : Peripherals.Instance
    {
        static Instance()
        {
            PropertyControl = new SynchronizedPropertyControl(typeof(Instance));
        }

        public Instance(PeripheralBase model) : base(model)
        {

        }
    }
}
