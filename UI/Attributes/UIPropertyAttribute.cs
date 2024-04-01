using System;
using System.Windows;

namespace xLibV100.UI
{
    [Flags]
    public enum UIPropertyFlag
    {
        IsClear,

        IsWritable = 1 << 0,
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class UIPropertyAttribute : Attribute
    {
        public UIPropertyFlag Flags = UIPropertyFlag.IsClear;

        public string Name;
        public string Associated;
        public object Section;

        public DataTemplate DataTemplate;

        public UIPropertyAttribute(UIPropertyFlag flags)
        {
            Flags = flags;
        }

        public UIPropertyAttribute(UIPropertyFlag flags, string associatedName)
        {
            Flags = flags;
            Associated = associatedName;
        }

        public UIPropertyAttribute()
        {

        }
    }
}
