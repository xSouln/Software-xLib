using xLibV100.Common;

namespace xLibV100.Adaptation
{
    public interface IPropertyConverter
    {
        object Convert(xMemoryReader memoryReader);

        byte[] ToBinary(object property);
    }
}
