﻿using xLibV100.Controls;

namespace xLibV100.Peripherals
{
    public class Instance : ModelBase, IInstance
    {
        protected static SynchronizedPropertyControl PropertyControl;

        public Instance(object parent) : base(parent)
        {

        }
    }

    public class Instance<TPeripheral> : Instance where TPeripheral : class
    {
        public Instance(PeripheralBase model) : base(model)
        {

        }

        public new TPeripheral Parent
        {
            get => parent as TPeripheral;
            set => parent = value;
        }
    }
}
