using System;

namespace xLibV100.Controls
{
    [Flags]
    public enum ModelFunctionFlags
    {
        None = 0,
        Asynchronous = 1 << 0,
    }

    public class ModelFunctionAttribute : Attribute
    {
        public string Name = null;

        public string Description = null;
        public string Group = null;

        public ModelFunctionFlags Flags;
    }
}
