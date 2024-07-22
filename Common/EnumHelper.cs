using System;

namespace xLibV100.Common
{
    public static class EnumHelper
    {
        public static TAttribute GetEnumAttribute<TEnum, TAttribute>(TEnum value)
            where TEnum : Enum
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(TAttribute), false);
                if (attributes.Length > 0)
                {
                    return (TAttribute)attributes[0];
                }
            }
            return null;
        }

        public static TAttribute GetEnumAttribute<TAttribute>(object value)
            where TAttribute : Attribute
        {
            if (value == null)
            {
                return null;
            }

            var type = value.GetType();

            if (!type.IsEnum)
            {
                return null;
            }

            var memberInfo = type.GetMember(value.ToString());
            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(TAttribute), false);
                if (attributes.Length > 0)
                {
                    return (TAttribute)attributes[0];
                }
            }

            return null;
        }
    }
}
