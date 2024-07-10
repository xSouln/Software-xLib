using xLibV100.Controls;

namespace xLibV100.Peripherals
{
    public class Usart : PeripheralBase
    {
        public Usart(Control model) : base(model)
        {
            Name = nameof(Usart);


        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
