using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Maintenance.CompositeBenchmarks.Utils
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (Visibility)value;
            if (v == Visibility.Collapsed || v == Visibility.Hidden)
                return null;
            return default(object);
        }
    }
}