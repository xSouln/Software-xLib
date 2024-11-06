using System;
using System.Collections.Generic;
using System.Linq;
using xLibV100.Common;

namespace xLibV100.Adaptation
{
    public class PropertyElement
    {
        public PropertyInfoT Info = new PropertyInfoT();
        public ushort Id;

        public byte[] Content { get; protected set; }

        public PropertyElement()
        {

        }

        public int Apply<TValue>(ushort id, TValue value) where TValue : unmanaged
        {
            var type = typeof(TValue);

            Info.TypeInfo = PropertyTypeInfo.BaseType;
            Id = id;

            Content = xMemory.ToByteArray((object)value);

            return 0;
        }

        public int Apply(ushort id, string content)
        {
            //Info.PropertyId = id;
            Info.TypeInfo = PropertyTypeInfo.String;
            Info.SizeInfo = PropertySizeInfo.Byte;

            Content = xMemory.ToByteArray(content);

            return 0;
        }
    }

    public class GetterPropertyProviderResponseHandler : IGetterPropertyProviderResponseHandler
    {
        protected PropertyProviderInfoT providerInfo = new PropertyProviderInfoT();

        public PropertyProviderInfoT ProviderInfo => providerInfo;

        public IList<PropertyElement> HandleResponse(byte[] data)
        {
            return null;
        }

        public void HandleResponse(object model, IEnumerable<PropertyProviderAttribute> properties, byte[] data)
        {
            xMemoryReader memoryReader = new xMemoryReader(data);

            try
            {
                providerInfo = memoryReader.GetValue<PropertyProviderInfoT>();

                if (providerInfo.AdaptionMode == PropertyAdaptionMode.ByRange)
                {
                    int id = memoryReader.GetValue<ushort>();

                    while (memoryReader.RemainLength > 0)
                    {
                        var propertyInfo = providerInfo.PropertiesInfoIsIncluded
                            ? memoryReader.GetValue<PropertyInfoT>() : new PropertyInfoT();

                        ushort propertyId = providerInfo.PropertiesIdsIsIncluded
                            ? memoryReader.GetValue<ushort>() : ushort.MaxValue;

                        PropertyProviderAttribute attribute = properties.FirstOrDefault(x => x.PropertyId == id);

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
    }

    public class GetterPropertyProviderByRange : IGetterPropertyProvider
    {
        public PropertyAdaptionMode AdaptionMode => PropertyAdaptionMode.ByRange;

        public PropertyAdaptionFlags Flags { get; }

        public ushort StartId { get; protected set; }

        public ushort CountOfIds { get; protected set; }


        public GetterPropertyProviderByRange(ushort startId = 0,
            ushort countOfIds = ushort.MaxValue,
            PropertyAdaptionFlags flags = PropertyAdaptionFlags.None)
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

        public PropertyAdaptionFlags Flags { get; }

        public List<ushort> Ids { get; protected set; }

        public GetterPropertyProviderByIds(IList<ushort> ids = null, PropertyAdaptionFlags flags = PropertyAdaptionFlags.None)
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
