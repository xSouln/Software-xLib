using System;

namespace xLibV100.Controls
{
    public class ModelAttribute : Attribute
    {
        public string Key;
        public Type ViewModelType;
    }
}
