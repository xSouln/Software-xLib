using xLibV100.Controls;

namespace xLibV100.Peripherals
{
    public class RTOS : PeripheralBase
    {
        public RTOS(Control model) : base(model)
        {
            Name = nameof(RTOS);


        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
