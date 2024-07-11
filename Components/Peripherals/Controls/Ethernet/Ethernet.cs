namespace xLibV100.Peripherals.EthernetControl
{
    public class Ethernet : PeripheralBase<Instance>
    {
        protected bool dhcpIsEnabled;
        protected bool sntpIsEnabled;

        public Ethernet(Control model) : base(model)
        {
            Name = nameof(Ethernet);


        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
