using AutoMapper;
using System;
using System.Reflection;
using xLibV100.Attributes;
using xLibV100.Common;

namespace xLibV100.Controls
{
    public class SynchronizedPropertyAttribute : Attribute
    {
        protected object propertyId;
        protected IValueConverter ValueConverter;
        protected PropertyInfo PropertyInfo;

        public bool CopyingByMapping;

        public SynchronizedPropertySelectorAttribute PropertySelector { get; protected set; }

        public Type Type
        {
            get => null;
            set
            {
                ValueConverter = xMemory.GetValueConverter(value);
            }
        }

        public object PropertyId
        {
            get => propertyId;
            set
            {
                propertyId = value;
                PropertySelector = EnumHelper.GetEnumAttribute<SynchronizedPropertySelectorAttribute>(propertyId);
            }
        }

        public void Apply(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;

            if (ValueConverter == null)
            {
                ValueConverter = xMemory.GetValueConverter(propertyInfo.PropertyType);
            }
        }

        public object GetValue(byte[] source, int offset = 0, int limit = int.MaxValue)
        {
            return ValueConverter?.GetValue(source, offset, limit);
        }

        public void SetValue(object model, byte[] source, int offset = 0, int limit = int.MaxValue)
        {
            if (ValueConverter == null || PropertyInfo == null)
            {
                return;
            }

            try
            {
                PropertyInfo.SetValue(model, ValueConverter.GetValue(source, offset, limit));

                if (CopyingByMapping)
                {
                    var value = ValueConverter.GetValue(source, offset, limit);

                    if (value != null)
                    {
                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap(value.GetType(), model.GetType());
                        });

                        IMapper mapper = config.CreateMapper();
                        mapper.Map(value, model);
                    }
                }
            }
            catch
            {

            }
        }

        /*public int Apply()
        {

        }*/
    }
}
