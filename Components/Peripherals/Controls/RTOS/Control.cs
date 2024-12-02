using xLibV100.Controls;

namespace xLibV100.Peripherals.RTOSControl
{
    public class RTOS : PeripheralBase
    {
        public RTOS(PeripheralControl model) : base(model)
        {
            Name = nameof(RTOS);


        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
