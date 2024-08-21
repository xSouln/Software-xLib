using System.Globalization;
using System;
using System.Windows.Data;

namespace xLibV100.UI.ValueConverters
{
    public class PropertyCheckConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Логика проверки наличия свойства или значения
            return value ?? parameter; // Возвращает параметр если значение null
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
