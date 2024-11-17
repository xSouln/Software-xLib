namespace xLibV100.Adaptation
{
    public class ProvidedProperty
    {
        public PropertyInfoT Info;
        public ushort Id;

        public byte[] Content { get; protected set; }

        public ProvidedProperty(ushort id, byte[] content, PropertyInfoT info = default)
        {
            Id = id;
            Content = content;
            Info = info;
        }
    }
}
