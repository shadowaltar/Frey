using System;
using System.Globalization;
using System.Windows.Data;

namespace Maintenance.PagReport.Utils
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class BooleanStringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "Included";
            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}