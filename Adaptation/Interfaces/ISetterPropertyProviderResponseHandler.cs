using System.Collections.Generic;

namespace xLibV100.Adaptation
{
    public interface ISetterPropertyProviderResponseHandler
    {
        PropertyProviderInfoT ProviderInfo { get; }

        IList<ProvidedProperty> HandleResponse(byte[] data);

        void HandleResponse(object model, IEnumerable<PropertyProviderAttribute> properties, byte[] data);
    }
}
