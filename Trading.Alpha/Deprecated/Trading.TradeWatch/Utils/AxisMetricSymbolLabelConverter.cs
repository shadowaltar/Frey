using System;
using System.Windows.Data;

namespace Trading.TradeWatch.Utils
{
    [ValueConversion(typeof(long), typeof(string))]
    public class AxisMetricSymbolLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = Double.Parse(value.ToString(), System.Globalization.NumberStyles.Float);
            double exponent = Math.Log10(Math.Abs(val));
            if (Math.Abs(val) >= 1)
            {
                switch ((int)Math.Floor(exponent))
                {
                    case 0:
                    case 1:
                    case 2:
                        return val.ToString();
                    case 3:
                    case 4:
                    case 5:
                        return (val / 1e3).ToString() + "k";
                    case 6:
                    case 7:
                    case 8:
                        return (val / 1e6).ToString() + "M";
                    case 9:
                    case 10:
                    case 11:
                        return (val / 1e9).ToString() + "G";
                    case 12:
                    case 13:
                    case 14:
                        return (val / 1e12).ToString() + "T";
                    case 15:
                    case 16:
                    case 17:
                        return (val / 1e15).ToString() + "P";
                    case 18:
                    case 19:
                    case 20:
                        return (val / 1e18).ToString() + "E";
                    case 21:
                    case 22:
                    case 23:
                        return (val / 1e21).ToString() + "Z";
                    default:
                        return (val / 1e24).ToString() + "Y";
                }
            }
            else
            {
                if (Math.Abs(val) > 0)
                {
                    switch ((int)Math.Floor(exponent))
                    {
                        case -1:
                        case -2:
                        case -3:
                            return (val * 1e3).ToString() + "m";
                        case -4:
                        case -5:
                        case -6:
                            return (val * 1e6).ToString() + "μ";
                        case -7:
                        case -8:
                        case -9:
                            return (val * 1e9).ToString() + "n";
                        case -10:
                        case -11:
                        case -12:
                            return (val * 1e12).ToString() + "p";
                        case -13:
                        case -14:
                        case -15:
                            return (val * 1e15).ToString() + "f";
                        case -16:
                        case -17:
                        case -18:
                            return (val * 1e15).ToString() + "a";
                        case -19:
                        case -20:
                        case -21:
                            return (val * 1e15).ToString() + "z";
                        default:
                            return (val * 1e15).ToString() + "y";
                    }
                }
                return "0";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}