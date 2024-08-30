using System.Threading.Tasks;
using xLibV100.Controls;
using xLibV100.Transactions.Common;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.GsmControl
{
    public partial class Gsm : PeripheralBase<Instance>
    {
        protected string description;
        protected string version;
        protected int countOfInstances;

        public Transactions.Control Transactions;

        public Gsm(Control model) : base(model)
        {
            Name = nameof(Gsm);

            Transactions = new Transactions.Control(this, Info.UID);

            Instances.Add(new Instance(this));
        }

        public virtual Task<ActionResult> GetInfoAsync()
        {
            return Task.FromResult(ActionResult.NotSupported);
        }


        public virtual Task<ActionResult> GetInstancesAsync()
        {
            return Task.FromResult(ActionResult.NotSupported);
        }

        public override bool ResponseIdentification(RxPacketManager manager, xContent content)
        {
            return Transactions.Identification(manager, content);
        }


        [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
        public string Description
        {
            get => description;
            protected set
            {
                if (value != description)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description), description);
                }
            }
        }


        [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
        public int CountOfInstances
        {
            get => countOfInstances;
            protected set
            {
                if (value != countOfInstances)
                {
                    countOfInstances = value;
                    OnPropertyChanged(nameof(CountOfInstances), countOfInstances);
                }
            }
        }
    }
}
