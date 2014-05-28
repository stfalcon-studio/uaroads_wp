using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EveWP8.Converters
{
    public class BooleanToOppositeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !value.Equals(Visibility.Visible);
        }
    }
}