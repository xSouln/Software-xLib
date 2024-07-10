using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xLibV100.Controls;

namespace xLibV100.Peripherals.xGPIO
{
    public class Control : TerminalObject
    {
        public Control(TerminalBase model) : base(model)
        {
            Name = nameof(xGPIO);


        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
