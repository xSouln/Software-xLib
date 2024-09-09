using System;
using xLibV100.Adaptation;

namespace xLibV100.Attributes
{
    public class SynchronizedPropertySelectorAttribute : Attribute
    {
        protected Type type;

        public SPropertyTypes Type;
        //public RWPropertyFlags Flags;

        /*public Type Type
        {
            get => type;
            set
            {
                type = value;

            }
        }*/
    }
}
