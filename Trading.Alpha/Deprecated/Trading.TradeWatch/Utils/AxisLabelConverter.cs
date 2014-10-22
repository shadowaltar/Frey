using System;
using System.Windows.Data;

namespace Trading.TradeWatch.Utils
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class AxisLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime date;
            if (value is DateTime)
                date = (DateTime)value;
            else
                date = DateTime.Parse((string)value);
            return String.Format("{0:MMM}" + Environment.NewLine + "{0:yy}", date);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}