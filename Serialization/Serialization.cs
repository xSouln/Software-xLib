namespace xLibV100.Serialization
{
    public class Serialization
    {
        public string SerializationType { get; set; }

        public Serialization()
        {

        }

        public Serialization(ISerializable serializable = null)
        {

        }

        public virtual int ApplyTo(ISerializable serializable)
        {
            return -1;
        }

        public virtual int GetFrom(ISerializable serializable)
        {
            return -1;
        }
    }
}
