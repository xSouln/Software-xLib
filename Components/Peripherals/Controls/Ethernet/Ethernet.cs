namespace xLibV100.Peripherals.EthernetControl
{
    public class Ethernet : PeripheralBase<Instance>
    {
        protected bool dhcpIsEnabled;
        protected bool sntpIsEnabled;

        public Transactions.Control Transactions;


        public Ethernet(Control model) : base(model)
        {
            Name = nameof(Ethernet);

            Transactions = new Transactions.Control(this, Info.UID);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
