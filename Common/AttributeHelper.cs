using System;
using System.Reflection;

namespace xLibV100.Common
{
    public static class AttributeHelper
    {
        public static T GetAttribute<T>(object obj) where T : Attribute
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.GetType().GetCustomAttribute<T>();
        }
    }
}
