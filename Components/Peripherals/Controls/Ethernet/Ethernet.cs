namespace xLibV100.Peripherals.Ethernet
{
    public class Ethernet : PeripheralBase
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
