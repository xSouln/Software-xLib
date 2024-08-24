using System;

namespace xLibV100.Serialization
{
    public interface ISerializer
    {
        Association GetAssociation(string typeName, AssociatedMode associatedMode = AssociatedMode.SerializableType);

        object Build(Serialization serialization, params object[] args);

        object Open(string path, params object[] args);

        int Save(string path = null, params object[] args);
    }
};
