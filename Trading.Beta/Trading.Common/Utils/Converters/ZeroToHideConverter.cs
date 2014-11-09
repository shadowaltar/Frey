using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Trading.Common.Utils.Converters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class ZeroToHideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (System.Convert.ToInt64(value) == 0)
                return Visibility.Hidden;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}