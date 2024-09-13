using System.Collections.Generic;

namespace xLibV100.Interfaces
{
    public interface IThreeElement
    {
        IList<IThreeElement> Elements { get; }
    }
}
