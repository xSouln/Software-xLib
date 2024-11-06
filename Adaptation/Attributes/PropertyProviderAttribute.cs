using System;
using System.Reflection;
using xLibV100.Common;

namespace xLibV100.Adaptation
{
    public class PropertyProviderAttribute : Attribute
    {
        public PropertySizeInfo SizeInfo;
        public PropertyTypeInfo Info;
        public PropertyInfo Property;

        public ushort PropertyId;

        public virtual int SetValue(object model, byte[] data)
        {
            return 0;
        }

        public virtual void SetValue(object model, xMemoryReader memoryReader)
        {
            return;
        }

        public virtual byte[] GetValue(object model)
        {
            return null;
        }
    }
}
