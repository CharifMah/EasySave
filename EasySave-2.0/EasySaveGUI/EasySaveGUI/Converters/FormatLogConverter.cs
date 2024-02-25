using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace EasySaveGUI.Converters
{
    public class FormatLogConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Dictionary<int, string> dict)
            {
                return dict.Select(kvp => GetItemString(kvp));
            }

            return null;
        }

        private string GetItemString(KeyValuePair<int, string> kvp)
        {
            return $"{kvp.Value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
