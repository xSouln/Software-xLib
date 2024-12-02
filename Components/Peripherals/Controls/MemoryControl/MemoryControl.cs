using System.Collections.ObjectModel;
using xLibV100.Controls;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.MemoryControl
{
    public class MemoryControl : PeripheralBase
    {
        protected string signature = "MemoryControl-Common";
        protected ObservableCollection<Instance> elements = new ObservableCollection<Instance>();
        protected Transactions.Control transactionsControl = new Transactions.Control();

        public MemoryControl(PeripheralControl model) : base(model)
        {
            Name = "Memory control";

            Instances = elements;

            Services.RegisterByType(transactionsControl);

            elements.Add(new Instance(this));
        }


        public override bool ResponseIdentification(RxPacketManager manager, xContent content)
        {
            return transactionsControl.Identification(manager, content);
        }


        [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
        public string Signature
        {
            get => signature;
            protected set
            {
                if (value != signature)
                {
                    signature = value;
                    OnPropertyChanged(nameof(Signature), signature);
                }
            }
        }
    }
}
