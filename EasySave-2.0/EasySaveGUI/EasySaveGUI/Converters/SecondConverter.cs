using System;
using System.Globalization;
using System.Windows.Data;

namespace EasySaveGUI.Converters
{
    public class SecondConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long lMs = 0;

            if (value is long l)
            {
                lMs = l;
            }
            else if (value is int i)
            {
                lMs = i;
            }
            else if (value is double d)
            {
                lMs = (long)d;
            }

            // Convert milliseconds to other units
            long totalSeconds = lMs / 1000;
            long totalMinutes = totalSeconds / 60;
            long totalHours = totalMinutes / 60;
            long totalDays = totalHours / 24;

            // Define abbreviations
            string abbreviationMs = culture.Name == "fr-FR" ? "ms" : "ms";
            string abbreviationSec = culture.Name == "fr-FR" ? "s" : "s";
            string abbreviationMin = culture.Name == "fr-FR" ? "min" : "min";
            string abbreviationHr = culture.Name == "fr-FR" ? "h" : "hr";
            string abbreviationDay = culture.Name == "fr-FR" ? "j" : "day(s)";

            // Construct the readable time string
            if (totalDays > 0)
            {
                return $"{totalDays} {abbreviationDay} {totalHours % 24} {abbreviationHr} {totalMinutes % 60} {abbreviationMin} {totalSeconds % 60} {abbreviationSec}";
            }
            else if (totalHours > 0)
            {
                return $"{totalHours} {abbreviationHr} {totalMinutes % 60} {abbreviationMin} {totalSeconds % 60} {abbreviationSec}";
            }
            else if (totalMinutes > 0)
            {
                return $"{totalMinutes} {abbreviationMin} {totalSeconds % 60} {abbreviationSec}";
            }
            else if (totalSeconds > 0)
            {
                return $"{totalSeconds} {abbreviationSec} {lMs % 1000} {abbreviationMs}";
            }
            else
            {
                return $"{lMs} {abbreviationMs}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
