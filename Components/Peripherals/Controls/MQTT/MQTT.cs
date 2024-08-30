namespace xLibV100.Peripherals.MqttControl
{
    public class Mqtt : PeripheralBase
    {
        protected string description;
        protected string version;
        protected int countOfInstances;

        public Transactions.Control Transactions;

        public Mqtt(Control model) : base(model)
        {
            Name = nameof(Mqtt);

            Transactions = new Transactions.Control(this, Info.UID);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
