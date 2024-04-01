using System;

namespace xLibV100.UI
{
    public class ContextMenuConstructorAttribute : Attribute
    {
        public string Subgroup;
        public object Content;
        public object Parameter;
    }
}
