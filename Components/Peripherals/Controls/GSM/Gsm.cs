using System.Collections.ObjectModel;
using System.Threading.Tasks;
using xLibV100.Controls;
using xLibV100.Transactions.Common;

namespace xLibV100.Peripherals.GSM
{
    public partial class Gsm : PeripheralBase
    {
        protected string description;
        protected string version;
        protected int countOfInstances;

        public ObservableCollection<Instance> Instances { get; set; } = new ObservableCollection<Instance>();

        public Gsm(Control model) : base(model)
        {
            Name = nameof(Gsm);
        }

        public virtual Task<ActionResult> GetInfoAsync()
        {
            return Task.FromResult(ActionResult.NotSupported);
        }


        public virtual Task<ActionResult> GetInstancesAsync()
        {
            return Task.FromResult(ActionResult.NotSupported);
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
