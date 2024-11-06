using System.Collections.Generic;
using xLibV100.Common;

namespace xLibV100.Adaptation
{
    public class SetterPropertyProviderByRange : IGetterPropertyProvider
    {
        public PropertyAdaptionMode AdaptionMode => PropertyAdaptionMode.ByRange;

        public PropertyAdaptionFlags Flags { get; }

        public ushort StartId { get; protected set; }

        public byte[] Content { get; protected set; }


        public SetterPropertyProviderByRange(byte[] content, 
            ushort startId = 0,
            PropertyAdaptionFlags flags = PropertyAdaptionFlags.None)
        {
            Flags = flags;
            StartId = startId;
            Content = content;
        }

        public byte[] ComposeRequest()
        {
            List<byte> content = new List<byte>();

            xMemory.Add(content, AdaptionMode);
            xMemory.Add(content, Flags);

            xMemory.Add(content, StartId);
            xMemory.Add(content, (ushort)Content.Length);

            xMemory.Add(content, Content);

            return content.ToArray();
        }
    }
}
