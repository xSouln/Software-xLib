using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using xLibV100.Controls;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals
{
    public class PeripheralBase : ModelBase<PeripheralControl>, IPeripheral
    {
        protected IEnumerable instances;

        public IEnumerable<IInstance> Instances
        {
            get => instances as IEnumerable<IInstance>;
            set => instances = value;
        }

        public PeripheralBase(PeripheralControl model) : base(model)
        {

        }

        public virtual unsafe bool ResponseIdentification(RxPacketManager manager, xContent content)
        {
            return false;
        }
    }

    public class PeripheralBase<TInstance> : PeripheralBase where TInstance : Instance
    {
        public new ObservableCollection<Instance> Instances
        {
            get => instances as ObservableCollection<Instance>;
            protected set => instances = value;
        }

        public PeripheralBase(PeripheralControl model) : base(model)
        {
            Instances = new ObservableCollection<Instance>();
        }
    }
}
