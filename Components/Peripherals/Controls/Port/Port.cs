namespace xLibV100.Peripherals.PortControl
{
    public class Port : PeripheralBase<Instance>
    {
        public Transactions.Control Transactions;


        public Port(Control model) : base(model)
        {
            Name = nameof(Port);

            Transactions = new Transactions.Control(this, Info.UID);
        }
    }
}
