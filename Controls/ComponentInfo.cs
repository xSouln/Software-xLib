namespace xLibV100.Controls
{
    public class ComponentInfo
    {
        public object Holder;
        public object Component;

        public object Name;

        public ComponentInfo(object holder = null, object component = null, object name = null)
        {
            Holder = holder;
            Component = component;
            Name = name;
        }
    }
}
