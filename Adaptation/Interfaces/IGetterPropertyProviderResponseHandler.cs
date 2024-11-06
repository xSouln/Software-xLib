using System.Collections.Generic;

namespace xLibV100.Adaptation
{
    public interface IGetterPropertyProviderResponseHandler
    {
        PropertyProviderInfoT ProviderInfo { get; }

        IList<PropertyElement> HandleResponse(byte[] data);

        void HandleResponse(object model, IEnumerable<PropertyProviderAttribute> properties, byte[] data);
    }
}
