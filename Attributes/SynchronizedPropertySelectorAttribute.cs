using System;
using xLibV100.Adaptation;

namespace xLibV100.Attributes
{
    public class SynchronizedPropertySelectorAttribute : Attribute
    {
        public SynchronizedPropertyTypes Type;
        public SynchronizedPropertyFlags Flags;
    }
}
