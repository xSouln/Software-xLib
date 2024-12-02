using xLibV100.Controls;

namespace xLibV100.Peripherals
{
    public class Usart : PeripheralBase
    {
        public Usart(PeripheralControl model) : base(model)
        {
            Name = nameof(Usart);


        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
