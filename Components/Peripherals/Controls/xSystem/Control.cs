using xLibV100.Controls;

namespace xLibV100.Peripherals.xSystem
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
