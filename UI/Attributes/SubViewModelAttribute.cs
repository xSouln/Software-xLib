using System;

namespace xLibV100.UI
{
    public class SubViewModelAttribute : Attribute
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public object Section { get; set; }
    }
}
