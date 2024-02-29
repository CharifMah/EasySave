using System;
using System.Globalization;
using System.Windows.Data;

namespace EasySaveGUI.Converters
{
    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan))
                throw new InvalidCastException("Value must be a TimeSpan");

            TimeSpan timeSpan = (TimeSpan)value;

            long milliseconds = (long)timeSpan.TotalMilliseconds;
            long seconds = (long)timeSpan.TotalSeconds;
            long minutes = (long)timeSpan.TotalMinutes;

            string formatted = "";

            if (milliseconds > 0 && milliseconds < 300)
            {
                formatted = milliseconds + " ms ";
            }
            else if (seconds > 0 && seconds < 60)
            {
                formatted = seconds + " sec ";
            }
            if (minutes > 0)
            {
                formatted = minutes + " min" + seconds + " sec ";
            }

            return formatted.Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
