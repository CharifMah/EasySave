using System;
using System.Globalization;
using System.Windows.Data;

namespace EasySaveGUI.Converters
{
    public class SecondConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long lSecondCount = 0;

            if (value is long l)
            {
                lSecondCount = l;
            }
            else if (value is int i)
            {
                lSecondCount = i;
            }
            else if (value is double d)
            {
                lSecondCount = (long)d;
            }

            string lAbbreviationSec = culture.Name == "fr-FR" ? "sec" : "sec";
            string lAbbreviationMin = culture.Name == "fr-FR" ? "min" : "min";
            string lAbbreviationHr = culture.Name == "fr-FR" ? "h" : "hr";
            string lAbbreviationDay = culture.Name == "fr-FR" ? "j" : "day(s)";

            if (lSecondCount < 60)
                return $"{lSecondCount} {lAbbreviationSec}";
            else if (lSecondCount < 3600)
                return $"{lSecondCount / 60} {lAbbreviationMin} {lSecondCount % 60} {lAbbreviationSec}";
            else if (lSecondCount < 86400)
                return $"{lSecondCount / 3600} {lAbbreviationHr} {lSecondCount % 3600 / 60} {lAbbreviationMin} {lSecondCount % 60} {lAbbreviationSec}";
            else
                return $"{lSecondCount / 86400} {lAbbreviationDay} {lSecondCount % 86400 / 3600} {lAbbreviationHr} {lSecondCount % 3600 / 60} {lAbbreviationMin} {lSecondCount % 60} {lAbbreviationSec}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
