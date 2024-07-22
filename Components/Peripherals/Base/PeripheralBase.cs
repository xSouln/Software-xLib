using System.Collections.ObjectModel;
using xLibV100.Controls;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals
{
    public class PeripheralBase : ModelBase<Control>
    {
        protected object instances;

        public object GetInstances => instances;

        public PeripheralBase(Control model) : base(model)
        {

        }

        public virtual unsafe bool ResponseIdentification(RxPacketManager manager, xContent content)
        {
            return false;
        }
    }

    public class PeripheralBase<TInstance> : PeripheralBase where TInstance : Instance
    {
        public ObservableCollection<Instance> Instances
        {
            get => instances as ObservableCollection<Instance>;
            protected set => instances = value;
        }

        public PeripheralBase(Control model) : base(model)
        {
            instances = new ObservableCollection<Instance>();
        }
    }
}
