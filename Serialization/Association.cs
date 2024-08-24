using System;

namespace xLibV100.Serialization
{
    public class Association
    {
        public Type SerializableType;
        public Type SerializationType;

        public Association(Type serializableType, Type serializationType)
        {
            SerializableType = serializableType;
            SerializationType = serializationType;
        }
    }
}
