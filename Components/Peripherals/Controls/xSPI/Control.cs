using xLibV100.Controls;

namespace xLibV100.Peripherals.xSPI
{
    public class Control : TerminalObject
    {
        public Control(TerminalBase model) : base(model)
        {
            Name = nameof(xADC);


        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
