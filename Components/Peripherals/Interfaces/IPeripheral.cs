using System.Collections.Generic;

namespace xLibV100.Peripherals
{
    public interface IPeripheral
    {
        IEnumerable<IInstance> Instances { get; }
    }
}
