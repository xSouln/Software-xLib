using System;
using System.Collections.Generic;
using System.Linq;
using xLibV100.Common;

namespace xLibV100.Adaptation
{
    public class GetterPropertyProviderResponseHandler : IGetterPropertyProviderResponseHandler
    {
        protected PropertyProviderInfoT providerInfo = new PropertyProviderInfoT();

        public PropertyProviderInfoT ProviderInfo => providerInfo;

        public void HandleResponse(object model, IEnumerable<PropertyProviderAttribute> properties, byte[] data)
        {
            xMemoryReader memoryReader = new xMemoryReader(data);

            try
            {
                providerInfo = memoryReader.GetValue<PropertyProviderInfoT>();

                if (providerInfo.AdaptionMode == PropertyAdaptionMode.ByRange)
                {
                    ushort id = memoryReader.GetValue<ushort>();

                    while (memoryReader.RemainLength > 0)
                    {
                        var propertyInfo = providerInfo.PropertiesInfoIsIncluded
                            ? memoryReader.GetValue<PropertyInfoT>() : new PropertyInfoT();

                        ushort propertyId = providerInfo.PropertiesIdsIsIncluded
                            ? memoryReader.GetValue<ushort>() : id;

                        PropertyProviderAttribute attribute = properties.FirstOrDefault(x => x.PropertyId == propertyId);

                        if (attribute == null && !providerInfo.PropertiesInfoIsIncluded)
                        {
                            throw new Exception("uncertainty of covertation");
                        }

                        attribute?.SetValue(model, memoryReader);

                        id++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<ProvidedProperty> HandleResponse(byte[] data)
        {
            return null;
        }
    }

    public class GetterPropertyProviderByRange : IGetterPropertyProvider
    {
        public PropertyAdaptionMode AdaptionMode => PropertyAdaptionMode.ByRange;

        public GetterPropertyAdaptionFlags Flags { get; }

        public ushort StartId { get; protected set; }

        public ushort CountOfIds { get; protected set; }


        public GetterPropertyProviderByRange(ushort startId = 0,
            ushort countOfIds = ushort.MaxValue,
            GetterPropertyAdaptionFlags flags = GetterPropertyAdaptionFlags.None)
        {
            Flags = flags;
            StartId = startId;
            CountOfIds = countOfIds;
        }

        public byte[] ComposeRequest()
        {
            List<byte> content = new List<byte>();

            xMemory.Add(content, AdaptionMode);
            xMemory.Add(content, Flags);

            xMemory.Add(content, StartId);
            xMemory.Add(content, CountOfIds);

            return content.ToArray();
        }
    }

    public class GetterPropertyProviderByIds : IGetterPropertyProvider
    {
        public PropertyAdaptionMode AdaptionMode => PropertyAdaptionMode.ByIds;

        public GetterPropertyAdaptionFlags Flags { get; }

        public List<ushort> Ids { get; protected set; }

        public GetterPropertyProviderByIds(IList<ushort> ids = null, GetterPropertyAdaptionFlags flags = GetterPropertyAdaptionFlags.None)
        {
            Ids = new List<ushort>();

            if (ids != null)
            {
                Ids.AddRange(ids);
            }
        }

        public int Add(ushort id)
        {
            Ids.Add(id);

            return 0;
        }

        public byte[] ComposeRequest()
        {
            List<byte> content = new List<byte>();

            xMemory.Add(content, AdaptionMode);
            xMemory.Add(content, Flags);

            foreach (var id in Ids)
            {
                xMemory.Add(content, id);
            }

            return content.ToArray();
        }
    }
}
