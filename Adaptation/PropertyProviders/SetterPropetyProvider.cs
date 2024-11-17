using System.Collections.Generic;
using System.Linq;
using xLibV100.Common;

namespace xLibV100.Adaptation
{
    public class SetterPropertyProviderByRange : ISetterPropertyProvider
    {
        public PropertyAdaptionMode AdaptionMode => PropertyAdaptionMode.ByRange;

        public SetterPropertyAdaptionFlags Flags { get; }

        public ushort StartId { get; protected set; }

        public List<ProvidedProperty> Properties { get; protected set; } = new List<ProvidedProperty>();


        public SetterPropertyProviderByRange(IEnumerable<byte[]> propertiesContent, ushort startId = 0)
        {
            StartId = startId;

            Properties = propertiesContent
                .Select((content, index) => new ProvidedProperty((ushort)(startId + index), content))
                .ToList();
        }

        public SetterPropertyProviderByRange(IEnumerable<ProvidedProperty> properties,
            int startId = 0,
            SetterPropertyAdaptionFlags adaptionFlags = SetterPropertyAdaptionFlags.None)
        {
            Flags = adaptionFlags;
            StartId = (ushort)startId;
            Properties = properties.ToList();
        }

        public byte[] ComposeRequest()
        {
            List<byte> content = new List<byte>();

            xMemory.Add(content, AdaptionMode);
            xMemory.Add(content, Flags);

            xMemory.Add(content, StartId);
            xMemory.Add(content, (ushort)Properties.Count);

            bool addInfo = BitsFieldHelper.GetState(Flags, SetterPropertyAdaptionFlags.IncludeInfo);
            bool addId = BitsFieldHelper.GetState(Flags, SetterPropertyAdaptionFlags.IncludeIds);
            bool addSize = BitsFieldHelper.GetState(Flags, SetterPropertyAdaptionFlags.IncludeSize);

            foreach (var property in Properties)
            {
                if (addInfo)
                {
                    xMemory.Add(content, property.Info);
                }

                if (addId)
                {
                    xMemory.Add(content, (ushort)property.Id);
                }

                if (addSize)
                {
                    xMemory.Add(content, (ushort)property.Content.Length);
                }

                xMemory.Add(content, property.Content);
            }

            return content.ToArray();
        }
    }
}
