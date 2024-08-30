using System;
using System.Collections.Generic;
using System.Reflection;
using xLibV100.Adaptation;
using xLibV100.Controls;

namespace xLibV100.Peripherals
{
    public class SynchronizedPropertyControl
    {
        public List<SynchronizedPropertyAttribute> SynchronizedProperties = new List<SynchronizedPropertyAttribute>();

        public SynchronizedPropertyControl(Type type)
        {
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute(typeof(SynchronizedPropertyAttribute)) is SynchronizedPropertyAttribute attribute)
                {
                    attribute.Apply(property);
                    SynchronizedProperties.Add(attribute);
                }
            }
        }

        public SynchronizedPropertyAttribute FineById(int id)
        {
            foreach (var synchronizedProperty in SynchronizedProperties)
            {
                if ((int)synchronizedProperty.PropertyId == id)
                {
                    return synchronizedProperty;
                }
            }

            return null;
        }

        public int Apply<T>(IEnumerable<T> properties) where T : ReadableProperty
        {
            if (properties == null)
            {
                return -1;
            }

            foreach (var property in properties)
            {
                var synchronizedProperty = FineById(property.Info.Id);
                synchronizedProperty?.SetValue(this, property.Content);
            }

            return 0;
        }
    }
}
