using System;
using System.Collections.Generic;
using System.IO;
using xLibV100.Common;

namespace xLibV100.Serialization
{
    public abstract class Serializer : ISerializer
    {
        public string Path { get; set; }
        public List<Association> Associations = new List<Association>();

        public Serializer(string path = null)
        {
            Path = path;
        }

        public virtual object Build(Serialization serialization, params object[] args)
        {
            if (serialization == null)
            {
                return null;
            }

            var association = GetAssociation(serialization.GetType().FullName, associatedMode: AssociatedMode.SerializationType);

            if (association == null)
            {
                return null;
            }

            ISerializable result = Activator.CreateInstance(association.SerializableType, args) as ISerializable;
            serialization.ApplyTo(result);

            return result;
        }

        public virtual Association GetAssociation(string typeName, AssociatedMode associatedMode = AssociatedMode.SerializableType)
        {
            if (typeName == null || typeName.Length == 0)
            {
                return null;
            }

            if (associatedMode == AssociatedMode.SerializableType)
            {
                foreach (var association in Associations)
                {
                    if (association.SerializableType.FullName == typeName)
                    {
                        return association;
                    }
                }
            }
            else if (associatedMode == AssociatedMode.SerializationType)
            {
                foreach (var association in Associations)
                {
                    if (association.SerializationType.FullName == typeName)
                    {
                        return association;
                    }
                }
            }

            return null;
        }

        public virtual object Open(string path = null, params object[] args)
        {
            return null;
        }

        public virtual int Save(string path = null, params object[] args)
        {
            return -1;
        }
    }
}
