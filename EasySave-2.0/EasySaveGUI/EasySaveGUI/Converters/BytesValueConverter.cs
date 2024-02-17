using System;
using System.Globalization;
using System.Windows.Data;

namespace EasySaveGUI.Converters
{
    public class BytesValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long lByteCount = 0;

            if (value is long l)
            {
                lByteCount = l;
            }
            else if (value is int i)
            {
                lByteCount = i;
            }
            else if (value is double d)
            {
                lByteCount = (long)d;
            }

            if (lByteCount < 1024)
                return $"{lByteCount} octets";
            else if (lByteCount < 1048576)
                return $"{lByteCount / 1024:N1} Ko";
            else if (lByteCount < 1073741824)
                return $"{lByteCount / 1048576:N1} Mo";
            else
                return $"{lByteCount / 1073741824:N1} Go";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
